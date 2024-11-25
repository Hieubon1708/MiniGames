using UnityEngine;
using UnityEngine.UI;

namespace HieuBon
{
    public class Box : MonoBehaviour
    {
        public bool isStart;
        public bool isEnd;
        public bool isVisible;
        public bool isOK;
        public Animation lightAni;
        public Image image;
        public Image iconStart;
        public Image iconEnd;
        public CanvasGroup canvasGroup;

        public void IsStart(bool isStart)
        {
            this.isStart = isStart;
            if (this.isStart)
            {
                isVisible = true;
                iconStart.gameObject.SetActive(true);
                image.color = GameController.instance.boxColor;
                iconStart.color = GameController.instance.lineColor;
                GameController.instance.playerController.SetLineStart(transform.position);
            }
        }

        public void IsOk(bool isOK)
        {
            this.isOK = isOK;
            if (!this.isOK) canvasGroup.alpha = 0;
        }

        public void PlayLightAni()
        {
            lightAni.Play();
        }

        public void Hide()
        {
            isVisible = false;
            image.color = GameController.instance.defaultColor;
        }

        public void Show()
        {
            isVisible = true;
            image.color = GameController.instance.boxColor;
            PlayLightAni();
        }

        public void ResetBox()
        {
            iconStart.gameObject.SetActive(false);
            iconEnd.gameObject.SetActive(false);
            isVisible = false;
            isStart = false;
            isEnd = false;
            isOK = false;
            canvasGroup.alpha = 1;
            image.color = GameController.instance.defaultColor;
        }
    }
}
