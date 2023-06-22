using SolPlay.FlappyGame.Runtime.Scripts.Core;
using SolPlay.FlappyGame.Runtime.Scripts.Level.Pipes;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Player
{
    public class PlayerDeathController : MonoBehaviour
    {
        [SerializeField] PlayerController _playerController;
        [SerializeField] PlayerInputs _playerInputs;
        [SerializeField] PlayerAnimationController _playerAnimation;
        [SerializeField] GameMode _gameMode;

        private bool _playerIsDied;
        public bool PlayerIsDead => _playerIsDied;
        public PlayerController Controller => _playerController;

        public void Reset()
        {
            _playerIsDied = false;
            _playerInputs.EnableInputs(true);
        }

        public void Die() 
        {
            if(!_playerIsDied)
            {
                _playerController.Die();
                _playerAnimation.EnableAnimations(false);
                _playerInputs.EnableInputs(false);
                _playerIsDied = true;
                _gameMode.GameOver();
            }    
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!_playerIsDied && other.GetComponent<Pipe>() is Pipe pipe)
            {
                _playerController.Die();
                _playerAnimation.EnableAnimations(false);
                _playerInputs.EnableInputs(false);
                _playerIsDied = true;
            }    
        }
    }
}