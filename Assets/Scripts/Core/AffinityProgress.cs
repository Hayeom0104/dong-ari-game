using System;
using System.Collections.Generic;

namespace DongAriGame.Core
{
    [Serializable]
    public sealed class AffinityProgress
    {
        private readonly Dictionary<ArtifactDefinition, int> artifacts = new Dictionary<ArtifactDefinition, int>();

        public int Count(ArtifactDefinition artifact) => artifacts.TryGetValue(artifact, out int count) ? count : 0;

        public void AddArtifact(ArtifactDefinition artifact)
        {
            if (artifact == null) throw new ArgumentNullException(nameof(artifact));
            Add(artifact.Color);
            artifacts[artifact] = Count(artifact) + 1;
        }

        public int Red;
        public int Blue;
        public int White;

        public void Reset()
        {
            artifacts.Clear();
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
            float damage = Red * 0.08f, crit = Red * 0.025f;
            float attackSpeed = Blue * 0.06f, moveSpeed = Blue * 0.04f, evade = Blue * 0.02f;
            float attack = White * 2f, health = White * 8f;
            foreach (var entry in artifacts)
            {
                ArtifactDefinition item = entry.Key;
                int count = entry.Value;
                damage += item.DamageBonus * count;
                crit += item.CriticalBonus * count;
                attackSpeed += item.AttackSpeedBonus * count;
                moveSpeed += item.MoveSpeedBonus * count;
                evade += item.EvasionBonus * count;
                attack += item.FlatAttack * count;
                health += item.FlatHealth * count;
            }
            // Percent damage applies to base attack; white remains a flat addition.
            result.AttackPower = baseStats.AttackPower * (1f + damage) + attack;
            result.CriticalChance = Math.Min(0.75f, baseStats.CriticalChance + crit);
            result.AttackSpeed *= 1f + attackSpeed;
            result.MoveSpeed *= 1f + moveSpeed;
            result.EvasionChance = Math.Min(0.60f, baseStats.EvasionChance + evade);
            result.MaxHealth += health;
            return result;
        }
    }
}
