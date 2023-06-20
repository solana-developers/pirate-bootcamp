using Frictionless;
using SolPlay.FlappyGame.Runtime.Scripts.Core;
using SolPlay.Scripts.Services;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.UI.Screens
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] GameMode _gameMode;

        public GameObject NftSelectedRoot;
        public GameObject NftUnselectedRoot;
    
        public void StartGame() => _gameMode.StartWaiting();

        private void Start()
        {
            UpdateContent();
            MessageRouter.AddHandler<NftSelectedMessage>(OnNftSelectedMessage);
            MessageRouter.AddHandler<NftLoadingFinishedMessage>(OnNftLoadingFinishedMessage);
        }

        private void OnNftLoadingFinishedMessage(NftLoadingFinishedMessage message)
        {
            UpdateContent();
        }

        private void OnNftSelectedMessage(NftSelectedMessage message)
        {
            UpdateContent();
        }
    
        private void UpdateContent()
        {
            var nftService = ServiceFactory.Resolve<NftService>();
            NftSelectedRoot.gameObject.SetActive(nftService.SelectedNft != null);
            NftUnselectedRoot.gameObject.SetActive(nftService.SelectedNft == null);
        }
    }
}
