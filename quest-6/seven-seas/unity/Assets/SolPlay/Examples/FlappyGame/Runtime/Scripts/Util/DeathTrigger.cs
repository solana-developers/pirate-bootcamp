using SolPlay.FlappyGame.Runtime.Scripts.Player;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Util
{
    [RequireComponent(typeof(Collider2D))]
    public class DeathTrigger : MonoBehaviour
    {
        private void Awake() => GetComponent<Collider2D>().isTrigger = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.GetComponent<PlayerDeathController>() is PlayerDeathController player)
                player.Die();    
        }
    }
}
