using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.UI.Game
{
    public class GameCanvasController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private Panel panelNextLevel;

        private void Awake()
        {
            GameManager.ActionLevelPassed += OnActionLevelPassed;
        }

        private void OnActionLevelPassed()
        {
            panelNextLevel.Push();
        }
    }
}
