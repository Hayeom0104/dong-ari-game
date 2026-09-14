using DongAriGame.Core;
using NUnit.Framework;

namespace DongAriGame.Tests
{
    public sealed class CoreRulesTests
    {
        [Test]
        public void ArtifactCatalogHasThreeUniqueItemsPerColor()
        {
            var ids = new System.Collections.Generic.HashSet<string>();
            int[] colors = new int[3];
            foreach (var item in ArtifactDefinition.All)
            {
                Assert.That(ids.Add(item.Id), Is.True);
                colors[(int)item.Color]++;
            }
            Assert.That(colors, Is.EqualTo(new[] { 3, 3, 3 }));
        }

        [Test]
        public void DuplicateArtifactsStackColorAndIndividualBonus()
        {
            var progress = new AffinityProgress();
            var ember = ArtifactDefinition.All[0];
            progress.AddArtifact(ember);
            progress.AddArtifact(ember);
            var original = new StatBlock { AttackPower = 10f, CriticalChance = 0f };
            var result = progress.ApplyTo(original);
            Assert.That(progress.Red, Is.EqualTo(2));
            Assert.That(progress.Count(ember), Is.EqualTo(2));
            Assert.That(result.AttackPower, Is.EqualTo(13.2f).Within(0.001f));
            Assert.That(result.CriticalChance, Is.EqualTo(0.05f).Within(0.001f));
            Assert.That(original.AttackPower, Is.EqualTo(10f));
        }

        [Test]
        public void MixedColorsPreserveFlatAttackAndDoNotDependOnPickupOrder()
        {
            var first = new AffinityProgress();
            var second = new AffinityProgress();
            first.AddArtifact(ArtifactDefinition.All[0]);
            first.AddArtifact(ArtifactDefinition.All[6]);
            second.AddArtifact(ArtifactDefinition.All[6]);
            second.AddArtifact(ArtifactDefinition.All[0]);
            var baseline = new StatBlock { AttackPower = 10f };
            Assert.That(first.ApplyTo(baseline).AttackPower, Is.EqualTo(17.6f).Within(0.001f));
            Assert.That(second.ApplyTo(baseline).AttackPower, Is.EqualTo(first.ApplyTo(baseline).AttackPower));
        }

        [Test]
        public void ArtifactCapsAndResetRemoveAllBonuses()
        {
            var progress = new AffinityProgress();
            for (int i = 0; i < 100; i++)
            {
                progress.AddArtifact(ArtifactDefinition.All[1]);
                progress.AddArtifact(ArtifactDefinition.All[5]);
            }
            var stats = progress.ApplyTo(new StatBlock());
            Assert.That(stats.CriticalChance, Is.EqualTo(0.75f));
            Assert.That(stats.EvasionChance, Is.EqualTo(0.60f));
            progress.Reset();
            Assert.That(progress.Red + progress.Blue + progress.White, Is.Zero);
            Assert.That(progress.Count(ArtifactDefinition.All[1]), Is.Zero);
            Assert.That(progress.ApplyTo(new StatBlock()).AttackPower, Is.EqualTo(10f));
        }

        [Test]
        public void ShopsAppearOnlyInRoomsFourAndNine()
        {
            var run = new RunProgress();
            for (int room = 1; room <= 10; room++)
            {
                Assert.That(run.IsShopRoom, Is.EqualTo(room == 4 || room == 9));
                run.CompleteCurrentRoom();
            }
            Assert.That(run.IsShopRoom, Is.False);
        }

        [Test]
        public void ShopPurchasesRespectBalanceAndDoNotGrantRoomRewards()
        {
            var run = new RunProgress();
            for (int i = 0; i < 3; i++) run.CompleteCurrentRoom();
            Assert.That(run.Gold, Is.EqualTo(180));
            Assert.That(run.TrySpendGold(181), Is.False);
            Assert.That(run.Gold, Is.EqualTo(180));
            Assert.That(run.TrySpendGold(180), Is.True);
            Assert.That(run.Gold, Is.Zero);
            run.CompleteCurrentRoom();
            Assert.That(run.CurrentRoom, Is.EqualTo(5));
            Assert.That(run.Gold, Is.Zero);
        }

        [Test]
        public void GoldResetsAndCannotBeSpentOutsideShop()
        {
            var run = new RunProgress();
            run.CompleteCurrentRoom();
            Assert.That(run.TrySpendGold(1), Is.False);
            Assert.That(run.Gold, Is.EqualTo(50));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => run.TrySpendGold(0));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => run.TrySpendGold(-1));
            run.Reset();
            Assert.That(run.Gold, Is.Zero);
            Assert.That(run.CurrentRoom, Is.EqualTo(1));
        }

        [Test]
        public void CompletedRunDoesNotGrantAdditionalGold()
        {
            var run = new RunProgress();
            for (int i = 0; i < 10; i++) run.CompleteCurrentRoom();
            int finalGold = run.Gold;
            run.CompleteCurrentRoom();
            Assert.That(run.Gold, Is.EqualTo(finalGold));
            Assert.That(run.CurrentRoom, Is.EqualTo(11));
        }

        [Test]
        public void ManaRecoversExactlyOnePerSecond()
        {
            var mana = new ManaPool(10f, 1f, 0f);
            mana.Tick(3.5f);
            Assert.That(mana.Current, Is.EqualTo(3.5f).Within(0.001f));
        }

        [Test]
        public void RunCompletesAfterTenRooms()
        {
            var run = new RunProgress();
            for (int i = 0; i < RunProgress.TotalRooms; i++) run.CompleteCurrentRoom();
            Assert.That(run.IsComplete, Is.True);
        }

        [Test]
        public void BlueAffinityRaisesMovementSpeed()
        {
            var progress = new AffinityProgress();
            progress.Add(AffinityType.Blue, 2);
            StatBlock result = progress.ApplyTo(new StatBlock { MoveSpeed = 5f });
            Assert.That(result.MoveSpeed, Is.EqualTo(5.4f).Within(0.001f));
        }

        [Test]
        public void WhiteAffinityAddsFlatHealthAndAttack()
        {
            var progress = new AffinityProgress();
            progress.Add(AffinityType.White, 2);
            StatBlock result = progress.ApplyTo(new StatBlock { MaxHealth = 100f, AttackPower = 10f });
            Assert.That(result.MaxHealth, Is.EqualTo(116f));
            Assert.That(result.AttackPower, Is.EqualTo(14f));
        }

        [Test]
        public void ManaCannotBeSpentBelowCost()
        {
            var mana = new ManaPool(10f, 1f, 2f);
            Assert.That(mana.TrySpend(3f), Is.False);
            Assert.That(mana.Current, Is.EqualTo(2f));
        }

        [Test]
        public void ManaIsCappedAtItsMaximum()
        {
            var mana = new ManaPool(50f, 1f, 49.5f);
            mana.Tick(10f);
            Assert.That(mana.Current, Is.EqualTo(50f));
        }
    }
}
