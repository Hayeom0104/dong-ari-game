using DongAriGame.Core;
using NUnit.Framework;

namespace DongAriGame.Tests
{
    public sealed class CoreRulesTests
    {
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
