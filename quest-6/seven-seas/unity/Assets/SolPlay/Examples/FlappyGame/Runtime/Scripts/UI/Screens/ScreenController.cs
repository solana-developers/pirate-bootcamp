using System;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.UI.Screens
{
    public class ScreenController : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] screens;

        private GameObject currentScreen;

        private void Awake()
        {
            foreach (GameObject screen in screens)
            {
                screen.gameObject.SetActive(false);
            }
        }

        public void ShowPauseHud() => ShowScreen(FindScreenWithComponent(typeof(PauseScreen)));
        public void ShowStartHud() => ShowScreen(FindScreenWithComponent(typeof(StartScreen)));
        public void ShowWaitingHud() => ShowScreen(FindScreenWithComponent(typeof(WaitingScreen)));
        public void ShowInGameHud() => ShowScreen(FindScreenWithComponent(typeof(InGameHudScreen)));
        public void ShowGameOverHud() => ShowScreen(FindScreenWithComponent(typeof(GameOverScreen)));

        private void ShowScreen(GameObject screen)
        {
            CloseCurrent();
            screen.SetActive(true);
            currentScreen = screen;
        }

        private void CloseCurrent()
        {
            if (currentScreen != null)
            {
                currentScreen.gameObject.SetActive(false);
            }
        }

        private GameObject FindScreenWithComponent(Type type)
        {
            foreach (GameObject screen in screens)
            {
                if (screen.GetComponent(type) != null)
                {
                    return screen;
                }
            }
            return null;
        }
    }
}
