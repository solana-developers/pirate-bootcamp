using SolPlay.FlappyGame.Runtime.Scripts.Core;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.UI.Screens
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField] GameMode _gameMode;

        public void ResumeGame() => _gameMode.ResumeGame();
    }
}
