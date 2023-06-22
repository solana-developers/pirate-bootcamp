using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Core
{
    [RequireComponent(typeof(Collider2D))]
    public class ScoreIncrementTrigger : MonoBehaviour
    {
        private void Awake() => GetComponent<Collider2D>().isTrigger = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.GetComponent<ScoreCollisor>() is ScoreCollisor scoreCollisor)
                scoreCollisor.IncrementScore();
        }
    }
}