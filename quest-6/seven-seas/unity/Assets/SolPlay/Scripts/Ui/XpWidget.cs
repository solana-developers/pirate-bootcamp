using System.Collections;
using Frictionless;
using SolPlay.Scripts.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolPlay.Scripts.Ui
{
    public class XpWidget : MonoBehaviour
    {
        public Slider XPSlider;
        public TextMeshProUGUI XpText;
        public TextMeshProUGUI LevelText;
        public bool AnimateAtStart = true;

        private Coroutine animationCoroutine;
        private uint shownXp;

        private void Start()
        {
            MessageRouter.AddHandler<NewHighScoreLoadedMessage>(OnNewHighscoreLoadedMessage);
            MessageRouter.AddHandler<NftLoadingFinishedMessage>(OnNftLoadingFinishedMessage);
            if (!AnimateAtStart)
            {
                var totalScore = ServiceFactory.Resolve<HighscoreService>().GetTotalScore();
                shownXp = totalScore;
            }

            AnimateXp();
        }

        private void OnNftLoadingFinishedMessage(NftLoadingFinishedMessage messge)
        {
            if (!AnimateAtStart)
            {
                var totalScore = ServiceFactory.Resolve<HighscoreService>().GetTotalScore();
                shownXp = totalScore;
            }
        }

        private void OnNewHighscoreLoadedMessage(NewHighScoreLoadedMessage message)
        {
            AnimateXp();
        }

        private void AnimateXp()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }

            var totalScore = ServiceFactory.Resolve<HighscoreService>().GetTotalScore();
            StartCoroutine(AnimateXpRoutine(shownXp, totalScore));
        }

        private IEnumerator AnimateXpRoutine(uint xp, uint newXp)
        {
            while (shownXp <= newXp)
            {
                SetData(shownXp);
                yield return new WaitForSeconds(0.04f);
                shownXp++;
            }
        }

        public void SetData(uint xp)
        {
            int playerLevel = Mathf.FloorToInt(Mathf.Log(xp / 5 + 1, 2));

            var nextLevelXp = GetNextLevelXp(playerLevel);
            var lastLevelXp = GetNextLevelXp(playerLevel - 1);

            XPSlider.minValue = lastLevelXp;
            XPSlider.maxValue = nextLevelXp;
            XPSlider.value = xp;

            XpText.text = $"{xp - lastLevelXp} / {nextLevelXp - lastLevelXp}";
            LevelText.text = (playerLevel + 1).ToString();
        }

        private int GetNextLevelXp(int currentLevel)
        {
            var totalXpNextLevel = 5 * (Mathf.Pow(2, currentLevel + 1) - 1);
            return (int) totalXpNextLevel;
        }
    }
}