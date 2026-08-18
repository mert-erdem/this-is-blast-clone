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
        private bool _levelLoaded;
        private bool _canSave;
        
        protected override void Awake()
        {
            base.Awake();
            
            _levelLoaded = GenerateLevel();
            _canSave = _levelLoaded;

            GameManager.ActionGameOver += OnActionGameOver;
            GameManager.ActionRestartLevel += OnActionRestartLevel;
            GameManager.ActionLevelPassed += OnActionLevelPassed;
            GameManager.ActionNextLevel += OnActionNextLevel;
        }

        private void Start()
        {
            // Due to script order can be problem (DI Container can be implemented later)
            if (_levelLoaded)
                GameManager.ActionGameStart?.Invoke();
        }

        private bool GenerateLevel()
        {
            if (SaveManager.HasSave())
            {
                return LoadSavedLevel();
            }

            _currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            return LoadLevel();
        }
        
        private bool LoadLevel()
        {
            TextAsset levelJson = Resources.Load<TextAsset>($"level{_currentLevel}");

            // There is no next level (back to the last level)
            if (levelJson == null && _currentLevel > 1)
            {
                _currentLevel--;
                SaveCurrentLevel();
                levelJson = Resources.Load<TextAsset>($"level{_currentLevel}");
            }

            if (levelJson == null)
            {
                Debug.LogError($"Level json not found in Resources: level{_currentLevel}");
                return false;
            }

            LevelData levelData = JsonUtility.FromJson<LevelData>(levelJson.text);

            board.Initialize(levelData.targetBlocks);
            cannonManager.Initialize(levelData.queueWidth, levelData.cannons);
            cannonSlotManager.Initialize();

            return true;
        }
        
        private bool LoadSavedLevel()
        {
            LevelSaveData saveData = SaveManager.Load();

            if (saveData == null)
            {
                return LoadLevel();
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

            return true;
        }
        
        private void Save()
        {
            if (!_levelLoaded || !_canSave)
                return;

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
        
        private void OnActionGameOver()
        {
            _canSave = false;
            SaveManager.Clear();
        }

        private void OnActionRestartLevel()
        {
            _levelLoaded = GenerateLevel();
            _canSave = _levelLoaded;

            if (_levelLoaded)
                GameManager.ActionGameStart?.Invoke();
        }

        private void OnActionLevelPassed()
        {
            _currentLevel++;
            SaveCurrentLevel();
            _canSave = false;
            SaveManager.Clear();
        }

        private void OnActionNextLevel()
        {
            _levelLoaded = GenerateLevel();
            _canSave = _levelLoaded;

            if (_levelLoaded)
                GameManager.ActionGameStart?.Invoke();
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

        private void SaveCurrentLevel()
        {
            PlayerPrefs.SetInt("CurrentLevel", _currentLevel);
            PlayerPrefs.Save();
        }

        private void OnDestroy()
        {
            GameManager.ActionGameOver -= OnActionGameOver;
            GameManager.ActionRestartLevel -= OnActionRestartLevel;
            GameManager.ActionLevelPassed -= OnActionLevelPassed;
            GameManager.ActionNextLevel -= OnActionNextLevel;
        }
    }
}
