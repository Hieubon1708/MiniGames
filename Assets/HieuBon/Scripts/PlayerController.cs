using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HieuBon
{
    public class PlayerController : MonoBehaviour
    {
        public bool isStartDrag;
        public bool isDrag;
        public int totalWin;
        public List<GameObject> boxPassed = new List<GameObject>();
        public List<Box> scBoxPassed = new List<Box>();
        public Box boxSelect;
        public LineRenderer lineRenderer;

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
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
                        isDrag = true;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                UIController.instance.ActiveTouch(false);
                isDrag = false;
            }
            if (isDrag)
            {
                Vector2 mousePosition = Input.mousePosition;
                UIController.instance.TouchHoverPosition(mousePosition);
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = mousePosition;
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                for (int i = 0; i < results.Count; i++)
                {
                    GameObject e = results[i].gameObject;
                    if (e.CompareTag("Box") && !boxPassed.Contains(e))
                    {
                        boxSelect = GameController.instance.GetBox(e);
                        Debug.Log(boxSelect.name);
                        boxPassed.Add(e);
                        scBoxPassed.Add(boxSelect);
                        boxSelect.PlayLightAni();
                        lineRenderer.positionCount++;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(boxSelect.transform.position.x, boxSelect.transform.position.y + 0.25f, 100));
                    }
                }
            }
        }

        void Cut(Box box)
        {

        }

        public void Restart()
        {
            boxPassed.Clear();
            scBoxPassed.Clear();
            totalWin = 0;
        }
    }
}