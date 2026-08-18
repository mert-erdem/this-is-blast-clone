using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Entities;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class LevelManager : Singleton<LevelManager>
    {
        [Header("External Classes")]
        [SerializeField] private Board board;
        [SerializeField] private CannonManager cannonManager;
        [SerializeField] private CannonSlotManager cannonSlotManager;

        private int _currentLevel = 1;
        
        protected override void Awake()
        {
            base.Awake();
            
            GenerateLevel();
        }

        private void GenerateLevel()
        {
            if (SaveManager.HasSave())
            {
                LoadSavedLevel();
                return;
            }

            _currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            LoadLevel();
        }
        
        private void LoadLevel()
        {
            TextAsset levelJson = Resources.Load<TextAsset>($"level{_currentLevel}");

            if (levelJson == null)
            {
                Debug.LogError($"Level json not found in Resources: level{_currentLevel}");
                return;
            }

            LevelData levelData = JsonUtility.FromJson<LevelData>(levelJson.text);

            board.Initialize(levelData.targetBlocks);
            cannonManager.Initialize(levelData.queueWidth, levelData.cannons);
            cannonSlotManager.Initialize();
        }
        
        private void LoadSavedLevel()
        {
            LevelSaveData saveData = SaveManager.Load();

            if (saveData == null)
            {
                LoadLevel();
                return;
            }

            _currentLevel = saveData.currentLevel;

            cannonSlotManager.Initialize();

            board.Restore(saveData.targetBlocks);
            cannonManager.Restore(saveData.queueWidth, saveData.cannonQueues);

            // Recreate slotted cannons because CannonSlotManager does not create cannons
            for (int i = 0; i < saveData.cannonSlots.Length; i++)
            {
                Cannon cannon = cannonManager.CreateSavedCannon(saveData.cannonSlots[i]);
                cannonSlotManager.SetSlotFromSave(saveData.cannonSlots[i].slotIndex, cannon);
            }
        }
        
        private void Save()
        {
            LevelSaveData saveData = new()
            {
                currentLevel = _currentLevel,
                queueWidth = cannonManager.GetQueueWidth(),
                targetBlocks = board.GetSaveData(),
                cannonQueues = cannonManager.GetSaveData(),
                cannonSlots = cannonSlotManager.GetSaveData()
            };

            SaveManager.Save(saveData);
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                Save();
        }

        private void OnApplicationQuit()
        {
            Save();
        }
    }
}
