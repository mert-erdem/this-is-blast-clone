using Game.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.UI
{
    public class MenuCanvasController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textPlayButton;
    
        private void Start()
        {
            SetPlayButtonText();
        }

        public void OnButtonPlayPressed()
        {
            SceneManager.LoadScene(nameof(GameScenes.Game));
        }

        private void SetPlayButtonText()
        {
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            textPlayButton.text = "LEVEL " + $"{currentLevel}";
        }
    }
}
