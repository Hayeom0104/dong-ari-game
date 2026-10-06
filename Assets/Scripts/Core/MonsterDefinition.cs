using System;
using System.Collections.Generic;

namespace DongAriGame.Core
{
    public enum MonsterMovement { Chase, Hop, Dash }

    public sealed class MonsterDefinition
    {
        public string Name { get; }
        public int SpriteRow { get; }
        public int FirstRoom { get; }
        public float HealthMultiplier { get; }
        public float Speed { get; }
        public float DamageMultiplier { get; }
        public float AttackInterval { get; }
        public MonsterMovement Movement { get; }
        public bool Variant { get; }

        private MonsterDefinition(string name, int row, int firstRoom, float hp, float speed,
            float damage, float interval, MonsterMovement movement, bool variant = false)
        {
            Name = name; SpriteRow = row; FirstRoom = firstRoom; HealthMultiplier = hp;
            Speed = speed; DamageMultiplier = damage; AttackInterval = interval;
            Movement = movement; Variant = variant;
        }

        public static IReadOnlyList<MonsterDefinition> All { get; } = Array.AsReadOnly(new[]
        {
            new MonsterDefinition("물방울 슬라임", 0, 1, 0.75f, 1.8f, 0.75f, 1.5f, MonsterMovement.Hop),
            new MonsterDefinition("고블린 정찰병", 1, 1, 0.85f, 2.5f, 0.85f, 1.1f, MonsterMovement.Chase),
            new MonsterDefinition("이끼 골렘", 2, 2, 1.6f, 1.05f, 1.25f, 1.8f, MonsterMovement.Chase),
            new MonsterDefinition("늪지 슬라임", 0, 3, 1.1f, 2.1f, 1.3f, 1.3f, MonsterMovement.Hop, true),
            new MonsterDefinition("고블린 돌격병", 1, 5, 1.05f, 2.1f, 1.1f, 1.25f, MonsterMovement.Dash, true),
            new MonsterDefinition("수정 골렘", 2, 7, 1.9f, 1.3f, 1.45f, 1.8f, MonsterMovement.Dash, true)
        });

        public static MonsterDefinition ForSpawn(int room, int wave, int slot)
        {
            if (room < 1 || room > RunProgress.TotalRooms) throw new ArgumentOutOfRangeException(nameof(room));
            if (wave < 1 || slot < 0) throw new ArgumentOutOfRangeException();
            int count = 0;
            foreach (var item in All) if (item.FirstRoom <= room) count++;
            int index = (int)(((long)room + wave + slot - 2) % count);
            foreach (var item in All)
                if (item.FirstRoom <= room && index-- == 0) return item;
            throw new InvalidOperationException("No eligible monster.");
        }
    }
}
