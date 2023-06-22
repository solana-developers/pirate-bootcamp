using SolPlay.FlappyGame.Runtime.Scripts.Core;
using TMPro;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.UI.Screens
{
    public class InGameHudScreen : MonoBehaviour
    {
        [SerializeField] private GameMode _gameMode;
        [SerializeField] private TextMeshProUGUI _scoreText;

        public void PauseGame() => _gameMode.PauseGame();

        private void LateUpdate()
        {
            _scoreText.text = _gameMode.Score.ToString();
        }
    }
}
