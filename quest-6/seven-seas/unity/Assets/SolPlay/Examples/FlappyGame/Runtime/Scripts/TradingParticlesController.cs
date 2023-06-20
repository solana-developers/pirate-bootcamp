using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts
{
    public class TradingParticlesController : MonoBehaviour
    {
        public ParticleSystem ParticleSystem;
    
        private bool IsMovingUp;
        private Vector3 LastPosition;

        public Color UpColor;
        public Color DownColor;
    
        void Update()
        {
            bool isMovingUp = transform.position.y > LastPosition.y;

            if (isMovingUp != IsMovingUp)
            {
                var particleSystemMain = ParticleSystem.main;
                particleSystemMain.startColor = isMovingUp ? new ParticleSystem.MinMaxGradient(UpColor) : new ParticleSystem.MinMaxGradient(DownColor);
                IsMovingUp = isMovingUp;
            }
            LastPosition = transform.position;
        }
    }
}
