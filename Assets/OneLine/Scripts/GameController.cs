using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OneLine
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;

        public Color defaultColor;
        public PlayerController playerController;
        public DataManager dataManager;
        public GridLayoutGroup gridLayoutGroup;
        public Color lineColor;
        public Color boxColor;
        public Box[] pools;
        public Box[][] boxes = new Box[7][];
        public LevelDataStorage levelDataStorage;
        public LevelConfig levelConfig;
        public LevelType type;
        public int level;

        public enum LevelType
        {
            Easy, Normal, Hard, ExtraHard
        }

        public void Awake()
        {
            instance = this;
            Generate();
        }

        public float GetTime()
        {
            if (levelConfig.boxPassed.Length < 10) return 0.1f;
            else if (levelConfig.boxPassed.Length < 20) return 0.085f;
            else if (levelConfig.boxPassed.Length < 30) return 0.065f;
            else if (levelConfig.boxPassed.Length < 40) return 0.045f;
            else return 0.025f;
        }

        public LevelType GetType(int type)
        {
            if (type == 0) return LevelType.Easy;
            else if (type == 1) return LevelType.Normal;
            else if (type == 2) return LevelType.Hard;
            else return LevelType.ExtraHard;
        }

        public void SaveLevel(int i, int j, bool isVisible)
        {
            if (isVisible) levelDataStorage.boxDataStorages.Add(new BoxDataStorage(i, j));
            else levelDataStorage.boxDataStorages.Remove(levelDataStorage.boxDataStorages[levelDataStorage.boxDataStorages.Count - 1]);
            dataManager.SaveLevel(levelDataStorage, type);
        }

        public void SaveLevel()
        {
            dataManager.SaveLevel(dataManager.CreateLevelDataStorage(), type);
        }

        public void ResartLevel()
        {
            SaveLevel();
            StartCoroutine(LoadLevel());
        }

        public void SetLevel(LevelType type, int level)
        {
            this.type = type;
            this.level = level;
        }

        public IEnumerator LoadLevel()
        {
            ResetBoxes();
            playerController.Restart();
            levelConfig = dataManager.GetLevel(level, type);
            levelDataStorage = dataManager.GetLevelStorage(type);

            int sizeHoriPadding = levelConfig.horiPadding;
            int sizeVertPadding = levelConfig.vertPadding;

            gridLayoutGroup.enabled = false;
            gridLayoutGroup.padding.left = (int)gridLayoutGroup.cellSize.x * sizeHoriPadding + (int)gridLayoutGroup.spacing.x * sizeHoriPadding;
            gridLayoutGroup.padding.top = (int)gridLayoutGroup.cellSize.y * sizeVertPadding + (int)gridLayoutGroup.spacing.y * sizeVertPadding;
            gridLayoutGroup.enabled = true;

            yield return new WaitForEndOfFrame();

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
