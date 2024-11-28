using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OneLine
{
    public class DataManager : MonoBehaviour
    {
        public string fileName;

        public enum Size
        {
            Size3, Size4, Size5, Size6, Size7
        }

        public int GetSize(Size size)
        {
            if (size == Size.Size3) return 7 - 3;
            else if (size == Size.Size4) return 7 - 4;
            else if (size == Size.Size5) return 7 - 5;
            else if (size == Size.Size6) return 7 - 6;
            else return 0;
        }

        void GenerateLevel(GameController.LevelType type)
        {
            LevelConfig levelConfig = new LevelConfig();
            levelConfig.totalWin = 49;
            levelConfig.boxConfigs = new BoxConfig[7][];

            for (int i = 0; i < 7; i++)
            {
                levelConfig.boxConfigs[i] = new BoxConfig[7];
                for (int j = 0; j < 7; j++)
                {
                    levelConfig.boxConfigs[i][j] = new BoxConfig();
                    levelConfig.boxConfigs[i][j].isStart = false;
                    levelConfig.boxConfigs[i][j].isOk = true;
                }
            }
            string js = JsonConvert.SerializeObject(levelConfig);
            string path = Path.Combine(Application.dataPath, "HieuBon/Resources/OneLine/" + type.ToString() + "/" + 1 + ".json");
            File.WriteAllText(path, js);
        }


        public LevelConfig GetLevel(int level, GameController.LevelType type)
        {
            string folderPath = "Assets/HieuBon/Resources/OneLine/" + type.ToString();
            string[] files = Directory.GetFiles(folderPath, "*.json");
            string jsonText = File.ReadAllText(files[level - 1]);
            fileName = files[level - 1].Replace(folderPath + "\\", "").Replace(".json", "");
            //Debug.LogWarning(level);
            /*for (int i = 0; i < files.Length; i++)
            {
                Debug.LogWarning(files[i]);
            }*/
            return JsonConvert.DeserializeObject<LevelConfig>(jsonText);
        }

        public LevelDataStorage GetLevelStorage(GameController.LevelType type)
        {
            string dataJs = Path.Combine(Application.persistentDataPath, type.ToString() + ".json");
            if (File.Exists(dataJs))
            {
                string levelDataStorage = File.ReadAllText(dataJs);
                return JsonConvert.DeserializeObject<LevelDataStorage>(levelDataStorage);
            }
            else
            {
                return CreateLevelDataStorage();
            }
        }

        public LevelDataStorage CreateLevelDataStorage()
        {
            LevelDataStorage levelDataStorage = new LevelDataStorage();
            levelDataStorage.boxDataStorages = new List<BoxDataStorage>();
            return levelDataStorage;
        }

        public void SaveLevel(LevelDataStorage levelDataStorage, GameController.LevelType type)
        {
            string js = JsonConvert.SerializeObject(levelDataStorage);
            string path = Path.Combine(Application.persistentDataPath, type.ToString() + ".json");
            File.WriteAllText(path, js);
        }

        public int GetTotal(string path)
        {
            string folderPath = "Assets/HieuBon/Resources/OneLine/" + path;
            string[] files = Directory.GetFiles(folderPath, "*.json");
            return files.Length;
        }
    }

    [System.Serializable]
    public class LevelConfig
    {
        public int horiPadding;
        public int vertPadding;
        public string boxHex;
        public int totalWin;
        public BoxConfig[][] boxConfigs;
        public BoxDataStorage[] boxPassed;
    }

    [System.Serializable]
    public class BoxConfig
    {
        public bool isStart;
        public bool isOk;
    }

    [System.Serializable]
    public class LevelDataStorage
    {
        public List<BoxDataStorage> boxDataStorages;
    }

    [System.Serializable]
    public class BoxDataStorage
    {
        public int row;
        public int col;

        public BoxDataStorage()
        {

        }

        public BoxDataStorage(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
}