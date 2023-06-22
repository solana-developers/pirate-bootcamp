using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SolPlay.FlappyGame.Runtime.Scripts.UI.Screens
{
    public class FadeScreen : MonoBehaviour
    {
        [SerializeField] Image _fadeImage;

        private const float FlashTime = 0.05f;
        private readonly Color FlashColor = Color.white;

        public IEnumerator Flash()
        {
            yield return StartCoroutine(FadeIn(FlashTime, FlashColor));
            yield return StartCoroutine(FadeOut(FlashTime, FlashColor));
        }

        public IEnumerator FadeIn(float fadeTime, Color fadeColor)
        {
            _fadeImage.enabled = true;
            fadeColor.a = 0;
            _fadeImage.color = fadeColor;

            Tween fade = _fadeImage.DOFade(1, fadeTime);
            yield return fade.WaitForCompletion();
        }

        public IEnumerator FadeOut(float fadeTime, Color fadeColor)
        {
            _fadeImage.enabled = true;
            fadeColor.a = 1;
            _fadeImage.color = fadeColor;

            Tween fade = _fadeImage.DOFade(0, fadeTime);
            yield return fade.WaitForCompletion();

            _fadeImage.enabled = false;
        }
    }
}