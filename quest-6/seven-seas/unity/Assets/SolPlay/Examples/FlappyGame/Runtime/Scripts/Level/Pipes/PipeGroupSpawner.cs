using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Level.Pipes
{
    public class PipeGroupSpawner : MonoBehaviour
    {
        [Header("Manager")]
        [SerializeField] GameObject _pipeTopPrefab;
        [SerializeField] GameObject _pipeBottomPrefab;
        [SerializeField] Transform _player;
        [SerializeField] int _spawnOnInit;
        [SerializeField] int _minPipesOnScreen;

        [Header("Position Controller")]
        [SerializeField] Vector3 _initialPosition;
        [SerializeField] float _offsetX;
        [SerializeField] float _minPipeBottomY;
        [SerializeField] float _maxPipeBottomY;

        [Header("Gap")]
        [SerializeField] float _minGapHeight;
        [SerializeField] float _maxGapHeight;
    
        private PipeGroup _lastPipe;

        public void Reset()
        {
            _lastPipe = null;
        }

        public PipeGroup SpawnPipes()
        {
            Pipe pipeBottom = Instantiate(_pipeBottomPrefab, transform).GetComponent<Pipe>();

            Vector3 pipePosition = new Vector3();

            pipePosition.x = _lastPipe != null
                ? _lastPipe.XPosition + _offsetX
                : _initialPosition.x;

            pipePosition.y = Random.Range(_minPipeBottomY, _maxPipeBottomY);

            pipeBottom.transform.position = pipePosition;

            Pipe pipeTop = Instantiate(_pipeTopPrefab, transform).GetComponent<Pipe>();
            float gapSize = Random.Range(_minGapHeight, _maxGapHeight);
            pipeTop.SetPositionTopAnchored(pipeBottom.TopPosition + Vector3.up * gapSize);

            _lastPipe = new PipeGroup 
            {
                bottomPipe = pipeBottom,
                topPipe = pipeTop
            };

            return _lastPipe;
        }
    }
}
