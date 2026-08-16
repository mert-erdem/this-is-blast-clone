using Game.Scripts.Core;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class LevelGenerator : Singleton<LevelGenerator>
    {
        [Header("External Classes")]
        [SerializeField] private Board board;
        [SerializeField] private CannonManager cannonManager;
        [SerializeField] private CannonSlotManager cannonSlotManager;
        
        [SerializeField] private int currentLevel = 1; // PlayerPrefs later

        protected override void Awake()
        {
            base.Awake();
            
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

            // Global/Game DI Containers can be implemented later.
            board.Initialize(levelData.targetBlocks);
            cannonManager.Initialize(levelData.queueWidth, levelData.cannons);
            cannonSlotManager.Initialize();
        }
    }
}
