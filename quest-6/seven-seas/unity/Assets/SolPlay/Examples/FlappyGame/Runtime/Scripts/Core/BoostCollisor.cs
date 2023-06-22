using SolPlay.FlappyGame.Runtime.Scripts.Audio;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Core
{
    public class BoostCollisor : MonoBehaviour
    {
        [SerializeField] GameMode _gameMode;
        [SerializeField] AudioClip _audioOnIncrement;

        public void IncrementBoost()
        {
            _gameMode.IncrementBoost();
            AudioUtility.PlaySFX(_audioOnIncrement);
        }
    
        public void StopBoost()
        {
            _gameMode.StopBoost();
            AudioUtility.PlaySFX(_audioOnIncrement);
        }

    }
}
