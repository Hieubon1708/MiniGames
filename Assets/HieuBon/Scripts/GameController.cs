using UnityEngine;
using UnityEngine.UIElements;

namespace HieuBon
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;

        public Color defaultColor;
        public PlayerController playerController;
        public DataManager dataManager;
        public Color lineColor;
        public Color boxColor;
        public Box[] pools;
        public Box[][] boxes = new Box[7][];
        public LevelDataStorage levelDataStorage;
        public LevelConfig levelConfig;

        public enum LevelType
        {
            Easy, Normal, Hard, ExtraHard
        }

        public void Awake()
        {
            instance = this;
            Generate();
        }

        private void Start()
        {
            LoadLevel();
        }

        public void SaveLevel(int i, int j, bool isVisible)
        {
            if (isVisible) levelDataStorage.boxDataStorages.Add(new BoxDataStorage(i, j));
            else levelDataStorage.boxDataStorages.Remove(levelDataStorage.boxDataStorages[levelDataStorage.boxDataStorages.Count - 1]);
            dataManager.SaveLevel(levelDataStorage, levelConfig.type);
        }

        public void SaveLevel()
        {
            dataManager.SaveLevel(dataManager.CreateLevelDataStorage(), levelConfig.type);
        }

        public void ResartLevel()
        {
            SaveLevel();
            LoadLevel();
        }

        public void LoadLevel()
        {
            ResetBoxes();
            playerController.Restart();

            levelConfig = dataManager.GetLevel(1, LevelType.ExtraHard);
            levelDataStorage = dataManager.GetLevelStorage(LevelType.ExtraHard);

            Color color;
            if (ColorConvert(levelConfig.boxHex, out color))
            {
                boxColor = color;
            }
            else
            {
                Debug.LogError(levelConfig.boxHex);
            }

            lineColor = new Color(boxColor.r - 50f / 255f, boxColor.g - 50f / 255f, boxColor.b - 50f / 255f);
            playerController.SetColorLine(lineColor);
            UIController.instance.TouchSetColor(lineColor);

            for (int i = 0; i < boxes.Length; i++)
            {
                for (int j = 0; j < boxes[i].Length; j++)
                {
                    boxes[i][j].IsOk(levelConfig.boxConfigs[i][j].isOk);
                    boxes[i][j].IsStart(levelConfig.boxConfigs[i][j].isStart);
                }
            }
            for (int k = 0; k < levelDataStorage.boxDataStorages.Count; k++)
            {
                int i = levelDataStorage.boxDataStorages[k].row;
                int j = levelDataStorage.boxDataStorages[k].col;

                boxes[i][j].LoadData();
            }
        }

        void Generate()
        {
            int count = 0;
            for (int i = 0; i < boxes.Length; i++)
            {
                Box[] boxChild = new Box[boxes.Length];
                for (int j = 0; j < boxChild.Length; j++)
                {
                    boxChild[j] = pools[count++];
                    boxChild[j].row = i;
                    boxChild[j].col = j;
                    boxChild[j].name = "row " + i + " col " + j;
                }
                boxes[i] = boxChild;
            }
        }

        public bool ColorConvert(string hex, out Color color)
        {
            if (ColorUtility.TryParseHtmlString(hex, out color)) return true;
            return false;
        }

        public void ResetBoxes()
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                for (int j = 0; j < boxes[i].Length; j++)
                {
                    boxes[i][j].ResetBox();
                }
            }
        }

        public Box GetBox(GameObject box)
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                for (int j = 0; j < boxes[i].Length; j++)
                {
                    if (boxes[i][j].gameObject == box) return boxes[i][j];

                }
            }
            return null;
        }
    }
}
