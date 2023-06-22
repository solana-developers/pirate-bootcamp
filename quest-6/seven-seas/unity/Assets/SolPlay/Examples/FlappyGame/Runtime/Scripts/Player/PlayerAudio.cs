using SolPlay.FlappyGame.Runtime.Scripts.Audio;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Player
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] AudioClip _flapAudio;
        [SerializeField] AudioClip _dieAudio;
        [SerializeField] AudioClip _hitGroundAudio;

        public void OnFlap() => AudioUtility.PlaySFX(_flapAudio);
        public void OnDie() => AudioUtility.PlaySFX(_dieAudio);
        public void OnHitGround() => AudioUtility.PlaySFX(_hitGroundAudio);
    }
}
