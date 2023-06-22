using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Audio
{
    public class AudioHandler : MonoBehaviour
    {
        [SerializeField] AudioSource _mainAudio;
        [SerializeField] AudioSource _sfxAudio;

        private void Awake() 
        {
            _mainAudio.loop = true;
            _sfxAudio.loop = false;
        }

        public void PlayMusic(AudioClip clip)
        {
            _mainAudio.clip = clip;
            _mainAudio.Play();
        } 
    
        public void PlaySFX(AudioClip clip) => _sfxAudio.PlayOneShot(clip);
    }
}
