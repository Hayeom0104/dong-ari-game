using UnityEngine;
using DongAriGame.Core;

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
        private MonsterMovement movement;
        private float movementTime;
        private Vector2 dashDirection;
        private SpriteRenderer visual;
        private Color baseTint;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            health = GetComponent<Health>();
            visual = GetComponent<SpriteRenderer>();
            baseTint = visual != null ? visual.color : Color.white;
        }

        private void OnEnable() => health.Died += HandleDeath;
        private void OnDisable() => health.Died -= HandleDeath;

        public void Configure(Transform newTarget, Health newTargetHealth, float moveSpeed, float attackDamage, float interval, MonsterMovement behavior = MonsterMovement.Chase)
        {
            target = newTarget;
            targetHealth = newTargetHealth;
            speed = Mathf.Max(0f, moveSpeed);
            damage = Mathf.Max(0f, attackDamage);
            attackInterval = Mathf.Max(0.1f, interval);
            movement = behavior;
            movementTime = 0f;
            nextAttackTime = Time.fixedTime + 0.5f;
            // Keep scaled elites and the boss able to reach across their colliders.
            attackDistance = 0.55f * (transform.localScale.x + newTarget.localScale.x) + 0.15f;
        }

        private void FixedUpdate()
        {
            if (target == null || targetHealth == null || targetHealth.IsDead || health.IsDead) return;
            movementTime += Time.fixedDeltaTime;
            Vector2 offset = (Vector2)target.position - body.position;
            if (offset.sqrMagnitude <= attackDistance * attackDistance)
            {
                if (Time.fixedTime >= nextAttackTime)
                {
                    nextAttackTime = Time.fixedTime + attackInterval;
                    targetHealth.TryDamage(damage);
                }
                if (visual != null) visual.color = baseTint;
                return;
            }
            Vector2 direction = offset.normalized;
            float multiplier = 1f;
            if (movement == MonsterMovement.Hop)
                multiplier = movementTime % 0.8f < 0.25f ? 0f : 1.45f;
            else if (movement == MonsterMovement.Dash)
            {
                float phase = movementTime % 3.5f;
                if (phase >= 1.2f && phase < 1.75f)
                {
                    dashDirection = direction;
                    if (visual != null) visual.color = Color.Lerp(baseTint, Color.white, 0.65f);
                    return;
                }
                if (visual != null) visual.color = baseTint;
                if (phase >= 1.75f && phase < 2.2f && dashDirection.sqrMagnitude > 0f)
                { direction = dashDirection; multiplier = 2.8f; }
                else if (phase >= 2.2f && phase < 2.6f) multiplier = 0f;
            }
            Vector2 next = body.position + direction * (speed * multiplier * Time.fixedDeltaTime);
            next.x = Mathf.Clamp(next.x, -9.5f, 9.5f);
            next.y = Mathf.Clamp(next.y, -6f, 6f);
            body.MovePosition(next);
        }

        private void HandleDeath()
        {
            Destroy(gameObject);
        }
    }
}

