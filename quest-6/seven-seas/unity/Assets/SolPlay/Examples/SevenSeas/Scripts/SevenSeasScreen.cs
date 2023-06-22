using Frictionless;
using NativeWebSocket;
using SolHunter;
using SevenSeas.Accounts;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK.Nft;
using SolPlay.Examples.SevenSeas.Scripts;
using SolPlay.Scripts.Services;
using SolPlay.Scripts.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TinyAdventure
{
    public class SevenSeasScreen : MonoBehaviour
    {
        public Button GetDataButton;
        public Button MoveRightButton;
        public Button MoveLeftButton;   
        public Button MoveUpButton;
        public Button BoomButton;
        public Button MoveDownButton;
        public Button InitializeButton;
        public Button ResetButton;
        public Button SpawnPlayerAndChestButton;
        public Button PickAvatarButton;
        public Button OpenInGameWalletPopup;
        public Button OpenNftScreenButton;
        public Button UpgradeButton;
        public Button InitShipButton;
        public Button ChutuluhButton;
        public Button StartThreadButton;
        public Button PauseThreadButton;
        public Button ResumeThreadButton;
        public Button AdminButton;
        public Button ResetShipButton;
        public TextMeshProUGUI ShipLevel;
        public TextMeshProUGUI UpgradeButtonText;

        public GameObject NoSelectedNftRoot;
        public GameObject NoPlayerSpawnedRoot;
        public GameObject GameRunningRoot;
        public GameObject UpgradeShipRoot;
        public GameObject AdminRoot;

        public SolHunterTile TilePrefab;
        public GameObject TilesRoot;
        public SolHunterTile[,] Tiles = new SolHunterTile[10, 10];
        public NftItemView AvatarNftItemView;
        public NftItemView GameAvatarNftItemView;
        
        private void Awake()
        {
            GetDataButton.onClick.AddListener(OnGetDataButtonClicked);
            MoveRightButton.onClick.AddListener(OnMoveRightButtonClicked);
            MoveLeftButton.onClick.AddListener(OnMoveLeftButtonClicked);
            MoveUpButton.onClick.AddListener(OnMoveUpButtonClicked);
            BoomButton.onClick.AddListener(OnBoomButtonClicked);
            ChutuluhButton.onClick.AddListener(OnChutuluhButtonClicked);
            StartThreadButton.onClick.AddListener(OnStartThreadButtonClicked);
            PauseThreadButton.onClick.AddListener(OnPauseThreadButtonClicked);
            ResumeThreadButton.onClick.AddListener(OnResumeThreadButtonClicked);
            MoveDownButton.onClick.AddListener(OnMoveDownButtonClicked);
            InitializeButton.onClick.AddListener(OnInitializeButtonClicked);
            ResetButton.onClick.AddListener(OnResetButtonClicked);
            SpawnPlayerAndChestButton.onClick.AddListener(OnSpawnPlayerButtonClicked);
            PickAvatarButton.onClick.AddListener(OnPickAvatarButtonClicked);
            OpenNftScreenButton.onClick.AddListener(OnPickAvatarButtonClicked);
            OpenInGameWalletPopup.onClick.AddListener(OnInGameWalletButtonClicked);
            UpgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
            InitShipButton.onClick.AddListener(OnInitShipButtonClicked);
            ResetShipButton.onClick.AddListener(OnResetShipButtonClicked);
            // We currently dont need the init ship button. It will be initialized with the first deploy ship
            // Like this the player will have less choices to make
            InitShipButton.gameObject.SetActive(false);
            UpgradeShipRoot.gameObject.SetActive(false);
            AdminButton.onClick.AddListener(OnAdminButtonClicked);
            AdminRoot.gameObject.SetActive(false);
        }

        private void OnAdminButtonClicked()
        {
            AdminRoot.gameObject.SetActive(!AdminRoot.gameObject.activeSelf);
        }

        private void Start()
        {
            MessageRouter.AddHandler<SevenSeasService.SolHunterGameDataChangedMessage>(OnGameDataChangedMessage);
            MessageRouter.AddHandler<SevenSeasService.SolHunterShipDataChangedMessage>(OnShipDataChangedMessage);
            MessageRouter.AddHandler<NftSelectedMessage>(OnNftSelectedMessage);
            MessageRouter.AddHandler<NftLoadedMessage>(OnNftJsonLoadedMessage);
            MessageRouter.AddHandler<NftLoadingFinishedMessage>(OnNftLoadingFinishedMessage);
            MessageRouter.AddHandler<WalletLoggedInMessage>(OnWalletLoggedInMessage);

            for (int x = 0; x < SevenSeasService.TILE_COUNT_X; x++)
            {
                for (int y = 0; y < SevenSeasService.TILE_COUNT_Y; y++)
                {
                    SolHunterTile solHunterTile = Instantiate(TilePrefab.gameObject, TilesRoot.transform)
                        .GetComponent<SolHunterTile>();
                    Tiles[x, y] = solHunterTile;
                }
            }

            UpdateContent();
        }

        private void OnShipDataChangedMessage(SevenSeasService.SolHunterShipDataChangedMessage message)
        {
            UpdateContent();
        }

        private async void OnWalletLoggedInMessage(WalletLoggedInMessage message)
        {
            var res = await ServiceFactory.Resolve<SevenSeasService>().GetGameData();
            if (res == null)
            {
                InitializeButton.gameObject.SetActive(true);
            }
        }

        private void OnNftLoadingFinishedMessage(NftLoadingFinishedMessage message)
        {
            UpdateContent();
        }

        private void OnNftJsonLoadedMessage(NftLoadedMessage message)
        {
            var solHunterService = ServiceFactory.Resolve<SevenSeasService>();
            var nftService = ServiceFactory.Resolve<NftService>();
            var playerAvatar = solHunterService.TryGetSpawnedPlayerAvatar();
            Nft solPlayNft = nftService.GetNftByMintAddress(playerAvatar);
            if (solPlayNft != null && message.Nft == solPlayNft)
            {
                UpdateContent();
            }
        }

        private async void UpdateContent()
        {
            if (ServiceFactory.Resolve<SolPlayWebSocketService>().GetState() != WebSocketState.Open)
            {
                GameRunningRoot.gameObject.SetActive(false);
                NoPlayerSpawnedRoot.gameObject.SetActive(false);
                NoSelectedNftRoot.gameObject.SetActive(false);
                return;
            }

            var sevenSeasService = ServiceFactory.Resolve<SevenSeasService>();
            UpgradeShipRoot.gameObject.SetActive(sevenSeasService.CurrentShipData != null);
            UpgradeButton.gameObject.SetActive(sevenSeasService.CurrentShipData != null);
            InitShipButton.gameObject.SetActive(sevenSeasService.CurrentShipData == null);
            if (sevenSeasService.CurrentShipData != null)
            {
                ShipLevel.text = "Ship Level: " + sevenSeasService.CurrentShipData.Upgrades;
                var shipUpgradeCost = sevenSeasService.GetShipUpgradeCost(sevenSeasService.CurrentShipData.Upgrades);
                UpgradeButtonText.text = "Upgrade: " + shipUpgradeCost;
                var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
                if (!walletHolderService.IsLoggedIn)
                {
                    return;
                } 
                var wallet = walletHolderService.BaseWallet;
                if (wallet != null && wallet.Account.PublicKey != null)
                {
                    var _associatedTokenAddress =
                        AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(wallet.Account.PublicKey, SevenSeasService.GoldTokenMint);
                    var tokenBalance = await wallet.ActiveRpcClient.GetTokenAccountBalanceAsync(_associatedTokenAddress, Commitment.Confirmed);
                    if (tokenBalance.Result != null)
                    {
                        UpgradeButton.interactable = shipUpgradeCost * Mathf.Pow(10, tokenBalance.Result.Value.Decimals) <= tokenBalance.Result.Value.AmountUlong;
                    }
                }
            }
            else
            {
                ShipLevel.text = "Ship Level: " + 1;
                UpgradeButtonText.text = "Upgrade: 5" + sevenSeasService.GetShipUpgradeCost(0);
            }

            if (sevenSeasService.IsPlayerSpawned())
            {
                SetNftGraphic();
                GameRunningRoot.gameObject.SetActive(true);
                NoPlayerSpawnedRoot.gameObject.SetActive(false);
                NoSelectedNftRoot.gameObject.SetActive(false);
                return;
            }

            var selectedNft = ServiceFactory.Resolve<NftService>().SelectedNft;
            if (selectedNft == null)
            {
                GameRunningRoot.gameObject.SetActive(false);
                NoPlayerSpawnedRoot.gameObject.SetActive(false);
                NoSelectedNftRoot.gameObject.SetActive(true);
                return;
            }

            if (!sevenSeasService.IsPlayerSpawned())
            {
                GameRunningRoot.gameObject.SetActive(false);
                NoPlayerSpawnedRoot.gameObject.SetActive(true);
                NoSelectedNftRoot.gameObject.SetActive(false);
                return;
            }

            NoPlayerSpawnedRoot.gameObject.SetActive(false);
            NoSelectedNftRoot.gameObject.SetActive(false);
            GameRunningRoot.gameObject.SetActive(true);
            SetNftGraphic();
        }

        private void OnInGameWalletButtonClicked()
        {
            ServiceFactory.Resolve<UiService>()
                .OpenPopup(UiService.ScreenType.InGameWalletPopup, new InGameWalletPopupUiData(0));
        }


        private async void OnUpgradeButtonClicked()
        {
            var ship = await ServiceFactory.Resolve<SevenSeasService>().GetShipData();

            if (ship != null)
            {
                ServiceFactory.Resolve<SevenSeasService>().UpgradeShip();
                Debug.Log("Ship level " + ship.Level);    
            }
            else
            {
                Debug.Log("Player has no ship yet");
                ServiceFactory.Resolve<SevenSeasService>().InitShip();
            }
        }
        
        private void OnInitShipButtonClicked()
        {
            ServiceFactory.Resolve<SevenSeasService>().InitShip();
        }
        
        private async void OnNftSelectedMessage(NftSelectedMessage message)
        {
            SetNftGraphic();
            UpdateContent();
            await ServiceFactory.Resolve<SevenSeasService>().GetShipData();
        }

        private void SetNftGraphic()
        {
            var solHunterService = ServiceFactory.Resolve<SevenSeasService>();
            var nftService = ServiceFactory.Resolve<NftService>();

            var playerAvatar = solHunterService.TryGetSpawnedPlayerAvatar();
            Nft solPlayNft = nftService.GetNftByMintAddress(playerAvatar);
            if (solPlayNft == null)
            {
                solPlayNft = nftService.SelectedNft;
            }

            if (solPlayNft == null)
            {
                if (playerAvatar == null)
                {
                    Debug.Log("Players doesnt have the nft anymore. And avatar is null");
                }
                else
                {
                    Debug.Log("Players doesnt have the nft anymore:" + playerAvatar);
                }
                
                return;
            }

            AvatarNftItemView.gameObject.SetActive(true);
            AvatarNftItemView.SetData(solPlayNft, view =>
            {
                // Nothing on click
            });
            GameAvatarNftItemView.SetData(solPlayNft, view =>
            {
                // Nothing on click
            });
        }

        private void OnPickAvatarButtonClicked()
        {
            var baseWallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;
            ServiceFactory.Resolve<UiService>()
                .OpenPopup(UiService.ScreenType.NftListPopup, new NftListPopupUiData(false, baseWallet));
        }

        private void OnGameDataChangedMessage(SevenSeasService.SolHunterGameDataChangedMessage message)
        {
            UpdateGameDataView(message.GameDataAccount);
            UpdateContent();
        }

        private void OnInitializeButtonClicked()
        {
            ServiceFactory.Resolve<SevenSeasService>().Initialize();
        }

        private void OnResetButtonClicked()
        {
            ServiceFactory.Resolve<SevenSeasService>().Reset();
        }

        private void OnResetShipButtonClicked()
        {
            ServiceFactory.Resolve<SevenSeasService>().ResetShip();
        }
        
        private void OnSpawnPlayerButtonClicked()
        {
            ServiceFactory.Resolve<SevenSeasService>().SpawnPlayerAndChest();
        }

        private void OnMoveUpButtonClicked()
        {
            ServiceFactory.Resolve<SevenSeasService>().Move(SevenSeasService.Direction.Up);
        }

        private void OnBoomButtonClicked()
        {
            ServiceFactory.Resolve<SevenSeasService>().Shoot();
        }

        private void OnChutuluhButtonClicked()
        {
            ServiceFactory.Resolve<SevenSeasService>().Chutuluh();
        }

        private void OnStartThreadButtonClicked()
        {
            ServiceFactory.Resolve<ClockWorkService>().StartThread();
        }
        private void OnPauseThreadButtonClicked()
        {
            ServiceFactory.Resolve<ClockWorkService>().PauseThread();
        }
        private void OnResumeThreadButtonClicked()
        {
            ServiceFactory.Resolve<ClockWorkService>().ResumeThread();
        }
        
        private void OnMoveRightButtonClicked()
        {
            ServiceFactory.Resolve<SevenSeasService>().Move(SevenSeasService.Direction.Right);
        }

        private void OnMoveDownButtonClicked()
        {
            ServiceFactory.Resolve<SevenSeasService>().Move(SevenSeasService.Direction.Down);
        }

        private void OnMoveLeftButtonClicked()
        {
            ServiceFactory.Resolve<SevenSeasService>().Move(SevenSeasService.Direction.Left);
        }

        private async void OnGetDataButtonClicked()
        {
            GameDataAccount gameData = await ServiceFactory.Resolve<SevenSeasService>().GetGameData();
            UpdateGameDataView(gameData);
        }

        private void UpdateGameDataView(GameDataAccount gameData)
        {
            var length = gameData.Board.GetLength(0);
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    var tile = gameData.Board[y][x];
                    Tiles[x, y].SetData(tile);
                }
            }
        }
    }
}