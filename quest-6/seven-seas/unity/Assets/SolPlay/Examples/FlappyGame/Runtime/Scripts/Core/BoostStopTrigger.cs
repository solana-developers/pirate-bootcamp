using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Core
{
    [RequireComponent(typeof(Collider2D))]
    public class BoostStopTrigger : MonoBehaviour
    {
        public float MinOffset = 0;
        public float MaxOffset = 2;
    
        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<BoostCollisor>() is BoostCollisor boostCollisor)
            {
                boostCollisor.StopBoost();
                gameObject.SetActive(false);
            }
        }
    }
}