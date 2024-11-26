using System.Collections.Generic;
using UnityEngine;

namespace HieuBon
{
    public class Tool : MonoBehaviour
    {
        public Box[][] boxes;
        public List<Direction> dir = new List<Direction>();
        public List<DirectionTaken> dirTaken = new List<DirectionTaken>();
        public LineRenderer line;
        int indexLine;
        public Box box;
        public List<Direction> dirTakenChild = new List<Direction>();
        public List<Box> boxPassed = new List<Box>();
        public int limit;
        public int totalWin;
        public bool isNotFound;
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
            for (int k = 0; k < boxes.Length; k++)
            {
                for (int j = 0; j < boxes[k].Length; j++)
                {
                    boxes[k][j].canvasGroup.alpha = 0;
                    boxes[k][j].iconStart.gameObject.SetActive(false);
                    boxes[k][j].isVisible = false;
                    boxes[k][j].isOK = false;
                    boxes[k][j].image.color = GameController.instance.defaultColor;
                }
            }
            List<BoxDataStorage> list = new List<BoxDataStorage>();
            int min = 2, max = 4;
            for (int i = min; i <= max; i++)
            {
                for (int j = min; j <= max; j++)
                {
                    list.Add(new BoxDataStorage(i, j));
                }
            }
            int count = 3;
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
                if (i == randomStart) boxes[row][col].IsStart(true);
                totalWin++;
            }
            Restart();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                DrawLine();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                MapGenerate();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                MapSave();
            }
        }

        void MapSave()
        {
        }

        void Restart()
        {
            box = GetBoxStart();
            boxPassed.Add(box);
            box.isVisible = true;

            Color color = Color.red;
            line.startColor = color;
            line.endColor = color;
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
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
                if (isNotFound) return;
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
                dirTaken.Add(new DirectionTaken(dirTakenChild));
                if (dirTakenChild.Count == 0)
                {
                    Debug.LogError("Nok");
                    isNotFound = true;
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
        }

        Box GetBoxStart()
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                for (int j = 0; j < boxes[i].Length; j++)
                {
                    if (boxes[i][j].iconStart.gameObject.activeSelf) return boxes[i][j];
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
