using System.Collections.Generic;
using DongAriGame.Core;
using UnityEngine;

namespace DongAriGame.Gameplay
{
    [RequireComponent(typeof(Health), typeof(PlayerController2D), typeof(PlayerCombat))]
    public sealed class GameSession : MonoBehaviour
    {
        private readonly AffinityProgress affinities = new AffinityProgress();
        private readonly RunProgress run = new RunProgress();
        private readonly HashSet<Health> livingEnemies = new HashSet<Health>();
        private readonly Dictionary<Health, int> enemyScores = new Dictionary<Health, int>();
        private CharacterClass selectedClass;
        private GamePhase phase = GamePhase.CharacterSelect;
        private PlayerController2D playerController;
        private PlayerCombat playerCombat;
        private Health playerHealth;
        private float elapsedTime;
        private readonly AffinityType[] artifactChoices = new AffinityType[3];
        private int score;
        private int currentWave;
        private int wavesInRoom;

        public GamePhase Phase => phase;
        public int CurrentRoom => run.CurrentRoom;
        public float ElapsedTime => elapsedTime;
        public int Score => score;

        private void Awake()
        {
            playerController = GetComponent<PlayerController2D>();
            playerCombat = GetComponent<PlayerCombat>();
            playerHealth = GetComponent<Health>();
            playerController.SetInputEnabled(false);
            playerCombat.SetInputEnabled(false);
        }

        private void OnEnable() => playerHealth.Died += HandlePlayerDied;

        private void OnDisable()
        {
            playerHealth.Died -= HandlePlayerDied;
            UnsubscribeEnemies();
        }

        private void Update()
        {
            if (phase != GamePhase.Playing) return;
            elapsedTime += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Q)) TryUseCharacterSkill();
        }

        private void StartRun(CharacterClass characterClass)
        {
            ClearEnemies();
            selectedClass = characterClass;
            run.Reset();
            affinities.Reset();
            elapsedTime = 0f;
            score = 0;
            transform.position = Vector3.zero;
            ApplyStats(true);
            playerCombat.ResetMana();
            phase = GamePhase.Playing;
            playerController.SetInputEnabled(true);
            playerCombat.SetInputEnabled(true);
            BeginRoom();
        }

        private StatBlock BuildCurrentStats()
        {
            StatBlock stats = selectedClass switch
            {
                CharacterClass.Warrior => new StatBlock { MaxHealth = 150f, AttackPower = 10f, MoveSpeed = 4.5f, AttackSpeed = 0.9f },
                CharacterClass.Archer => new StatBlock { MaxHealth = 100f, AttackPower = 10f, MoveSpeed = 5.8f, AttackSpeed = 1.35f, CriticalChance = 0.1f },
                CharacterClass.Mage => new StatBlock { MaxHealth = 70f, AttackPower = 10f, MoveSpeed = 5f, AttackSpeed = 0.8f },
                _ => new StatBlock()
            };
            return affinities.ApplyTo(stats);
        }

        private void ApplyStats(bool restoreHealth)
        {
            StatBlock stats = BuildCurrentStats();
            if (restoreHealth)
                playerHealth.Configure(stats.MaxHealth, CombatFaction.Player, stats.EvasionChance);
            else
            {
                playerHealth.SetMaximum(stats.MaxHealth);
                playerHealth.Heal(stats.MaxHealth * 0.18f);
            }

            playerController.SetMoveSpeed(stats.MoveSpeed);
            playerHealth.SetEvasion(stats.EvasionChance);
            playerCombat.ConfigureMana(GetMaximumMana());
            float range = selectedClass == CharacterClass.Archer ? 2.8f : selectedClass == CharacterClass.Mage ? 2.2f : 1.35f;
            playerCombat.Configure(stats.AttackPower, stats.AttackSpeed, stats.CriticalChance, range);
        }

        private void BeginRoom()
        {
            currentWave = 1;
            wavesInRoom = run.CurrentRoom == RunProgress.TotalRooms ? 1 : 1 + (run.CurrentRoom - 1) / 4;
            SpawnWave();
        }

        private void SpawnWave()
        {
            ClearEnemies();
            bool bossRoom = run.CurrentRoom == RunProgress.TotalRooms;
            int enemyCount = bossRoom ? 1 : Mathf.Min(2 + run.CurrentRoom / 2 + currentWave, 6);
            for (int i = 0; i < enemyCount; i++)
            {
                float angle = Mathf.PI * 2f * i / enemyCount;
                Vector2 position = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 5.3f;
                bool elite = !bossRoom && currentWave == wavesInRoom && i == enemyCount - 1;
                Health enemy = CreateEnemy(position, bossRoom, elite);
                livingEnemies.Add(enemy);
                enemy.Died += HandleEnemyDied;
            }
        }

        private Health CreateEnemy(Vector2 position, bool boss, bool elite)
        {
            var enemy = new GameObject(boss ? "Room Boss" : elite ? "Elite Enemy" : "Enemy");
            enemy.transform.position = position;
            enemy.transform.localScale = boss ? Vector3.one * 1.8f : elite ? Vector3.one * 1.3f : Vector3.one;
            enemy.AddComponent<SpriteRenderer>();
            var visual = enemy.AddComponent<SolidColorVisual>();
            visual.SetColor(boss ? new Color(0.75f, 0.2f, 0.95f) : elite ? new Color(1f, 0.65f, 0.12f) : new Color(0.92f, 0.22f, 0.2f));
            enemy.AddComponent<BoxCollider2D>();
            var rigidbody = enemy.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0f;
            rigidbody.freezeRotation = true;
            Health enemyHealth = enemy.AddComponent<Health>();
            float hp = boss ? 200f + run.CurrentRoom * 50f : 30f + run.CurrentRoom * 10f;
            if (elite) hp *= 2f;
            enemyHealth.Configure(hp, CombatFaction.Enemy);
            var reward = enemy.AddComponent<EnemyReward>();
            reward.Configure(boss ? 1000 : elite ? 250 : 100, elite);
            enemyScores[enemyHealth] = reward.ScoreValue;
            var chaser = enemy.AddComponent<EnemyChaser>();
            float damage = boss ? 20f + run.CurrentRoom * 5f : 5f + run.CurrentRoom * 2f;
            chaser.Configure(transform, playerHealth, boss ? 1.4f : elite ? 2.2f : 1.8f,
                elite ? damage * 1.5f : damage, boss ? 1.1f : 1.35f);
            return enemyHealth;
        }

        private void HandleEnemyDied()
        {
            foreach (Health enemy in livingEnemies)
                if (enemy != null && enemy.IsDead && enemyScores.Remove(enemy, out int earned)) score += earned;
            livingEnemies.RemoveWhere(enemy => enemy == null || enemy.IsDead);
            if (livingEnemies.Count > 0 || phase != GamePhase.Playing) return;

            if (currentWave < wavesInRoom)
            {
                currentWave++;
                SpawnWave();
                return;
            }

            if (run.CurrentRoom == RunProgress.TotalRooms)
            {
                run.CompleteCurrentRoom();
                EndRun(true);
                return;
            }

            artifactChoices[0] = AffinityType.Red;
            artifactChoices[1] = AffinityType.Blue;
            artifactChoices[2] = AffinityType.White;
            phase = GamePhase.ArtifactSelect;
            playerController.SetInputEnabled(false);
            playerCombat.SetInputEnabled(false);
        }

        private void SelectArtifact(AffinityType type)
        {
            if (phase != GamePhase.ArtifactSelect) return;
            affinities.Add(type);
            run.CompleteCurrentRoom();
            ApplyStats(false);
            phase = GamePhase.Playing;
            playerController.SetInputEnabled(true);
            playerCombat.SetInputEnabled(true);
            BeginRoom();
        }

        private void HandlePlayerDied() => EndRun(false);

        private void EndRun(bool victory)
        {
            phase = victory ? GamePhase.Victory : GamePhase.Defeat;
            playerController.SetInputEnabled(false);
            playerCombat.SetInputEnabled(false);
            ClearEnemies();
        }

        private void ClearEnemies()
        {
            foreach (Health enemy in livingEnemies)
            {
                if (enemy == null) continue;
                enemy.Died -= HandleEnemyDied;
                Destroy(enemy.gameObject);
            }
            livingEnemies.Clear();
            enemyScores.Clear();
        }

        private void UnsubscribeEnemies()
        {
            foreach (Health enemy in livingEnemies)
                if (enemy != null) enemy.Died -= HandleEnemyDied;
        }

        private void OnGUI()
        {
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            Matrix4x4 previous = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * Mathf.Max(0.65f, scale));
            if (phase == GamePhase.CharacterSelect) DrawCharacterSelect();
            else if (phase == GamePhase.Playing) DrawHud();
            else if (phase == GamePhase.ArtifactSelect) DrawArtifactSelect();
            else DrawResult();
            GUI.matrix = previous;
        }

        private void DrawHud()
        {
            GUI.Box(new Rect(20, 18, 350, 118), string.Empty);
            GUI.Label(new Rect(36, 28, 320, 28), $"방 {run.CurrentRoom} / {RunProgress.TotalRooms}   웨이브 {currentWave}/{wavesInRoom}");
            GUI.Label(new Rect(36, 59, 320, 25), $"체력 {playerHealth.Current:0} / {playerHealth.Maximum:0}");
            GUI.Label(new Rect(36, 88, 320, 25), $"마나 {playerCombat.Mana.Current:0.0} / {playerCombat.Mana.Maximum:0} (+1/초)");
            GUI.Label(new Rect(390, 18, 320, 28), $"경과 {FormatTime(elapsedTime)}   점수 {score:N0}");
            GUI.Label(new Rect(20, 665, 650, 30), $"이동: WASD/방향키    기본 공격: Space    고유 스킬: Q (마나 20)");
        }

        private void DrawCharacterSelect()
        {
            GUI.Box(new Rect(390, 130, 500, 410), "캐릭터 선택");
            GUI.Label(new Rect(455, 180, 380, 40), "자신의 속도로 10개의 방을 돌파하세요.");
            if (GUI.Button(new Rect(465, 245, 350, 65), "전사\n높은 체력 · 근접 공격")) StartRun(CharacterClass.Warrior);
            if (GUI.Button(new Rect(465, 330, 350, 65), "궁수\n빠른 이동과 공격 · 긴 사거리")) StartRun(CharacterClass.Archer);
            if (GUI.Button(new Rect(465, 415, 350, 65), "마법사\n강한 공격 · 중거리")) StartRun(CharacterClass.Mage);
        }

        private void DrawArtifactSelect()
        {
            GUI.Box(new Rect(365, 160, 550, 360), $"{run.CurrentRoom}번 방 클리어!");
            GUI.Label(new Rect(470, 215, 360, 35), "아티팩트를 하나 선택하세요.");
            if (GUI.Button(new Rect(420, 270, 440, 60), ArtifactText(artifactChoices[0]))) SelectArtifact(artifactChoices[0]);
            if (GUI.Button(new Rect(420, 345, 440, 60), ArtifactText(artifactChoices[1]))) SelectArtifact(artifactChoices[1]);
            if (GUI.Button(new Rect(420, 420, 440, 60), ArtifactText(artifactChoices[2]))) SelectArtifact(artifactChoices[2]);
        }

        private void DrawResult()
        {
            string title = phase == GamePhase.Victory ? "던전 정복 성공!" : "도전 종료";
            string reason = phase == GamePhase.Victory ? "10개의 방을 모두 돌파했습니다." : "체력이 모두 소진되었습니다.";
            GUI.Box(new Rect(390, 190, 500, 320), title);
            GUI.Label(new Rect(470, 270, 350, 60), $"{reason}\n기록: {FormatTime(elapsedTime)}\n점수: {score:N0}");
            if (GUI.Button(new Rect(490, 380, 300, 65), "캐릭터 선택으로")) phase = GamePhase.CharacterSelect;
        }

        private static string ArtifactText(AffinityType type)
        {
            return type switch
            {
                AffinityType.Red => "붉은 핵\n공격력 +8% · 치명타 +2.5%",
                AffinityType.Blue => "푸른 깃털\n공격속도 +6% · 이동속도 +4% · 회피 +2%",
                _ => "백색 결정\n공격력 +2 · 최대 체력 +8"
            };
        }

        private static string FormatTime(float seconds)
        {
            int total = Mathf.FloorToInt(seconds);
            return $"{total / 60:00}:{total % 60:00}";
        }

        private float GetMaximumMana()
        {
            return selectedClass switch
            {
                CharacterClass.Warrior => 50f,
                CharacterClass.Archer => 80f,
                CharacterClass.Mage => 120f,
                _ => 50f
            };
        }

        private void TryUseCharacterSkill()
        {
            float range = selectedClass switch
            {
                CharacterClass.Warrior => 2.6f,
                CharacterClass.Archer => 4.2f,
                CharacterClass.Mage => 5.2f,
                _ => 2f
            };
            float multiplier = selectedClass switch
            {
                CharacterClass.Warrior => 2.2f,
                CharacterClass.Archer => 1.6f,
                CharacterClass.Mage => 2.8f,
                _ => 1f
            };
            playerCombat.TryUseSkill(20f, multiplier, range);
        }
    }
}
