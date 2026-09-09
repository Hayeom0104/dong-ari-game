#if UNITY_EDITOR
using DongAriGame.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DongAriGame.Editor
{
    [InitializeOnLoad]
    public static class ProjectSetup
    {
        private const string ScenePath = "Assets/Scenes/Main.unity";

        static ProjectSetup()
        {
            EditorApplication.delayCall += EnsureStarterScene;
        }

        [MenuItem("Dong Ari Game/Rebuild Starter Scene")]
        public static void EnsureStarterScene()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null) return;

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            GameObject player = CreateActor("Player", Color.cyan, Vector2.zero);
            player.AddComponent<PlayerController2D>();
            player.AddComponent<PlayerCombat>();
            player.AddComponent<GameSession>();

            for (int i = 0; i < 4; i++)
            {
                GameObject enemy = CreateActor($"Enemy_{i + 1}", new Color(0.9f, 0.25f, 0.25f),
                    new Vector2(Mathf.Cos(i * Mathf.PI * 0.5f), Mathf.Sin(i * Mathf.PI * 0.5f)) * 5f);
                enemy.GetComponent<Health>().Configure(30f);
                enemy.AddComponent<EnemyChaser>().SetTarget(player.transform);
            }

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("Dong Ari Game: starter scene created at Assets/Scenes/Main.unity");
        }

        private static GameObject CreateActor(string name, Color color, Vector2 position)
        {
            var actor = new GameObject(name);
            actor.transform.position = position;
            var renderer = actor.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            renderer.color = color;
            actor.AddComponent<BoxCollider2D>();
            actor.AddComponent<Rigidbody2D>();
            actor.AddComponent<Health>();
            return actor;
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 7f;
            camera.backgroundColor = new Color(0.06f, 0.07f, 0.1f);
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        }
    }
}
#endif

