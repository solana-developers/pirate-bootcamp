using System.Collections;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts
{
    public class CameraShake : MonoBehaviour {

        public static CameraShake instance;

        private Vector3 _originalPos;
        private float _timeAtCurrentFrame;
        private float _timeAtLastFrame;
        private float _fakeDelta;
    
        public float CameraSize = 10.65f;
        public float Minimum = 5;
        public float FixedOffset = 0;

        private Camera Camera;
        private Vector3 StartPosition;
        
        void Awake()
        {
            instance = this;
            instance._originalPos = instance.gameObject.transform.localPosition;
            Camera = GetComponentInChildren<Camera>();
            StartPosition = Camera.gameObject.transform.position;
        }

        void Update()
        {
            // Calculate a fake delta time, so we can Shake while game is paused.
            _timeAtCurrentFrame = Time.realtimeSinceStartup;
            _fakeDelta = _timeAtCurrentFrame - _timeAtLastFrame;
            _timeAtLastFrame = _timeAtCurrentFrame; 

            if (Screen.width < Screen.height)
            {
                Camera.gameObject.transform.position = StartPosition + new Vector3(0, 20, 0);
            }
            else
            {
                Camera.gameObject.transform.position = StartPosition;
            }
            //Camera.orthographicSize = CameraSize / Screen.width * Screen.height;
            //var orthographicSize = ((CameraSize / 1000) * Screen.width);        
            //Camera.orthographicSize = Mathf.Max(Minimum, orthographicSize);
            
            //Camera.fieldOfView = 1 / (Screen.width / FixedOffset);
        }

        public static void Shake (float duration, float amount) {
           // instance.StopAllCoroutines();
            instance.StartCoroutine(instance.cShake(duration, amount));
        }

        public IEnumerator cShake (float duration, float amount) {
            float endTime = Time.time + duration;

            while (duration > 0) {
                transform.localPosition = _originalPos + Random.insideUnitSphere * amount;

                duration -= _fakeDelta;

                yield return null;
            }

            transform.localPosition = _originalPos;
        }
    }
}