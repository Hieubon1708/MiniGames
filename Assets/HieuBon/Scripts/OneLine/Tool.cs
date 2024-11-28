using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OneLine
{
    public class Tool : MonoBehaviour
    {
        public Box[][] boxes;
        public List<Direction> dir = new List<Direction>();
        public List<DirectionTaken> dirTaken = new List<DirectionTaken>();
        public LineRenderer line;
        public Color[] color;
        public Box box;
        public List<Direction> dirTakenChild = new List<Direction>();
        public List<Box> boxPassed = new List<Box>();
        public int limit;
        public int totalWin;
        public bool isNotFound;
        public BoxDataStorage boxStart;
        public int horiPadding;
        public int vertPadding;

        public enum Direction
        {
            None, Left, Right, Top, Bottom
        }

        void MapGenerate()
        {
            isNotFound = false;
            totalWin = 0;
            dir.Clear();
            dirTaken.Clear();
            dirTakenChild.Clear();
            boxPassed.Clear();
            boxes = GameController.instance.boxes;

            GameController.instance.boxColor = color[Random.Range(0, color.Length)];
            GameController.instance.lineColor = new Color(GameController.instance.boxColor.r - 50f / 255f, GameController.instance.boxColor.g - 50f / 255f, GameController.instance.boxColor.b - 50f / 255f);
            GameController.instance.playerController.SetColorLine(GameController.instance.lineColor);

            for (int k = 0; k < boxes.Length; k++)
            {
                for (int j = 0; j < boxes[k].Length; j++)
                {
                    boxes[k][j].canvasGroup.alpha = 0.05f;
                    boxes[k][j].iconStart.gameObject.SetActive(false);
                    boxes[k][j].isVisible = false;
                    boxes[k][j].isOK = false;
                    boxes[k][j].image.color = GameController.instance.defaultColor;
                }
            }
            List<BoxDataStorage> list = new List<BoxDataStorage>();
            int min = 0, max = 3;
            for (int i = min; i <= max; i++)
            {
                for (int j = min; j <= max; j++)
                {
                    list.Add(new BoxDataStorage(i, j));
                }
            }
            int count = Random.Range(4, 6);
            while (count > 0)
            {
                int random = Random.Range(0, list.Count);
                list.RemoveAt(random);
                count--;
            }
            int randomStart = Random.Range(0, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                int row = list[i].row;
                int col = list[i].col;
                boxes[row][col].canvasGroup.alpha = 1;
                boxes[row][col].isOK = true;
                if (i == randomStart)
                {
                    boxStart = new BoxDataStorage(row, col);
                    boxes[row][col].IsStart(true);
                }
                totalWin++;
            }
            Restart();
            DrawLine();
        }

        int GetTotal(string path)
        {
            string folderPath = "Assets/HieuBon/Resources/OneLine/" + path;
            string[] files = Directory.GetFiles(folderPath, "*.json");
            return files.Length;
        }

        void LevelModifi()
        {
            string folderPath = "Assets/HieuBon/Resources/OneLine/" + GameController.LevelType.ExtraHard.ToString();
            string[] files = Directory.GetFiles(folderPath, "*.json");
            for (int k = 0; k < files.Length; k++)
            {
                string jsonText = File.ReadAllText(files[k]);
                LevelConfig levelConfig = JsonConvert.DeserializeObject<LevelConfig>(jsonText);

                int maxRow = int.MinValue;
                int minRow = int.MaxValue;
                int maxCol = int.MinValue;
                int minCol = int.MaxValue;

                for (int i = 0; i < levelConfig.boxConfigs.Length; i++)
                {
                    for (int j = 0; j < levelConfig.boxConfigs[i].Length; j++)
                    {
                        if (levelConfig.boxConfigs[i][j].isOk)
                        {
                            if (i < minRow)
                            {
                                minRow = i;
                            }
                            if (i > maxRow)
                            {
                                maxRow = i;
                            }
                            if (j < minCol)
                            {
                                minCol = j;
                            }
                            if (j > maxCol)
                            {
                                maxCol = j;
                            }
                        }
                    }
                }

                string folder = "";
                int sizeX = maxCol - minCol;
                if (sizeX == 1) folder = "2x2";
                else if (sizeX == 2) folder = "3x3";
                else if (sizeX == 3) folder = "4x4";
                else if (sizeX == 4) folder = "5x5";
                else if (sizeX == 5) folder = "6x6";
                else folder = "7x7";

                levelConfig.horiPadding = levelConfig.boxConfigs.Length - 1 - maxCol - minCol;
                levelConfig.vertPadding = levelConfig.boxConfigs.Length - 1 - maxRow - minRow;

                string pathModifi = files[k].Replace("\\", "/").Replace("Assets/", "");
                Debug.LogWarning(pathModifi);

                string js = JsonConvert.SerializeObject(levelConfig);
                string path = Path.Combine(Application.dataPath, pathModifi);
                File.WriteAllText(path, js);

                /*Debug.Log("Hori Padding = " + levelConfig.horiPadding);
                Debug.Log("Vert Padding = " + levelConfig.vertPadding);
                Debug.Log("Min Row = " + minRow);
                Debug.Log("Min Col = " + minCol);
                Debug.Log("Folder = " + folder);
                Debug.Log("Fix Complete !!!");*/
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //DrawLine();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                //MapGenerate();
                //StartCoroutine(AutomaticPathFinding());
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                //MapSave();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                //LevelModifi();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.LogWarning("Size 2x2 = " + GetTotal("2x2"));
                Debug.LogWarning("Size 3x3 = " + GetTotal("3x3"));
                Debug.LogWarning("Size 4x4 = " + GetTotal("4x4"));
                Debug.LogWarning("Size 5x5 = " + GetTotal("5x5"));
                Debug.LogWarning("Size 6x6 = " + GetTotal("6x6"));
                Debug.LogWarning("Size 7x7 = " + GetTotal("7x7"));
            }
        }

        public IEnumerator AutomaticPathFinding()
        {
            isNotFound = false;
            totalWin = 0;
            dir.Clear();
            dirTaken.Clear();
            dirTakenChild.Clear();
            boxPassed.Clear();
            boxes = GameController.instance.boxes;

            GameController.instance.boxColor = color[Random.Range(0, color.Length)];
            GameController.instance.lineColor = new Color(GameController.instance.boxColor.r - 50f / 255f, GameController.instance.boxColor.g - 50f / 255f, GameController.instance.boxColor.b - 50f / 255f);
            GameController.instance.playerController.SetColorLine(GameController.instance.lineColor);

            for (int k = 0; k < boxes.Length; k++)
            {
                for (int j = 0; j < boxes[k].Length; j++)
                {
                    boxes[k][j].canvasGroup.alpha = 0.05f;
                    boxes[k][j].iconStart.gameObject.SetActive(false);
                    boxes[k][j].isVisible = false;
                    boxes[k][j].isOK = false;
                    boxes[k][j].image.color = GameController.instance.defaultColor;
                }
            }

            int rowStartRandom = Random.Range(0, 4);
            int colStartRandom = Random.Range(0, 4);

            box = boxes[rowStartRandom][colStartRandom];
            boxPassed.Add(box);
            box.isVisible = true;

            line.startColor = GameController.instance.lineColor;
            line.endColor = GameController.instance.lineColor;
            line.positionCount = 0;

            box.IsOk(true);
            box.IsStart(true);

            while (true)
            {
                yield return new WaitForSeconds(0.025f);
                GetDirectionPathFinding(box);
                RandomDir(box.row, box.col);
                if (dir.Count != 0)
                {
                    boxPassed.Add(box);
                    box.IsOk(true);
                    box.isVisible = true;
                    line.positionCount++;
                    line.SetPosition(line.positionCount - 1, new Vector3(box.transform.position.x, box.transform.position.y + 0.025f, 100));
                }
                else
                {
                    Debug.LogError("Path not found !");
                    yield break;
                }
            }
        }

        void RandomDir(int row, int col)
        {
            if (dir.Count == 0)
            {
                Debug.LogError("Count = 0");
                return;
            }
            int i = Random.Range(0, dir.Count);
            if (dir[i] == Direction.Left)
            {
                box = boxes[row][col - 1];
            }
            else if (dir[i] == Direction.Right)
            {
                box = boxes[row][col + 1];
            }
            else if (dir[i] == Direction.Top)
            {
                box = boxes[row - 1][col];
            }
            else
            {
                box = boxes[row + 1][col];
            }
        }

        string HexConvert(Color color)
        {
            return "#" + ColorUtility.ToHtmlStringRGB(color);
        }

        void MapSave()
        {
            LevelConfig levelConfig = new LevelConfig();
            levelConfig.boxHex = HexConvert(GameController.instance.boxColor);
            levelConfig.totalWin = boxPassed.Count;
            levelConfig.boxConfigs = new BoxConfig[7][];
            levelConfig.boxPassed = new BoxDataStorage[boxPassed.Count];
            for (int i = 0; i < boxPassed.Count; i++)
            {
                levelConfig.boxPassed[i] = new BoxDataStorage(boxPassed[i].row, boxPassed[i].col);
            }

            for (int i = 0; i < levelConfig.boxConfigs.Length; i++)
            {
                BoxConfig[] child = new BoxConfig[7];
                for (int j = 0; j < child.Length; j++)
                {
                    child[j] = new BoxConfig();
                    child[j].isOk = GameController.instance.boxes[i][j].isOK;
                }
                levelConfig.boxConfigs[i] = child;
            }

            int maxRow = int.MinValue;
            int minRow = int.MaxValue;
            int maxCol = int.MinValue;
            int minCol = int.MaxValue;

            for (int i = 0; i < boxes.Length; i++)
            {
                for (int j = 0; j < boxes[i].Length; j++)
                {
                    if (boxes[i][j].isOK)
                    {
                        if (boxes[i][j].row < minRow)
                        {
                            minRow = boxes[i][j].row;
                        }
                        if (boxes[i][j].row > maxRow)
                        {
                            maxRow = boxes[i][j].row;
                        }
                        if (boxes[i][j].col < minCol)
                        {
                            minCol = boxes[i][j].col;
                        }
                        if (boxes[i][j].col > maxCol)
                        {
                            maxCol = boxes[i][j].col;
                        }
                    }
                }
            }

            string folder = "";
            int sizeX = maxCol - minCol;
            if (sizeX == 1) folder = "2x2";
            else if (sizeX == 2) folder = "3x3";
            else if (sizeX == 3) folder = "4x4";
            else if (sizeX == 4) folder = "5x5";
            else if (sizeX == 5) folder = "6x6";
            else folder = "7x7";

            if(minCol - 0 > boxes.Length - maxCol) levelConfig.horiPadding = -minCol;
            else if(minCol - 0 < boxes.Length - 1 - maxCol) levelConfig.horiPadding = boxes.Length - 1 - maxCol;
            else levelConfig.horiPadding = 0;

            if (minRow - 0 > boxes.Length - maxRow) levelConfig.vertPadding = -minRow;
            else if (minRow - 0 < boxes.Length - 1 - maxRow) levelConfig.vertPadding = boxes.Length - 1 - maxRow;
            else levelConfig.vertPadding = 0;

            //levelConfig.boxConfigs[boxStart.row][boxStart.col].isStart = true;
            levelConfig.boxConfigs[boxPassed[0].row][boxPassed[0].col].isStart = true;

            int level = GetTotal(folder) + 1;
            string js = JsonConvert.SerializeObject(levelConfig);
            string path = Path.Combine(Application.dataPath, "HieuBon/Resources/OneLine/" + folder + "/" + folder + "_" + level + ".json");
            File.WriteAllText(path, js);

            Debug.Log("Hori Padding = " + levelConfig.horiPadding);
            Debug.Log("Vert Padding = " + levelConfig.vertPadding);
            Debug.Log("Min Row = " + minRow);
            Debug.Log("Min Col = " + minCol);
            Debug.Log("Folder = " + folder);
            Debug.Log("Save Complete !!!");
        }

        void Restart()
        {
            box = GetBoxStart();
            boxPassed.Add(box);
            box.isVisible = true;

            line.startColor = GameController.instance.lineColor;
            line.endColor = GameController.instance.lineColor;
            line.positionCount = 0;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, new Vector3(box.transform.position.x, box.transform.position.y + 0.025f, 100));
        }

        void DrawLine()
        {
            while (!isNotFound)
            {
                GetDirection(box);
                CheckDir(box.row, box.col, dirTakenChild);
                if (isNotFound)
                {
                    System.GC.Collect();
                    //MapGenerate();
                    return;
                }
                if (box != null)
                {
                    boxPassed.Add(box);
                    box.isVisible = true;
                    line.positionCount++;
                    line.SetPosition(line.positionCount - 1, new Vector3(box.transform.position.x, box.transform.position.y + 0.025f, 100));
                }
                else
                {
                    Box lastBox = boxPassed[boxPassed.Count - 1];
                    boxPassed.Remove(lastBox);
                    lastBox.isVisible = false;
                    box = boxPassed[boxPassed.Count - 1];
                    line.positionCount--;
                }
                if (boxPassed.Count == totalWin)
                {
                    Debug.LogWarning("Ok");
                    return;
                }
            }
        }

        bool IsContainsDirTaken(List<Direction> dir)
        {
            for (int i = 0; i < dirTaken.Count; i++)
            {
                if (dirTaken[i].directions.Count < dir.Count) continue;
                bool isContain = true;
                for (int j = 0; j < dir.Count; j++)
                {
                    if (dir[j] != dirTaken[i].directions[j])
                    {
                        isContain = false;
                        break;
                    }
                }
                if (isContain)
                {
                    return true;
                }
            }
            return false;
        }

        void CheckDir(int row, int col, List<Direction> dirTakenChild)
        {
            box = null;
            List<Direction> temp = new List<Direction>(dirTakenChild);
            bool isOk = false;
            for (int i = 0; i < dir.Count; i++)
            {
                temp.Add(dir[i]);
                if (!IsContainsDirTaken(temp))
                {
                    isOk = true;
                    if (dir[i] == Direction.Left)
                    {
                        box = boxes[row][col - 1];
                        dirTakenChild.Add(Direction.Left);
                    }
                    else if (dir[i] == Direction.Right)
                    {
                        box = boxes[row][col + 1];
                        dirTakenChild.Add(Direction.Right);
                    }
                    else if (dir[i] == Direction.Top)
                    {
                        box = boxes[row - 1][col];
                        dirTakenChild.Add(Direction.Top);
                    }
                    else
                    {
                        box = boxes[row + 1][col];
                        dirTakenChild.Add(Direction.Bottom);
                    }
                    break;
                }
                temp.RemoveAt(temp.Count - 1);
            }
            if (!isOk)
            {
                if (dir.Count == 0) dirTaken.Add(new DirectionTaken(dirTakenChild));
                if (dirTakenChild.Count == 0)
                {
                    isNotFound = true;
                    Debug.LogError("NOk");
                    System.GC.Collect();
                    return;
                }
                dirTakenChild.RemoveAt(dirTakenChild.Count - 1);
            }
        }

        void GetDirection(Box box)
        {
            dir.Clear();
            if (box.col - 1 >= 0 && boxes[box.row][box.col - 1].isOK && !boxes[box.row][box.col - 1].isVisible)
            {
                dir.Add(Direction.Left);
            }
            if (box.col + 1 < boxes.Length && boxes[box.row][box.col + 1].isOK && !boxes[box.row][box.col + 1].isVisible)
            {
                dir.Add(Direction.Right);
            }
            if (box.row - 1 >= 0 && boxes[box.row - 1][box.col].isOK && !boxes[box.row - 1][box.col].isVisible)
            {
                dir.Add(Direction.Top);
            }
            if (box.row + 1 < boxes[box.col].Length && boxes[box.row + 1][box.col].isOK && !boxes[box.row + 1][box.col].isVisible)
            {
                dir.Add(Direction.Bottom);
            }
            Debug.LogWarning(dir.Count);
        }

        void GetDirectionPathFinding(Box box)
        {
            dir.Clear();
            if (box.col - 1 >= 0 && !boxes[box.row][box.col - 1].isVisible)
            {
                dir.Add(Direction.Left);
            }
            if (box.col + 1 < boxes.Length - 3 && !boxes[box.row][box.col + 1].isVisible)
            {
                dir.Add(Direction.Right);
            }
            if (box.row - 1 >= 0 && !boxes[box.row - 1][box.col].isVisible)
            {
                dir.Add(Direction.Top);
            }
            if (box.row + 1 < boxes[box.col].Length - 2 && !boxes[box.row + 1][box.col].isVisible)
            {
                dir.Add(Direction.Bottom);
            }
        }

        Box GetBoxStart()
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                for (int j = 0; j < boxes[i].Length; j++)
                {
                    if (boxes[i][j].iconStart.gameObject.activeSelf)
                    {
                        return boxes[i][j];
                    }
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class DirectionTaken
    {
        public List<Tool.Direction> directions;

        public DirectionTaken(List<Tool.Direction> directions)
        {
            this.directions = new List<Tool.Direction>(directions);
        }
    }
}
