using System;
using System.Collections.Generic;
using UnityEngine;

namespace DongAriGame.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteWalkAnimator : MonoBehaviour
    {
        [Serializable] private sealed class Atlas { public Clip[] clips; }
        [Serializable] private sealed class Clip { public Frame[] frames; public float pixelsPerUnit; }
        [Serializable] private sealed class Frame
        {
            public int x, y, width, height;
            public float pivotX, pivotY;
        }
        private static readonly Dictionary<string, Sprite[]> Cache = new Dictionary<string, Sprite[]>();
        private SpriteRenderer visual;
        private Sprite[] frames;
        private Vector3 previousPosition;
        private float frameTime;
        private int frame;
        private float lastMovedTime;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetCache()
        {
            foreach (Sprite[] clip in Cache.Values)
                foreach (Sprite sprite in clip)
                    if (sprite != null) Destroy(sprite);
            Cache.Clear();
        }

        private void Awake() => visual = GetComponent<SpriteRenderer>();

        public void Configure(string atlasName, int row, Color tint)
        {
            if (visual == null) visual = GetComponent<SpriteRenderer>();
            string key = atlasName + ":" + row;
            if (!Cache.TryGetValue(key, out frames))
            {
                Texture2D texture = Resources.Load<Texture2D>("Art/" + atlasName);
                TextAsset data = Resources.Load<TextAsset>("Art/" + atlasName + "Frames");
                if (texture == null || data == null)
                {
                    Debug.LogError($"Missing animation atlas: {atlasName}", this);
                    enabled = false;
                    return;
                }
                Atlas atlas = JsonUtility.FromJson<Atlas>(data.text);
                if (atlas?.clips == null || row < 0 || row >= atlas.clips.Length)
                    throw new ArgumentOutOfRangeException(nameof(row));
                Clip clip = atlas.clips[row];
                frames = new Sprite[clip.frames.Length];
                for (int i = 0; i < frames.Length; i++)
                {
                    Frame f = clip.frames[i];
                    frames[i] = Sprite.Create(texture, new Rect(f.x, f.y, f.width, f.height),
                        new Vector2(f.pivotX, f.pivotY), clip.pixelsPerUnit, 0, SpriteMeshType.FullRect);
                    frames[i].name = key + ":" + i;
                }
                Cache.Add(key, frames);
            }
            enabled = true;
            visual.color = tint;
            visual.flipX = false;
            frame = 0;
            frameTime = 0f;
            visual.sprite = frames[0];
            previousPosition = transform.position;
            lastMovedTime = float.NegativeInfinity;
        }

        private void LateUpdate()
        {
            if (frames == null || frames.Length == 0) return;
            Vector3 delta = transform.position - previousPosition;
            previousPosition = transform.position;
            if (Mathf.Abs(delta.x) > 0.0001f) visual.flipX = delta.x < 0f;
            if (delta.sqrMagnitude > 0.000001f && delta.sqrMagnitude < 4f) lastMovedTime = Time.time;
            if (Time.time - lastMovedTime < 0.08f)
            {
                frameTime += Time.deltaTime * 10f;
                int steps = Mathf.FloorToInt(frameTime);
                frameTime -= steps;
                frame = (frame + steps) % frames.Length;
            }
            else { frame = 0; frameTime = 0f; }
            visual.sprite = frames[frame];
            visual.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
        }
    }
}
