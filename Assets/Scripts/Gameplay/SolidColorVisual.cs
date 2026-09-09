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
            runtimeSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f), 1f);
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
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
