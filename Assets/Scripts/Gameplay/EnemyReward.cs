using UnityEngine;

namespace DongAriGame.Gameplay
{
    public sealed class EnemyReward : MonoBehaviour
    {
        public int ScoreValue { get; private set; }
        public bool IsElite { get; private set; }

        public void Configure(int scoreValue, bool elite)
        {
            ScoreValue = Mathf.Max(0, scoreValue);
            IsElite = elite;
        }
    }
}
