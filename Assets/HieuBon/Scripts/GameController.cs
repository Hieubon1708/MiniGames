using UnityEngine;

namespace HieuBon
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;

        public PlayerController playerController;

        public Color lineColor;
        public Color boxColor;
        public Box[] boxes;

        public void Awake()
        {
            instance = this;
        }

        public void Start()
        {
            SetBoxStart();
        }

        void SetBoxStart()
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i].IsStart();
            }
        }

        public void SetBoxColor()
        {

        }
        
        void SetBoxEnd()
        {

        }

        public Box GetBox(GameObject box)
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                if (boxes[i].gameObject == box) return boxes[i];
            }
            return null;
        }
    }
}
