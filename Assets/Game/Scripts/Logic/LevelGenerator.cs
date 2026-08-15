using Game.Scripts.Core;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class LevelGenerator : Singleton<LevelGenerator>
    {
        [SerializeField] private int currentLevel = 1; // PlayerPrefs later

        protected override void Awake()
        {
            base.Awake();
            
            // TODO: Can be called from a GameManager to separate from MonoBehaviour
            Generate();
        }

        private void Generate()
        {
            TextAsset levelJson = Resources.Load<TextAsset>($"level{currentLevel}");

            if (levelJson == null)
            {
                Debug.LogError($"Level json not found in Resources: level{currentLevel}");
                return;
            }

            LevelData levelData = JsonUtility.FromJson<LevelData>(levelJson.text);

            Board.Instance.Initialize(levelData.targetBlocks);
            CannonManager.Instance.Initialize(levelData.queueWidth, levelData.cannons);
        }
    }
}
