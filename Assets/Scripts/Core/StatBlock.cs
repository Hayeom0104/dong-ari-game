using System;

namespace DongAriGame.Core
{
    [Serializable]
    public sealed class StatBlock
    {
        public float MaxHealth = 100f;
        public float AttackPower = 10f;
        public float MoveSpeed = 5f;
        public float AttackSpeed = 1f;
        public float CriticalChance = 0.05f;
        public float EvasionChance;

        public StatBlock Copy()
        {
            return (StatBlock)MemberwiseClone();
        }
    }
}

