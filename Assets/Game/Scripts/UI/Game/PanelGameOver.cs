using Game.Scripts.Core;

namespace Game.Scripts.UI.Game
{
    public class PanelGameOver : Panel
    {
        public void OnButtonRetryPressed()
        {
            GameManager.ActionRestartLevel?.Invoke();
        }
    }
}
