using System.Collections.Generic;
using UnityEngine;

namespace HieuBon
{
    public class Tool : MonoBehaviour
    {
        public Box[][] boxes;
        public LevelConfig levelConfig;
        public List<Direction> dir = new List<Direction>();
        public List<List<Direction>> dirTaken = new List<List<Direction>>();
        public LineRenderer[] lineRenderers;
        int indexLine;
        public Box box;
        public List<Direction> dirTakenChild = new List<Direction>();
        public List<Box> boxPassed = new List<Box>();
        public int limit;

        public enum Direction
        {
            None, Left, Right, Top, Bottom
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                boxes = GameController.instance.boxes;
                levelConfig = GameController.instance.levelConfig;
                box = GetBoxStart();
                boxPassed.Add(box);
                box.isVisible = true;
                lineRenderers[indexLine].positionCount++;
                lineRenderers[indexLine].SetPosition(lineRenderers[indexLine].positionCount - 1, new Vector3(box.transform.position.x, box.transform.position.y + 0.025f, 100));
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                DrawLine();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                lineRenderers[indexLine].positionCount = 0;
            }
        }

        void DrawLine()
        {
            Color color = Color.red; //new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            lineRenderers[indexLine].startColor = color;
            lineRenderers[indexLine].endColor = color;
            lineRenderers[indexLine].startWidth = 0.1f;
            lineRenderers[indexLine].endWidth = 0.1f;

            /*for (int k = 0; k < boxes.Length; k++)
            {
                for (int j = 0; j < boxes[k].Length; j++)
                {
                    if (!levelConfig.boxConfigs[k][j].isStart) boxes[k][j].isVisible = false;
                }
            }*/

            int i = 0;
            while (i < limit)
            {
                GetDirection(box);
                CheckDir(box.row, box.col, dirTakenChild);
                if (box != null)
                {
                    boxPassed.Add(box);
                    box.isVisible = true;
                    lineRenderers[indexLine].positionCount++;
                    lineRenderers[indexLine].SetPosition(lineRenderers[indexLine].positionCount - 1, new Vector3(box.transform.position.x, box.transform.position.y + 0.025f, 100));
                }
                else
                {
                    Box lastBox = boxPassed[boxPassed.Count - 1];
                    boxPassed.Remove(lastBox);
                    lastBox.isVisible = false;
                    box = boxPassed[boxPassed.Count - 1];
                    lineRenderers[indexLine].positionCount--;
                }
                i++;
                if (dir.Count == 0)
                {
                    Debug.LogError("Not found !!!");
                    if (dirTaken.Count == levelConfig.totalWin)
                    {
                        Debug.LogWarning("Ok");
                        return;
                    }
                }
            }
        }

        bool IsContainsDirTaken(List<Direction> dir)
        {
            for (int i = 0; i < dirTaken.Count; i++)
            {
                if (dirTaken[i].Count < dir.Count) continue;
                bool isContain = true;
                for (int j = 0; j < dir.Count; j++)
                {
                    if (dir[j] != dirTaken[i][j])
                    {
                        isContain = false;
                        break;
                    }
                }
                if (isContain) return true;
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
            }
            if (!isOk && dir.Count == 0)
            {
                dirTaken.Add(new List<Direction>(dirTakenChild));
                Debug.LogError("Not found !!!");
            }
            if (!isOk)
            {
                dirTakenChild.RemoveAt(dirTakenChild.Count - 1);
            }
        }

        void GetDirection(Box box)
        {
            dir.Clear();
            //Debug.LogWarning(box.row + " " + box.col);
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
                    if (levelConfig.boxConfigs[i][j].isStart) return boxes[i][j];
                }
            }
            return null;
        }
    }
}
