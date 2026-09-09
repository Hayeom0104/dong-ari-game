using UnityEngine;

namespace DongAriGame.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerController2D : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float moveSpeed = 5f;
        [SerializeField] private Vector2 arenaHalfSize = new Vector2(8.5f, 5.8f);
        private Rigidbody2D body;
        private Vector2 movement;
        public Vector2 Facing { get; private set; } = Vector2.right;

        public void SetInputEnabled(bool value)
        {
            enabled = value;
            if (!value) movement = Vector2.zero;
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
        }

        private void Update()
        {
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            if (movement.sqrMagnitude > 0.01f) Facing = movement;
        }

        private void FixedUpdate()
        {
            Vector2 next = body.position + movement * (moveSpeed * Time.fixedDeltaTime);
            next.x = Mathf.Clamp(next.x, -arenaHalfSize.x, arenaHalfSize.x);
            next.y = Mathf.Clamp(next.y, -arenaHalfSize.y, arenaHalfSize.y);
            body.MovePosition(next);
        }

        public void SetMoveSpeed(float value)
        {
            moveSpeed = Mathf.Max(0.1f, value);
        }
    }
}
