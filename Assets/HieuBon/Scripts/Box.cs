using UnityEngine;
using UnityEngine.UI;

namespace HieuBon
{
    public class Box : MonoBehaviour
    {
        public bool isStart;
        public bool isEnd;
        public bool isVisible;
        public Animation lightAni;
        public Image image;
        public GameObject iconStart;
        public GameObject iconEnd;

        public void IsStart()
        {
            if(isStart) iconStart.SetActive(true);
        }

        public void IsEnd()
        {
            if (isEnd) iconEnd.SetActive(true);
        }

        public void PlayLightAni()
        {
            lightAni.Play();
        }

        public void SetBoxColor()
        {
            image.color = GameController.instance.boxColor;
        }
    }
}
