using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace OneLine
{
    public class UIController : MonoBehaviour
    {
        public static UIController instance;

        public GameObject touchHover;
        public Image touchHoverImage;
        public GamePlay gamePlay;
        public Camera cam;

        public void Awake()
        {
            instance = this;
        }

        public void TouchSetColor(Color color)
        {
            color.a = 0.5f;
            touchHoverImage.color = color;
        }

        public void ActiveTouch(bool isActive)
        {
            touchHover.SetActive(isActive);
        }

        public void TouchHoverPosition(Vector2 position)
        {
            Vector3 pos = cam.ScreenToWorldPoint(position);
            touchHover.transform.position = new Vector3(pos.x, pos.y, 100);
        }
    }
}