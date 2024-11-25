using UnityEngine;

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
            LoadLevel(1);
        }

        public void LoadLevel(int level)
        {
            ResetBoxes();

            levelConfig = dataManager.GetLevel(1, LevelType.ExtraHard);
            levelDataStorage = dataManager.GetLevelStorage(1, LevelType.ExtraHard);

            Color color;
            if (ColorConvert(levelConfig.boxHex, out color))
            {
                boxColor = color;
            }
            else
            {
                Debug.LogError(levelConfig.boxHex);
            }
            if (ColorConvert(levelConfig.lineHex, out color))
            {
                lineColor = color;
                playerController.SetColorLine(lineColor);
            }
            else
            {
                Debug.LogError(levelConfig.lineHex);
            }

            for (int i = 0; i < boxes.Length; i++)
            {
                for (int j = 0; j < boxes[i].Length; j++)
                {
                    boxes[i][j].IsOk(levelConfig.boxConfigs[i][j].isOk);
                    boxes[i][j].IsStart(levelConfig.boxConfigs[i][j].isStart);
                }
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
                }
                boxes[i] = boxChild;
            }
        }

        public bool ColorConvert(string hex, out Color color)
        {
            if (ColorUtility.TryParseHtmlString(hex, out color)) return true;
            return false;
        }

        void ResetBoxes()
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
