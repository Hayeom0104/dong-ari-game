using UnityEngine;

namespace DongAriGame.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public sealed class EnemyChaser : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float speed = 2f;
        private Rigidbody2D body;
        private Transform target;
        private Health health;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            health = GetComponent<Health>();
        }

        private void OnEnable() => health.Died += HandleDeath;
        private void OnDisable() => health.Died -= HandleDeath;

        public void SetTarget(Transform value) => target = value;

        private void FixedUpdate()
        {
            if (target == null || health.IsDead) return;
            Vector2 direction = ((Vector2)target.position - body.position).normalized;
            body.MovePosition(body.position + direction * (speed * Time.fixedDeltaTime));
        }

        private void HandleDeath()
        {
            Destroy(gameObject);
        }
    }
}

