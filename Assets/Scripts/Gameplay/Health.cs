using System;
using UnityEngine;

namespace DongAriGame.Gameplay
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maximum = 100f;
        public float Current { get; private set; }
        public float Maximum => maximum;
        public bool IsDead => Current <= 0f;
        public event Action<float, float> Changed;
        public event Action Died;

        private void Awake()
        {
            Current = maximum;
        }

        public void Configure(float maxHealth)
        {
            maximum = Mathf.Max(1f, maxHealth);
            Current = maximum;
            Changed?.Invoke(Current, maximum);
        }

        public void Damage(float amount)
        {
            if (IsDead || amount <= 0f) return;
            Current = Mathf.Max(0f, Current - amount);
            Changed?.Invoke(Current, maximum);
            if (IsDead) Died?.Invoke();
        }
    }
}

