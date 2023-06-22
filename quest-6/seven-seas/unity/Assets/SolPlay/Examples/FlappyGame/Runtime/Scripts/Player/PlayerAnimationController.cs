using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Player
{
    public static class PlayerAnimationKeys
    {
        public static readonly int VelocityYId = Animator.StringToHash("VelocityY");
    }

    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] Animator _anim;
        [SerializeField] PlayerController _player;

        public void EnableAnimations(bool value) => _anim.enabled = value;

        private void LateUpdate() 
        {
            _anim.SetFloat(PlayerAnimationKeys.VelocityYId, _player.Velocity.y);
        }
    }
}