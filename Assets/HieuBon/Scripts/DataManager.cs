using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace HieuBon
{
    public class DataManager : MonoBehaviour
    {
        private void Awake()
        {
            //GenerateLevel(GameController.LevelType.ExtraHard);
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
            string path = Path.Combine(Application.dataPath, "HieuBon/Resources/" + type.ToString() + "/" + 1 + ".json");
            File.WriteAllText(path, js);
        }


        public LevelConfig GetLevel(int level, GameController.LevelType type)
        {
            TextAsset js = Resources.Load<TextAsset>(type.ToString() + "/" + level);
            return JsonConvert.DeserializeObject<LevelConfig>(js.text);
        }

        public LevelDataStorage GetLevelStorage(int level, GameController.LevelType type)
        {
            string dataJs = Path.Combine(Application.persistentDataPath, type.ToString() + "/" + level + ".json");
            if (File.Exists(dataJs))
            {
                string levelDataStorage = File.ReadAllText(dataJs);
                return JsonConvert.DeserializeObject<LevelDataStorage>(levelDataStorage);
            }
            return new LevelDataStorage();
        }

        public void SaveLevel(LevelDataStorage levelDataStorage, int level, GameController.LevelType type)
        {
            string js = JsonConvert.SerializeObject(levelDataStorage);
            string path = Path.Combine(Application.persistentDataPath, type.ToString() + "/" + level + ".json");
            File.WriteAllText(path, js);
        }
    }

    [System.Serializable]
    public class LevelConfig
    {
        public string boxHex;
        public string lineHex;
        public int totalWin;
        public BoxConfig[][] boxConfigs;
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
        public BoxDataStorage[][] boxDataStorage;
    }

    [System.Serializable]
    public class BoxDataStorage
    {
        public bool isVisible;
    }
}