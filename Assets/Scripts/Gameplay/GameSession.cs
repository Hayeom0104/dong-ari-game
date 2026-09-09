using DongAriGame.Core;
using UnityEngine;

namespace DongAriGame.Gameplay
{
    public sealed class GameSession : MonoBehaviour
    {
        [SerializeField] private CharacterClass selectedClass = CharacterClass.Warrior;
        private readonly AffinityProgress affinities = new AffinityProgress();
        private readonly RunProgress run = new RunProgress();

        public CharacterClass SelectedClass => selectedClass;
        public AffinityProgress Affinities => affinities;
        public RunProgress Run => run;

        public StatBlock BuildCurrentStats()
        {
            StatBlock stats = selectedClass switch
            {
                CharacterClass.Warrior => new StatBlock { MaxHealth = 130f, AttackPower = 14f, MoveSpeed = 4.5f },
                CharacterClass.Archer => new StatBlock { MaxHealth = 90f, AttackPower = 11f, MoveSpeed = 5.8f, AttackSpeed = 1.2f },
                CharacterClass.Mage => new StatBlock { MaxHealth = 80f, AttackPower = 18f, MoveSpeed = 5f },
                _ => new StatBlock()
            };
            return affinities.ApplyTo(stats);
        }
    }
}

