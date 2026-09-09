using System;
using UnityEngine;

namespace DongAriGame.Gameplay
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maximum = 100f;
        [SerializeField] private CombatFaction faction;
        [SerializeField, Range(0f, 0.75f)] private float evasionChance;
        public float Current { get; private set; }
        public float Maximum => maximum;
        public bool IsDead => Current <= 0f;
        public CombatFaction Faction => faction;
        public event Action<float, float> Changed;
        public event Action Died;

        private void Awake()
        {
            Current = maximum;
        }

        public void Configure(float maxHealth, CombatFaction newFaction = CombatFaction.Neutral, float evasion = 0f)
        {
            maximum = Mathf.Max(1f, maxHealth);
            faction = newFaction;
            evasionChance = Mathf.Clamp(evasion, 0f, 0.75f);
            Current = maximum;
            Changed?.Invoke(Current, maximum);
        }

        public bool TryDamage(float amount, bool canEvade = true)
        {
            if (IsDead || amount <= 0f) return false;
            if (canEvade && Random.value < evasionChance) return false;
            Current = Mathf.Max(0f, Current - amount);
            Changed?.Invoke(Current, maximum);
            if (IsDead) Died?.Invoke();
            return true;
        }

        public void SetMaximum(float value)
        {
            maximum = Mathf.Max(1f, value);
            Current = Mathf.Min(Current, maximum);
            Changed?.Invoke(Current, maximum);
        }

        public void SetEvasion(float value)
        {
            evasionChance = Mathf.Clamp(value, 0f, 0.75f);
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;
            Current = Mathf.Min(maximum, Current + amount);
            Changed?.Invoke(Current, maximum);
        }
    }
}
