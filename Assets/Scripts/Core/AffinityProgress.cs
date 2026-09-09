using System;

namespace DongAriGame.Core
{
    [Serializable]
    public sealed class AffinityProgress
    {
        public int Red;
        public int Blue;
        public int White;

        public void Reset()
        {
            Red = 0;
            Blue = 0;
            White = 0;
        }

        public void Add(AffinityType type, int amount = 1)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));

            switch (type)
            {
                case AffinityType.Red: Red += amount; break;
                case AffinityType.Blue: Blue += amount; break;
                case AffinityType.White: White += amount; break;
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public StatBlock ApplyTo(StatBlock baseStats)
        {
            if (baseStats == null) throw new ArgumentNullException(nameof(baseStats));

            StatBlock result = baseStats.Copy();
            result.AttackPower *= 1f + Red * 0.08f;
            result.CriticalChance = Math.Min(0.75f, result.CriticalChance + Red * 0.025f);
            result.AttackSpeed *= 1f + Blue * 0.06f;
            result.MoveSpeed *= 1f + Blue * 0.04f;
            result.EvasionChance = Math.Min(0.60f, result.EvasionChance + Blue * 0.02f);
            result.AttackPower += White * 2f;
            result.MaxHealth += White * 8f;
            return result;
        }
    }
}
