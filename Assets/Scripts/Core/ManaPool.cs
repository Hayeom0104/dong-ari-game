using System;

namespace DongAriGame.Core
{
    public sealed class ManaPool
    {
        public float Current { get; private set; }
        public float Maximum { get; }
        public float RecoveryPerSecond { get; }

        public ManaPool(float maximum, float recoveryPerSecond = 1f, float initial = 0f)
        {
            if (maximum <= 0f) throw new ArgumentOutOfRangeException(nameof(maximum));
            if (recoveryPerSecond < 0f) throw new ArgumentOutOfRangeException(nameof(recoveryPerSecond));
            Maximum = maximum;
            RecoveryPerSecond = recoveryPerSecond;
            Current = Math.Clamp(initial, 0f, maximum);
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime < 0f) throw new ArgumentOutOfRangeException(nameof(deltaTime));
            Current = Math.Min(Maximum, Current + RecoveryPerSecond * deltaTime);
        }

        public bool TrySpend(float amount)
        {
            if (amount < 0f) throw new ArgumentOutOfRangeException(nameof(amount));
            if (Current < amount) return false;
            Current -= amount;
            return true;
        }

        public void Fill()
        {
            Current = Maximum;
        }
    }
}
