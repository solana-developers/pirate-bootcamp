using System.Collections;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Core
{
    public class SelfDestroyAfterSeconds : MonoBehaviour
    {
        public float SecondsToLife = 2;

        private void Start()
        {
            StartCoroutine(DestroyDelayed());
        }
        
        private IEnumerator DestroyDelayed()
        {
            yield return new WaitForSeconds(SecondsToLife);
            Destroy(gameObject);
        }
    }
}