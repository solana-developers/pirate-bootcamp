using System;
using Frictionless;
using Solana.Unity.Programs;
using Solana.Unity.SDK.Nft;
using Solana.Unity.Wallet;
using SolPlay.Orca;
using SolPlay.Scripts.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolPlay.Scripts.Ui
{
    /// <summary>
    /// When clicking a Nft this context menu opens and shows some information about the Nft
    /// </summary>
    public class NftContextMenu : MonoBehaviour
    {
        public GameObject Root;
        public Button CloseButton;
        public TextMeshProUGUI NftNameText;
        public TextMeshProUGUI PowerLevelText;
        public Button SelectButton;
        public Button TransferButton;
        public Nft currentNft;
        private Action<Nft> onNftSelected;

        private void Awake()
        {
            ServiceFactory.RegisterSingleton(this);
            Root.gameObject.SetActive(false);
            CloseButton.onClick.AddListener(OnCloseButtonClicked);
            SelectButton.onClick.AddListener(OnSelectClicked);
            TransferButton.onClick.AddListener(OnTransferClicked);
        }

        private void OnTransferClicked()
        {
            ServiceFactory.Resolve<UiService>().OpenPopup(UiService.ScreenType.TransferNftPopup, new TransferNftPopupUiData(currentNft));
            Close();
        }

        private async void OnSelectClicked()
        {
            ServiceFactory.Resolve<NftService>().SelectNft(currentNft);
            MessageRouter.RaiseMessage(
                new BlimpSystem.ShowLogMessage($"{currentNft.metaplexData.data.offchainData.name} selected"));
            Close();
            var tabBarComponent = ServiceFactory.Resolve<TabBarComponent>();
            if (tabBarComponent != null)
            {
                tabBarComponent.HorizontalScrollSnap.ChangePage(1);
            }
            else
            {
                
                // In case you want to load another scene please use the SolPlay instance
                // SolPlay.Instance.LoadScene("FlappyGameExample");
                onNftSelected?.Invoke(currentNft);
            }
        }

        private void OnCloseButtonClicked()
        {
            Close();
        }

        private void Close()
        {
            Root.gameObject.SetActive(false);
        }

        public void Open(NftItemView nftItemView, Action<Nft> onNftSelected)
        {
            this.onNftSelected = onNftSelected;
            currentNft = nftItemView.CurrentSolPlayNft;
            Root.gameObject.SetActive(true);
            NftNameText.text = nftItemView.CurrentSolPlayNft.metaplexData.data.offchainData.name;
            transform.position = nftItemView.transform.position;
            var powerLevelService = ServiceFactory.Resolve<HighscoreService>();
            if (powerLevelService.TryGetHighscoreForSeed(nftItemView.CurrentSolPlayNft.metaplexData.data.mint, out HighscoreEntry highscoreEntry))
            {
                PowerLevelText.text = $"High score: {highscoreEntry.Highscore}";
            }
        }
    }
}