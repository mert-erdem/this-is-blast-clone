using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.UI.Game
{
    public class GameCanvasController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private Panel panelNextLevel;
        [SerializeField] private Panel panelGameOver;

        private void Awake()
        {
            GameManager.ActionLevelPassed += OnActionLevelPassed;
            GameManager.ActionGameOver += OnActionGameOver;
        }

        private void OnActionLevelPassed()
        {
            panelNextLevel.Push();
        }

        private void OnActionGameOver()
        {
            panelGameOver.Push();
        }
    }
}
