using DongAriGame.Core;
using UnityEngine;

namespace DongAriGame.Gameplay
{
    [RequireComponent(typeof(PlayerController2D))]
    public sealed class PlayerCombat : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float attackRange = 1.4f;
        [SerializeField, Min(0f)] private float manaCost = 3f;
        [SerializeField] private LayerMask targetMask = ~0;
        private readonly Collider2D[] hits = new Collider2D[16];
        private PlayerController2D controller;
        private ManaPool mana;
        private float attackPower = 10f;
        private float nextAttackTime;

        public ManaPool Mana => mana;

        private void Awake()
        {
            controller = GetComponent<PlayerController2D>();
            mana = new ManaPool(10f, 1f, 10f);
        }

        private void Update()
        {
            mana.Tick(Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Space)) TryAttack();
        }

        public void Configure(float damage)
        {
            attackPower = Mathf.Max(0f, damage);
        }

        public bool TryAttack()
        {
            if (Time.time < nextAttackTime || !mana.TrySpend(manaCost)) return false;
            nextAttackTime = Time.time + 0.35f;
            Vector2 center = (Vector2)transform.position + controller.Facing * attackRange;
            int count = Physics2D.OverlapCircleNonAlloc(center, attackRange, hits, targetMask);
            for (int i = 0; i < count; i++)
            {
                if (hits[i].gameObject == gameObject) continue;
                if (hits[i].TryGetComponent(out Health health)) health.Damage(attackPower);
            }
            return true;
        }
    }
}

