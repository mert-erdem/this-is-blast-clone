using UnityEngine.Events;

namespace Game.Scripts.Core
{
    public class GameManager
    {
        public static UnityAction
            ActionGameSceneLoaded,
            ActionGameStart,
            ActionGameOver,
            ActionLevelPassed,
            ActionNextLevel,
            ActionRestartLevel;
    }
}
