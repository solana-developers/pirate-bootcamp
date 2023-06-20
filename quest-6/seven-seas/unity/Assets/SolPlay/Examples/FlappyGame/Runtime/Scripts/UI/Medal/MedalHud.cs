using UnityEngine;
using UnityEngine.UI;

namespace SolPlay.FlappyGame.Runtime.Scripts.UI.Medal
{
    [RequireComponent(typeof(MedalParamatersHandler))]
    public class MedalHud : MonoBehaviour
    {
        [SerializeField] MedalParamatersHandler _medalHandler;
        [SerializeField] Image _renderer;

        public void HandleScore(int score)
        {
            Medal medal = _medalHandler.GetMedalWithScore(score);

            if(medal != null)
            {
                _renderer.sprite = medal.Renderer;
                _renderer.gameObject.SetActive(true);
            }
            else
            {
                _renderer.gameObject.SetActive(false);
            }
        }
    }
}
