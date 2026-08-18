using System;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.UI.Game
{
    public class GameCanvasController : MonoBehaviour
    {
        [Header("Panels")] 
        [SerializeField] private Panel panelInGame;
        [SerializeField] private Panel panelNextLevel;
        [SerializeField] private Panel panelGameOver;

        private void Awake()
        {
            GameManager.ActionLevelPassed += OnActionLevelPassed;
            GameManager.ActionGameOver += OnActionGameOver;
        }

        private void Start()
        {
            panelInGame.Push();
        }

        private void OnActionLevelPassed()
        {
            panelInGame.Pop();
            panelNextLevel.Push();
        }

        private void OnActionGameOver()
        {
            panelInGame.Pop();
            panelGameOver.Push();
        }
    }
}
