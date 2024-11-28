using UnityEngine;
using UnityEngine.UI;

namespace OneLine
{
    public class Box : MonoBehaviour
    {
        public int row;
        public int col;
        public bool isVisible;
        public bool isOK;
        public Animation lightAni;
        public Animation lightWinAni;
        public Image image;
        public Image iconStart;
        public Image iconChildStart;
        public Image iconEnd;
        public Image iconChildEnd;
        public CanvasGroup canvasGroup;

        public void IsStart(bool isStart)
        {
            if (isStart)
            {
                iconStart.gameObject.SetActive(true);
                iconStart.color = GameController.instance.lineColor;
                LoadData();
            }
        }

        public void LoadData()
        {
            isVisible = true;
            image.color = GameController.instance.boxColor;
            GameController.instance.playerController.AddBoxPassed(this);
            GameController.instance.playerController.LineSetPos(new Vector3(transform.position.x, transform.position.y + 0.025f, 100));
        }

        public void IsOk(bool isOK)
        {
            this.isOK = isOK;
            if (this.isOK) canvasGroup.alpha = 1;
        }

        public void PlayLightAni()
        {
            lightAni.Play();
        }
        
        public void PlayLightWinAni()
        {
            lightWinAni.Play();
        }

        public void ShowEnd()
        {
            Color color = GameController.instance.lineColor;
            iconEnd.color = color;
            iconChildEnd.color = new Color(color.r - 150f / 255f, color.g - 150f / 255f, color.b - 150f / 255f);
            iconEnd.gameObject.SetActive(true);
        }

        public void ShowStart()
        {
            Color color = GameController.instance.lineColor;
            iconChildStart.color = new Color(color.r - 150f / 255f, color.g - 150f / 255f, color.b - 150f / 255f);
            iconChildStart.gameObject.SetActive(true);
        }

        public void Hide()
        {
            isVisible = false;
            image.color = GameController.instance.defaultColor;
            GameController.instance.SaveLevel(row, col, isVisible);
        }

        public void Show()
        {
            isVisible = true;
            image.color = GameController.instance.boxColor;
            PlayLightAni();
            GameController.instance.SaveLevel(row, col, isVisible);
        }

        public void ResetBox()
        {
            iconStart.gameObject.SetActive(false);
            iconChildStart.gameObject.SetActive(false);
            iconEnd.gameObject.SetActive(false);
            isVisible = false;
            isOK = false;
            canvasGroup.alpha = 0;
            image.color = GameController.instance.defaultColor;
        }
    }
}
