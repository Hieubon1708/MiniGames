using UnityEngine;

namespace HieuBon
{
    public class UIController : MonoBehaviour
    {
        public static UIController instance;

        public GameObject touchHover;

        public void Awake()
        {
            instance = this;
        }

        public void ActiveTouch(bool isActive)
        {
            touchHover.SetActive(isActive);
        }
        
        public void TouchHoverPosition(Vector2 position)
        {
            touchHover.transform.position = position;
        }
    }
}