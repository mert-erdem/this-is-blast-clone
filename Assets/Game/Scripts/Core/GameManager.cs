using UnityEngine.Events;

namespace Game.Scripts.Core
{
    public class GameManager
    {
        public static UnityAction
            ActionGameStart,
            ActionGameOver,
            ActionLevelPassed,
            ActionNextLevel,
            ActionRestartLevel;
    }
}
