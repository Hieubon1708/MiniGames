using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace HieuBon
{
    public class PlayerController : MonoBehaviour
    {
        public bool isStartDrag;
        public bool isDrag;
        public List<Box> scBoxPassed = new List<Box>();
        public Box boxSelect;
        public LineRenderer lineRenderer;

        public void SetColorLine(Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }

        public void ResetLine()
        {
            lineRenderer.positionCount = 0;
        }

        public void AddBoxPassed(Box box)
        {
            scBoxPassed.Add(box);
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (scBoxPassed.Count == GameController.instance.levelConfig.totalWin) return;
                Vector2 mousePosition = Input.mousePosition;
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = mousePosition;
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                for (int i = 0; i < results.Count; i++)
                {
                    GameObject e = results[i].gameObject;
                    if (e.CompareTag("Box"))
                    {
                        Box box = GameController.instance.GetBox(e);
                        if (box.isVisible)
                        {
                            isDrag = true;
                            UIController.instance.ActiveTouch(true);
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDrag = false;
                if (scBoxPassed.Count == GameController.instance.levelConfig.totalWin) return;
                UIController.instance.ActiveTouch(false);
            }
            if (isDrag)
            {
                if (scBoxPassed.Count == GameController.instance.levelConfig.totalWin) return;
                Vector2 mousePosition = Input.mousePosition;
                UIController.instance.TouchHoverPosition(mousePosition);
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = mousePosition;
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                for (int i = 0; i < results.Count; i++)
                {
                    GameObject e = results[i].gameObject;
                    if (e.CompareTag("Box"))
                    {
                        boxSelect = GameController.instance.GetBox(e);
                        if (!boxSelect.isOK) return;
                        if (boxSelect.isVisible)
                        {
                            Cut(boxSelect);
                            return;
                        }
                        //Debug.LogWarning(boxSelect.name);
                        ChainSelect(boxSelect);
                    }
                }
            }
        }

        void ChainSelect(Box target)
        {
            if (target == scBoxPassed[scBoxPassed.Count - 1]) return;
            Box lastSelect = scBoxPassed[scBoxPassed.Count - 1];
            if (target.row == lastSelect.row)
            {
                int row = target.row;
                int min = Mathf.Min(target.col, lastSelect.col);
                int max = Mathf.Max(target.col, lastSelect.col);

                if (IsAStraightRow(row, min, max, lastSelect))
                {
                    if (target.col < lastSelect.col)
                    {
                        for (int i = max; i >= min; i--)
                        {
                            Box box = GameController.instance.boxes[row][i];
                            if (box != lastSelect)
                            {
                                box.Show();
                                scBoxPassed.Add(box);
                                LineSetPos(new Vector3(box.transform.position.x, box.transform.position.y + 0.025f, 100));
                            }
                        }
                    }
                    else if (target.col > lastSelect.col)
                    {
                        for (int i = min; i <= max; i++)
                        {
                            Box box = GameController.instance.boxes[row][i];
                            if (box != lastSelect)
                            {
                                box.Show();
                                scBoxPassed.Add(box);
                                LineSetPos(new Vector3(box.transform.position.x, box.transform.position.y + 0.025f, 100));
                            }
                        }
                    }
                }
            }
            else if (target.col == lastSelect.col)
            {
                int col = target.col;
                int min = Mathf.Min(target.row, lastSelect.row);
                int max = Mathf.Max(target.row, lastSelect.row);

                if (IsAStraightCol(col, min, max, lastSelect))
                {
                    if (target.row < lastSelect.row)
                    {
                        for (int i = max; i >= min; i--)
                        {
                            Box box = GameController.instance.boxes[i][col];
                            if (box != lastSelect)
                            {
                                box.Show();
                                scBoxPassed.Add(box);
                                LineSetPos(new Vector3(box.transform.position.x, box.transform.position.y + 0.025f, 100));
                            }
                        }
                    }
                    else if (target.row > lastSelect.row)
                    {
                        for (int i = min; i <= max; i++)
                        {
                            Box box = GameController.instance.boxes[i][col];
                            if (box != lastSelect)
                            {
                                box.Show();
                                scBoxPassed.Add(box);
                                LineSetPos(new Vector3(box.transform.position.x, box.transform.position.y + 0.025f, 100));
                            }
                        }
                    }
                }
            }
            else
            {
                Box boxLeft = lastSelect.col - 1 >= 0 ? GameController.instance.boxes[lastSelect.row][lastSelect.col - 1] : null;
                Box boxRight = lastSelect.col + 1 < GameController.instance.boxes[lastSelect.row].Length ? GameController.instance.boxes[lastSelect.row][lastSelect.col + 1] : null;
                Box boxUp = lastSelect.row - 1 >= 0 ? GameController.instance.boxes[lastSelect.row - 1][lastSelect.col] : null;
                Box boxDown = lastSelect.row + 1 < GameController.instance.boxes.Length ? GameController.instance.boxes[lastSelect.row + 1][lastSelect.col] : null;

                bool isOk = false;

                if (target.row == lastSelect.row - 1 && target.col == lastSelect.col - 1)
                {
                    if (boxLeft && boxLeft.isOK && (!boxUp || !boxUp.isOK))
                    {
                        boxLeft.Show();
                        scBoxPassed.Add(boxLeft);
                        LineSetPos(new Vector3(boxLeft.transform.position.x, boxLeft.transform.position.y + 0.025f, 100));
                        isOk = true;
                    }
                    else if ((!boxLeft || !boxLeft.isOK) && boxUp && boxUp.isOK)
                    {
                        boxUp.Show();
                        scBoxPassed.Add(boxUp);
                        LineSetPos(new Vector3(boxUp.transform.position.x, boxUp.transform.position.y + 0.025f, 100));
                        isOk = true;
                    }
                }
                else if (target.row == lastSelect.row - 1 && target.col == lastSelect.col + 1)
                {
                    if (boxRight && boxRight.isOK && (!boxUp || !boxUp.isOK))
                    {
                        boxRight.Show();
                        scBoxPassed.Add(boxRight);
                        LineSetPos(new Vector3(boxRight.transform.position.x, boxRight.transform.position.y + 0.025f, 100));
                        isOk = true;
                    }
                    else if ((!boxRight || !boxRight.isOK) && boxUp && boxUp.isOK)
                    {
                        boxUp.Show();
                        scBoxPassed.Add(boxUp);
                        LineSetPos(new Vector3(boxUp.transform.position.x, boxUp.transform.position.y + 0.025f, 100));
                        isOk = true;
                    }
                }
                else if (target.row == lastSelect.row + 1 && target.col == lastSelect.col - 1)
                {
                    if (boxLeft && boxLeft.isOK && (!boxDown || !boxDown.isOK))
                    {
                        boxLeft.Show();
                        scBoxPassed.Add(boxLeft);
                        LineSetPos(new Vector3(boxLeft.transform.position.x, boxLeft.transform.position.y + 0.025f, 100));
                        isOk = true;
                    }
                    else if ((!boxLeft || !boxLeft.isOK) && boxDown && boxDown.isOK)
                    {
                        boxDown.Show();
                        scBoxPassed.Add(boxDown);
                        LineSetPos(new Vector3(boxDown.transform.position.x, boxDown.transform.position.y + 0.025f, 100));
                        isOk = true;
                    }
                }
                else if (target.row == lastSelect.row + 1 && target.col == lastSelect.col + 1)
                {
                    if (boxRight && boxRight.isOK && (!boxDown || !boxDown.isOK))
                    {
                        boxRight.Show();
                        scBoxPassed.Add(boxRight);
                        LineSetPos(new Vector3(boxRight.transform.position.x, boxRight.transform.position.y + 0.025f, 100));
                        isOk = true;
                    }
                    else if ((!boxRight || !boxRight.isOK) && boxDown & boxDown.isOK)
                    {
                        boxDown.Show();
                        scBoxPassed.Add(boxDown);
                        LineSetPos(new Vector3(boxDown.transform.position.x, boxDown.transform.position.y + 0.025f, 100));
                        isOk = true;
                    }
                }
                if (isOk)
                {
                    target.Show();
                    scBoxPassed.Add(target);
                    LineSetPos(new Vector3(target.transform.position.x, target.transform.position.y + 0.025f, 100));
                }
            }
            if(scBoxPassed.Count == GameController.instance.levelConfig.totalWin)
            {
                UIController.instance.ActiveTouch(false);
                scBoxPassed[0].ShowStart();
                scBoxPassed[scBoxPassed.Count - 1].ShowEnd();

                scBoxPassed[0].iconStart.transform.DOScale(1.1f, 0.1f).SetUpdate(true);
                scBoxPassed[scBoxPassed.Count - 1].iconEnd.transform.DOScale(1.1f, 0.1f).SetUpdate(true);
                DOVirtual.Float(0.25f, 0.3f, 0.1f, (v) =>
                {
                    lineRenderer.startWidth = v;
                    lineRenderer.endWidth = v;
                }).SetUpdate(true);

                for (int i = 0; i < scBoxPassed.Count; i++)
                {
                    int index = i;
                    DOVirtual.DelayedCall(index * 0.025f, delegate
                    {
                        scBoxPassed[index].PlayLightWinAni();
                    });
                }
                DOVirtual.DelayedCall((scBoxPassed.Count - 1) * 0.025f, delegate
                {
                    scBoxPassed[0].iconStart.transform.DOScale(1f, 0.1f).SetUpdate(true);
                    scBoxPassed[scBoxPassed.Count - 1].iconEnd.transform.DOScale(1f, 0.1f).SetUpdate(true);
                    DOVirtual.Float(0.3f, 0.25f, 0.1f, (v) =>
                    {
                        lineRenderer.startWidth = v;
                        lineRenderer.endWidth = v;
                    }).SetUpdate(true);
                });
            }
        }

        bool IsAStraightRow(int row, int min, int max, Box lastSelect)
        {
            for (int i = min; i <= max; i++)
            {
                Box box = GameController.instance.boxes[row][i];
                if (!box.isOK
                    || box.isVisible
                    && box != lastSelect) return false;
            }
            return true;
        }

        bool IsAStraightCol(int col, int min, int max, Box lastSelect)
        {
            for (int i = min; i <= max; i++)
            {
                Box box = GameController.instance.boxes[i][col];
                if (!box.isOK
                    || box.isVisible
                    && box != lastSelect) return false;
            }
            return true;
        }

        public void LineSetPos(Vector3 pos)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
        }

        void Cut(Box box)
        {
            if (box == scBoxPassed[scBoxPassed.Count - 1]) return;
            int index = scBoxPassed.IndexOf(box);
            for (int i = scBoxPassed.Count - 1; i > index; i--)
            {
                scBoxPassed[i].Hide();
                scBoxPassed.RemoveAt(i);
                lineRenderer.positionCount--;
            }
        }

        public void Restart()
        {
            scBoxPassed.Clear();
            ResetLine();
        }

        public void OnDestroy()
        {
            lineRenderer.DOKill();
            if (scBoxPassed.Count != 0)
            {
                scBoxPassed[0].DOKill();
                scBoxPassed[scBoxPassed.Count - 1].DOKill();
            }
        }
    }
}