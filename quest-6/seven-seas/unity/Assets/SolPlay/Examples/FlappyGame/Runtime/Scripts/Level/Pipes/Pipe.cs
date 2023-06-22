using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Level.Pipes
{
    public class Pipe : MonoBehaviour
    {
        [SerializeField] Transform _topAnchor;
        public Vector3 TopPosition => _topAnchor.position;

        private void OnDrawGizmos() 
        {
            float gizmosSize = 0.2f;
            Gizmos.color = Color.red;
            Gizmos.DrawCube(TopPosition, Vector3.one * gizmosSize);
        }

        public void SetPositionTopAnchored(Vector3 position) 
        {
            Vector3 delta = transform.position - _topAnchor.position;
            _topAnchor.position = position;
            transform.position = _topAnchor.position + delta;
        }
    }
}