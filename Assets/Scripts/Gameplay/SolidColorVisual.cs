using UnityEngine;

namespace DongAriGame.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SolidColorVisual : MonoBehaviour
    {
        [SerializeField] private Color color = Color.white;
        private Sprite runtimeSprite;

        private void Awake()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            // Existing scenes may keep this placeholder component after art is added.
            if (spriteRenderer.sprite != null) return;
            runtimeSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f), 1f);
            spriteRenderer.sprite = runtimeSprite;
            spriteRenderer.color = color;
        }

        public void SetColor(Color value)
        {
            color = value;
            if (TryGetComponent(out SpriteRenderer spriteRenderer)) spriteRenderer.color = color;
        }

        private void OnDestroy()
        {
            if (runtimeSprite != null) Destroy(runtimeSprite);
        }
    }
}
