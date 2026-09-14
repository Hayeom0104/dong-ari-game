using System;
using System.Collections.Generic;

namespace DongAriGame.Core
{
    // Immutable catalog; every artifact grants one color point plus its own bonus.
    public sealed class ArtifactDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public AffinityType Color { get; }
        public string BonusText { get; }
        public float DamageBonus { get; }
        public float CriticalBonus { get; }
        public float AttackSpeedBonus { get; }
        public float MoveSpeedBonus { get; }
        public float EvasionBonus { get; }
        public float FlatAttack { get; }
        public float FlatHealth { get; }

        private ArtifactDefinition(string id, string name, AffinityType color, string text,
            float damage = 0f, float critical = 0f, float attackSpeed = 0f,
            float moveSpeed = 0f, float evasion = 0f, float attack = 0f, float health = 0f)
        {
            Id = id; Name = name; Color = color; BonusText = text;
            DamageBonus = damage; CriticalBonus = critical; AttackSpeedBonus = attackSpeed;
            MoveSpeedBonus = moveSpeed; EvasionBonus = evasion; FlatAttack = attack; FlatHealth = health;
        }

        public static IReadOnlyList<ArtifactDefinition> All { get; } = Array.AsReadOnly(new[]
        {
            new ArtifactDefinition("ember", "불씨의 핵", AffinityType.Red, "데미지 +8%", damage: 0.08f),
            new ArtifactDefinition("ruby", "루비 렌즈", AffinityType.Red, "치명타 확률 +5%p", critical: 0.05f),
            new ArtifactDefinition("banner", "진홍 깃발", AffinityType.Red, "데미지 +4% · 치명타 +2.5%p", damage: 0.04f, critical: 0.025f),
            new ArtifactDefinition("clock", "푸른 태엽", AffinityType.Blue, "공격속도 +12%", attackSpeed: 0.12f),
            new ArtifactDefinition("boots", "바람 장화", AffinityType.Blue, "이동속도 +8%", moveSpeed: 0.08f),
            new ArtifactDefinition("veil", "안개 망토", AffinityType.Blue, "회피 확률 +4%p", evasion: 0.04f),
            new ArtifactDefinition("whetstone", "백색 숫돌", AffinityType.White, "고정 공격력 +4", attack: 4f),
            new ArtifactDefinition("heart", "대리석 심장", AffinityType.White, "최대 체력 +20", health: 20f),
            new ArtifactDefinition("seal", "상아 인장", AffinityType.White, "고정 공격력 +2 · 최대 체력 +10", attack: 2f, health: 10f)
        });

        public static string ColorText(AffinityType color) => color switch
        {
            AffinityType.Red => "빨강",
            AffinityType.Blue => "파랑",
            AffinityType.White => "흰색",
            _ => throw new ArgumentOutOfRangeException(nameof(color))
        };

        public static string ColorEffect(AffinityType color) => color switch
        {
            AffinityType.Red => "데미지 +8% · 치명타 +2.5%p",
            AffinityType.Blue => "공속 +6% · 이속 +4% · 회피 +2%p",
            AffinityType.White => "고정 공격력 +2 · 최대 체력 +8",
            _ => throw new ArgumentOutOfRangeException(nameof(color))
        };
    }
}
