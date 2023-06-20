using SolPlay.FlappyGame.Runtime.Scripts.Player;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Level
{
    public class GroundParallax : MonoBehaviour
    {
        [SerializeField] Ground _groundLeft;
        [SerializeField] Ground _groundRight;
        [SerializeField] PlayerController _player;
        [SerializeField] float _tolerance;

        private Vector3 delta;
        private Vector3 groundRightStartPosition;
        private Vector3 groundLeftStartPosition;
    
        private void Start() 
        {
            delta = _groundRight.transform.position - _groundLeft.transform.position;
            groundRightStartPosition = _groundRight.transform.position;
            groundLeftStartPosition = _groundLeft.transform.position;
        }

        public void Reset()
        {
            _groundRight.transform.position = groundRightStartPosition;
            _groundLeft.transform.position = groundLeftStartPosition;
        }

        private void LateUpdate() 
        {
            if(_groundLeft.transform.position.x + _tolerance < _player.transform.position.x)
                _groundLeft.transform.position = _groundRight.transform.position + delta;

            if(_groundRight.transform.position.x + _tolerance < _player.transform.position.x)
                _groundRight.transform.position = _groundLeft.transform.position + delta;
        }
    }
}