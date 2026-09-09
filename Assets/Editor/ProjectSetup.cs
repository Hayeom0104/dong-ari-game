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

        [MenuItem("Dong Ari Game/Create Starter Scene")]
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

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("Dong Ari Game: starter scene created at Assets/Scenes/Main.unity");
        }

        private static GameObject CreateActor(string name, Color color, Vector2 position)
        {
            var actor = new GameObject(name);
            actor.transform.position = position;
            actor.AddComponent<SpriteRenderer>();
            var visual = actor.AddComponent<SolidColorVisual>();
            visual.SetColor(color);
            actor.AddComponent<BoxCollider2D>();
            actor.AddComponent<Rigidbody2D>();
            Health health = actor.AddComponent<Health>();
            if (name == "Player") health.Configure(100f, CombatFaction.Player);
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
