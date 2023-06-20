using System;
using System.Collections.Generic;
using Frictionless;
using SolPlay.FlappyGame.Runtime.Scripts.Core;
using SolPlay.FlappyGame.Runtime.Scripts.Player;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Util
{
    [ExecuteAlways]
    public class PlayerFollow : MonoBehaviour
    {
        [SerializeField] float _xOffset;
        [SerializeField] Transform _objectToFollow;
        [SerializeField] PlayerController _playerController;

        public List<GameObject> Backgrounds;
        public List<GameObject> WaterBackgrounds;
    
        private void Start() 
        {
            if(_objectToFollow == null)
                enabled = false;

            MessageRouter.AddHandler<ScoreChangedMessage>(OnScoreChangedMessage);
        }

        private void OnScoreChangedMessage(ScoreChangedMessage message)
        {
            UpdateBackgrounds(message.NewScore);
        }

        public void UpdateBackgrounds(int score)
        {
            foreach (var bg in Backgrounds)
            {
                bg.gameObject.SetActive(false);
            }
            foreach (var bg in WaterBackgrounds)
            {
                bg.gameObject.SetActive(false);
            }
        
            switch (_playerController.Type)
            {
                case PlayerController.NftType.Water:
                    UpdateWaterBackgrounds(score);
                    break;
                case PlayerController.NftType.Default:
                    DefaultBackgrounds(score);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    
        private void DefaultBackgrounds(int score)
        {
            Backgrounds[0].gameObject.SetActive(false);
            Backgrounds[1].gameObject.SetActive(false);
            Backgrounds[2].gameObject.SetActive(false);
            Backgrounds[3].gameObject.SetActive(false);
            Backgrounds[4].gameObject.SetActive(false);

            if (score <= 10)
            {
                Backgrounds[0].gameObject.SetActive(true);
            }
            else if (score > 10 && score < 25)
            {
                Backgrounds[1].gameObject.SetActive(true);
            }
            else if (score > 25 && score < 50)
            {
                Backgrounds[2].gameObject.SetActive(true);
            }
            else if (score > 50 && score < 100)
            {
                Backgrounds[3].gameObject.SetActive(true);
            }
            else if (score > 100)
            {
                Backgrounds[4].gameObject.SetActive(true);
            }
        }
    
        private void UpdateWaterBackgrounds(int score)
        {
            WaterBackgrounds[0].gameObject.SetActive(true);
        }

        void LateUpdate()
        {
            Vector3 target = transform.position;
            target.x = _objectToFollow.position.x + _xOffset;
            transform.position = target;
        }
    }
}
