using System.Threading.Tasks;
using Frictionless;
using SolPlay.DeeplinksNftExample.Scripts;
using SolPlay.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace SolPlay.Scripts.Ui
{
    /// <summary>
    /// Screen that loads all NFTs when opened
    /// </summary>
    public class NftListPopup : BasePopup
    {
        public Button GetNFtsDataButton;
        public Button GetNFtsNotCachedButton;
        public Button GetBeaverButton;
        public Button MintInAppButton;
        public Button MintInApp3DButton;
        public NftItemListView NftItemListView;
        public GameObject YouDontOwnABeaverRoot;
        public GameObject YouOwnABeaverRoot;
        public GameObject LoadingSpinner;
        public GameObject MinitingBlocker;

        async void Start()
        {
            GetNFtsDataButton.onClick.AddListener(OnGetNftButtonClicked);
            GetNFtsNotCachedButton.onClick.AddListener(OnNFtsNotCachedButtonClicked);
            GetBeaverButton.onClick.AddListener(OnOpenMinitingUrlButtonClicked);
            MintInAppButton.onClick.AddListener(OnMintInAppButtonClicked);
            MintInApp3DButton.onClick.AddListener(OnMintInApp3DButtonClicked);

            MessageRouter
                .AddHandler<NftLoadingStartedMessage>(OnNftLoadingStartedMessage);
            MessageRouter
                .AddHandler<NftLoadingFinishedMessage>(OnNftLoadingFinishedMessage);
            MessageRouter
                .AddHandler<NftLoadedMessage>(OnNftLoadedMessage);
            MessageRouter
                .AddHandler<NftMintFinishedMessage>(OnNftMintFinishedMessage);
            MessageRouter
                .AddHandler<WalletLoggedInMessage>(OnWalletLoggedInMessage);
        }

        public override void Open(UiService.UiData uiData)
        {
            var nftListPopupUiData = (uiData as NftListPopupUiData);

            if (nftListPopupUiData == null)
            {
                Debug.LogError("Wrong ui data for nft list popup");
                return;
            }

            NftItemListView.UpdateContent();
            NftItemListView.SetData(nft =>
            {
                // when an nft was selected we want to close the popup so we can start the game.
                Close();
            });
            base.Open(uiData);
        }

        private async void OnWalletLoggedInMessage(WalletLoggedInMessage message)
        {
            await OnLogin();
        }

        private async Task OnLogin()
        {
            await RequestNfts(true);
        }

        private async void OnMintInApp3DButtonClicked()
        {
            if (MinitingBlocker != null)
            {
                MinitingBlocker.gameObject.SetActive(true);
            }

            var signature = await ServiceFactory.Resolve<NftMintingService>()
                .MintNftWithMetaData(
                    "https://arweave.net/x-NmscUWB6zzdROsLsX1-CfRVVlYcuBL2rQ5vk8Fslo",
                    "Alpha Racing", "Test", b =>
                    {
                        if (MinitingBlocker != null)
                        {
                            MinitingBlocker.gameObject.SetActive(false);
                        }
                    });
        }

        private async void OnMintInAppButtonClicked()
        {
            if (MinitingBlocker != null)
            {
                MinitingBlocker.gameObject.SetActive(true);
            }
            
            
            // Mint a pirate sship
            var signature = await ServiceFactory.Resolve<NftMintingService>()
                .MintNftWithMetaData(
                    "https://shdw-drive.genesysgo.net/QZNGUVnJgkw6sGQddwZVZkhyUWSUXAjXF9HQAjiVZ55/DummyPirateShipMetaData.json",
                    "Simple Pirate Ship", "Pirate", b =>
                    {
                        if (MinitingBlocker != null)
                        {
                            MinitingBlocker.gameObject.SetActive(false);
                        }
                    });
            
            // Mint a baloon beaver
           /* var signature = await ServiceFactory.Resolve<NftMintingService>()
                .MintNftWithMetaData(
                    "https://shdw-drive.genesysgo.net/2TvgCDMEcSGnfuSUZNHvKpHL9Z5hLn19YqvgeUpS6rSs/manifest.json",
                    "Balloon Beaver", "Beaver", b =>
                    {
                        if (MinitingBlocker != null)
                        {
                            MinitingBlocker.gameObject.SetActive(false);
                        }
                    });
*/
            // Mint a solandy
            /*LoggingService.Log("Start minting a 'SolAndy' nft", true);
            var signature = await ServiceFactory.Resolve<NftMintingService>().MintNftWithMetaData("https://shdw-drive.genesysgo.net/4JaYMUSY8f56dFzmdhuzE1QUqhkJYhsC6wZPaWg9Zx7f/manifest.json", "SolAndy", "SolPlay");
            ServiceFactory.Resolve<TransactionService>().CheckSignatureStatus(signature,
                () =>
                {
                    RequestNfts(true);
                    LoggingService.Log("Mint Successfull! Woop woop!", true);
                });*/

            // Mint from a candy machine (This one is from zen republic, i only used it for testing)
            //var signature = await ServiceFactory.Resolve<NftMintingService>()
            //    .MintNFTFromCandyMachineV2(new PublicKey("3eqPffoeSj7e2ZkyHJHyYPc7qm8rbGDZFwM9oYSW4Z5w"));
        }

        private void OnOpenMinitingUrlButtonClicked()
        {
            // Here you can just open the link to your minting page within phantom mobile browser
            PhantomUtils.OpenUrlInWalletBrowser("https://beavercrush.com");
        }

        private void OnNftLoadedMessage(NftLoadedMessage message)
        {
            NftItemListView.AddNFt(message.Nft);
            UpdateBeaverStatus();
        }

        private bool UpdateBeaverStatus()
        {
            var nftService = ServiceFactory.Resolve<NftService>();
            bool ownsBeaver = nftService.OwnsNftOfMintAuthority(NftService.BeaverNftMintAuthority);
            YouDontOwnABeaverRoot.gameObject.SetActive(!ownsBeaver);
            YouOwnABeaverRoot.gameObject.SetActive(ownsBeaver);
            return ownsBeaver;
        }

        private async void OnGetNftButtonClicked()
        {
            await RequestNfts(true);
        }

        private async void OnNFtsNotCachedButtonClicked()
        {
            await RequestNfts(false);
        }

        private void OnNftLoadingStartedMessage(NftLoadingStartedMessage message)
        {
            GetNFtsDataButton.interactable = false;
            GetNFtsNotCachedButton.interactable = false;
        }

        private void OnNftLoadingFinishedMessage(NftLoadingFinishedMessage message)
        {
            NftItemListView.UpdateContent();
        }

        private void OnNftMintFinishedMessage(NftMintFinishedMessage message)
        {
            RequestNfts(true);
        }

        private void Update()
        {
            var nftService = ServiceFactory.Resolve<NftService>();
            if (nftService != null)
            {
                GetNFtsDataButton.interactable = !nftService.IsLoadingTokenAccounts;
                GetNFtsNotCachedButton.interactable = !nftService.IsLoadingTokenAccounts;
                LoadingSpinner.gameObject.SetActive(nftService.IsLoadingTokenAccounts);
            }
        }

        private async Task RequestNfts(bool tryUseLocalCache)
        {
            ServiceFactory.Resolve<NftService>().LoadNfts();
        }
    }
}