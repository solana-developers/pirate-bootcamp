using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Audio
{
    public class AudioFX : MonoBehaviour
    {
        [SerializeField] AudioClip _clip;

        public void PlayAudio() => AudioUtility.PlaySFX(_clip);
    }
}
