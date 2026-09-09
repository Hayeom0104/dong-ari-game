using UnityEngine;

namespace DongAriGame.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public sealed class EnemyChaser : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float speed = 2f;
        [SerializeField, Min(0.1f)] private float attackDistance = 1.1f;
        private Rigidbody2D body;
        private Transform target;
        private Health targetHealth;
        private Health health;
        private float damage = 7f;
        private float attackInterval = 1.2f;
        private float nextAttackTime;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            health = GetComponent<Health>();
        }

        private void OnEnable() => health.Died += HandleDeath;
        private void OnDisable() => health.Died -= HandleDeath;

        public void Configure(Transform newTarget, Health newTargetHealth, float moveSpeed, float attackDamage, float interval)
        {
            target = newTarget;
            targetHealth = newTargetHealth;
            speed = Mathf.Max(0f, moveSpeed);
            damage = Mathf.Max(0f, attackDamage);
            attackInterval = Mathf.Max(0.1f, interval);
        }

        private void FixedUpdate()
        {
            if (target == null || health.IsDead) return;
            Vector2 offset = (Vector2)target.position - body.position;
            if (offset.sqrMagnitude <= attackDistance * attackDistance)
            {
                if (Time.fixedTime >= nextAttackTime)
                {
                    nextAttackTime = Time.fixedTime + attackInterval;
                    targetHealth?.TryDamage(damage);
                }
                return;
            }
            Vector2 direction = offset.normalized;
            body.MovePosition(body.position + direction * (speed * Time.fixedDeltaTime));
        }

        private void HandleDeath()
        {
            Destroy(gameObject);
        }
    }
}
