using Game.Scripts.Core;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class LevelManager : Singleton<LevelManager>
    {
        [Header("External Classes")]
        [SerializeField] private Board board;
        [SerializeField] private CannonManager cannonManager;
        [SerializeField] private CannonSlotManager cannonSlotManager;
        
        [SerializeField] private int currentLevel = 1; // PlayerPrefs later

        protected override void Awake()
        {
            base.Awake();
            
            TextAsset saveJson = Resources.Load<TextAsset>($"save");

            if (saveJson == null || string.IsNullOrWhiteSpace(saveJson.text))
            {
                Generate();
                
                return;
            }
            
            GenerateFromSave();
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

            // Global/Game DI Containers can be implemented later.
            board.Initialize(levelData.targetBlocks);
            cannonManager.Initialize(levelData.queueWidth, levelData.cannons);
            cannonSlotManager.Initialize();
        }

        private void GenerateFromSave()
        {
        }
    }
}
