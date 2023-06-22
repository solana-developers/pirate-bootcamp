using SolPlay.FlappyGame.Runtime.Scripts.Core;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.UI.Screens
{
    public class WaitingScreen : MonoBehaviour
    {
        [SerializeField] GameMode _gameMode;
        [SerializeField] float _timeToWait;

        private void OnEnable() 
        {
            _gameMode.StartWithDelay(_timeToWait);
        }
    }
}
