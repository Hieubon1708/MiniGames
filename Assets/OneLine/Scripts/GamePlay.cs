using TMPro;
using UnityEngine;

namespace OneLine
{
    public class GamePlay : MonoBehaviour
    {
        public GameObject restartButton;
        public GameObject hintButton;
        public GameObject nextLevelButton;
        public GameObject mode;
        public GameObject gamePlay;

        public TextMeshProUGUI textLevel;

        public void Play(int type)
        {
            LevelHandle(GameController.instance.GetType(type));
            UIModeActive(false);
            UIIngameActive(true);
        }

        public void LevelHandle(GameController.LevelType type)
        {
            //Debug.LogWarning(PlayerPrefs.GetInt(GameController.instance.type.ToString(), 1));
            if (GameController.instance.dataManager.GetTotal(type.ToString()) < PlayerPrefs.GetInt(type.ToString(), 1))
            {
                PlayerPrefs.SetInt(type.ToString(), PlayerPrefs.GetInt(type.ToString(), 1) - 1);
            }
            textLevel.text = type.ToString() + " Level " + PlayerPrefs.GetInt(type.ToString(), 1);
            UIEndActive(true);
            GameController.instance.SetLevel(type, PlayerPrefs.GetInt(type.ToString(), 1));
            StartCoroutine(GameController.instance.LoadLevel());
        }

        public void NextLevel()
        {
            LevelHandle(GameController.instance.type);
        }

        public void UIModeActive(bool isActive)
        {
            mode.SetActive(isActive);
        }

        public void UIIngameActive(bool isActive)
        {
            gamePlay.SetActive(isActive);
        }

        public void UIEndActive(bool isActive)
        {
            restartButton.SetActive(isActive);
            hintButton.SetActive(isActive);
            if (GameController.instance.dataManager.GetTotal(GameController.instance.type.ToString()) >= PlayerPrefs.GetInt(GameController.instance.type.ToString(), 1)) nextLevelButton.SetActive(!isActive);
        }

        public void Back()
        {
            GameController.instance.playerController.Kill();
            UIModeActive(true);
            UIIngameActive(false);
        }

        public void Hint()
        {
            GameController.instance.playerController.Hint();
        }
    }
}
