using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Core
{
    [RequireComponent(typeof(Collider2D))]
    public class BoostIncrementTrigger : MonoBehaviour
    {
        public float MinOffset = 0;
        public float MaxOffset = 2;
        public int ChanceToSpawnPercentage = 40;
        public GameObject CollectPrefab;
    
        private void Awake()
        {
            var random = Random.Range(0, 101);
            if (random > ChanceToSpawnPercentage)
            {
                gameObject.SetActive(false);
                return;
            }
            GetComponent<Collider2D>().isTrigger = true;
            var transformPosition = transform.localPosition;
            transform.localPosition = new Vector3(transformPosition.x, Random.Range(MinOffset, MaxOffset), transformPosition.z);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<BoostCollisor>() is BoostCollisor boostCollisor)
            {
                boostCollisor.IncrementBoost();
                var collectedFx = Instantiate(CollectPrefab, other.transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }
        }


    }
}