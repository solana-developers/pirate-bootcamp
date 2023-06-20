using SolPlay.FlappyGame.Runtime.Scripts.Audio;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Core
{
    public class ScoreCollisor : MonoBehaviour
    {
        [SerializeField] GameMode _gameMode;
        [SerializeField] AudioClip _audioOnIncrement;

        public void IncrementScore()
        {
            _gameMode.IncrementScore();
            AudioUtility.PlaySFX(_audioOnIncrement);
        }
    }
}
