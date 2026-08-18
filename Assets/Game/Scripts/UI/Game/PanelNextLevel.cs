using Game.Scripts.Core;

namespace Game.Scripts.UI.Game
{
    public class PanelNextLevel : Panel
    {
        public void OnButtonNextLevelPressed()
        {
            GameManager.ActionNextLevel?.Invoke();
        }
    }
}
