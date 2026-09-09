using DongAriGame.Core;
using UnityEngine;

namespace DongAriGame.Gameplay
{
    [RequireComponent(typeof(PlayerController2D))]
    public sealed class PlayerCombat : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float attackRange = 1.4f;
        [SerializeField, Min(0f)] private float manaCost = 3f;
        private readonly Collider2D[] hits = new Collider2D[16];
        private PlayerController2D controller;
        private ManaPool mana;
        private float attackPower = 10f;
        private float attackInterval = 0.7f;
        private float criticalChance = 0.05f;
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

        public void Configure(float damage, float attackSpeed, float critChance, float range)
        {
            attackPower = Mathf.Max(0f, damage);
            attackInterval = 0.7f / Mathf.Max(0.1f, attackSpeed);
            criticalChance = Mathf.Clamp01(critChance);
            attackRange = Mathf.Max(0.5f, range);
        }

        public void SetInputEnabled(bool value) => enabled = value;

        public void ResetMana() => mana.Fill();

        public bool TryAttack()
        {
            if (Time.time < nextAttackTime || !mana.TrySpend(manaCost)) return false;
            nextAttackTime = Time.time + attackInterval;
            Vector2 center = (Vector2)transform.position + controller.Facing * attackRange;
            int count = Physics2D.OverlapCircleNonAlloc(center, attackRange, hits);
            float damage = Random.value < criticalChance ? attackPower * 2f : attackPower;
            for (int i = 0; i < count; i++)
            {
                if (hits[i].gameObject == gameObject) continue;
                if (hits[i].TryGetComponent(out Health health) && health.Faction == CombatFaction.Enemy)
                    health.TryDamage(damage, false);
            }
            return true;
        }
    }
}
