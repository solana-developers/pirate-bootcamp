using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Level.Pipes
{
    [RequireComponent(typeof(PipeGroupSpawner))]
    public class PipeGroupDestroyer : MonoBehaviour
    {
        [SerializeField] PipeGroupSpawner _spawner;
        [SerializeField] Transform _player;
        [SerializeField] float _minDistanceToDestroy;

        public bool CanDestroy(PipeGroup pipe)
        {
            return pipe.XPosition < _player.position.x && (_player.position.x - pipe.XPosition) > _minDistanceToDestroy;
        }

        public void Destroy(PipeGroup pipe) 
        {
            Destroy(pipe.bottomPipe.gameObject);
            Destroy(pipe.topPipe.gameObject);
        }
    }
}
