using System.IO;
using UnityEngine;

namespace Game.Scripts.Data
{
    public static class SaveManager
    {
        private const string SAVE_FILE_NAME = "save.json";

        private static string SavePath => Path.Combine(Application.dataPath, "Game", "Resources", SAVE_FILE_NAME);

        public static bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public static void Save(LevelSaveData saveData)
        {
            string json = JsonUtility.ToJson(saveData, true);

            Directory.CreateDirectory(Path.GetDirectoryName(SavePath) ?? string.Empty);
            File.WriteAllText(SavePath, json);
        }

        public static LevelSaveData Load()
        {
            if (!HasSave())
                return null;

            string json = File.ReadAllText(SavePath);

            return JsonUtility.FromJson<LevelSaveData>(json);
        }

        public static void Clear()
        {
            if (!HasSave())
                return;

            File.Delete(SavePath);
        }
    }
}
