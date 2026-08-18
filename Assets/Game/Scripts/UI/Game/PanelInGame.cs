using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Game
{
    public class PanelInGame : Panel
    {
        [SerializeField] private TextMeshProUGUI textLevel;
        
        private void OnEnable()
        {
            SetLevelText();
        }

        private void SetLevelText()
        {
            textLevel.text = "LEVEL " + PlayerPrefs.GetInt("CurrentLevel", 1);
        }
    }
}
