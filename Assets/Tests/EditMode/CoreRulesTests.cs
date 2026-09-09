using DongAriGame.Core;
using NUnit.Framework;

namespace DongAriGame.Tests
{
    public sealed class CoreRulesTests
    {
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
    }
}

