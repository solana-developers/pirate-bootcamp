using Frictionless;
using SolPlay.Scripts.Services;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SolPlay.Scripts.Ui
{
    /// <summary>
    /// Screen will enable the connected root as soon as the wallet is logged in
    /// </summary>
    public class LoginScreen : MonoBehaviour
    {
        [FormerlySerializedAs("PhantomLoginButton")] public Button DevnetLoginButton;
        public Button DevnetInGameWalletButton;
        public Button LocalhostInGameWalletButton;
        public Button MainNetButton;
        public GameObject ConnectedRoot;
        public GameObject NotConnectedRoot;
        public GameObject TabBarRoot;
        public GameObject LoadingRoot;

        private void Awake()
        {
            if (DevnetLoginButton)
            {
                DevnetLoginButton.onClick.AddListener(OnPhantomButtonClicked);    
            }

            if (DevnetInGameWalletButton)
            {
                DevnetInGameWalletButton.onClick.AddListener(OnDevnetInGameWalletButtonClicked);   
            }
            if (LocalhostInGameWalletButton)
            {
                LocalhostInGameWalletButton.onClick.AddListener(OnLocalhostInGameWalletButtonClicked);
            }            
            if (MainNetButton)
            {
                MainNetButton.onClick.AddListener(OnMainnetButtonClicked);
            }
            SetLoadingRoot(false);
        }

        private void Start()
        {
            UpdateContent();
        }

        private async void OnDevnetInGameWalletButtonClicked()
        {
            SetLoadingRoot(true);
            await ServiceFactory.Resolve<WalletHolderService>().Login(WalletHolderService.Network.Devnet, true);
            UpdateContent();
        }

        private async void OnLocalhostInGameWalletButtonClicked()
        {
            SetLoadingRoot(true);
            await ServiceFactory.Resolve<WalletHolderService>().Login(WalletHolderService.Network.LocalNet, true);
            UpdateContent();
        }

        private async void OnPhantomButtonClicked()
        {
            SetLoadingRoot(true);
            await ServiceFactory.Resolve<WalletHolderService>().Login(WalletHolderService.Network.Devnet, false);
            UpdateContent();
        }

        private async void OnMainnetButtonClicked()
        {
            SetLoadingRoot(true);
            await ServiceFactory.Resolve<WalletHolderService>().Login(WalletHolderService.Network.Mainnet, false);
            UpdateContent();
        }

        private void UpdateContent()
        {
            SetLoadingRoot(false);
            bool isLoggedIn = ServiceFactory.Resolve<WalletHolderService>().IsLoggedIn;
            ConnectedRoot.gameObject.SetActive(isLoggedIn);
            NotConnectedRoot.gameObject.SetActive(!isLoggedIn);
            if (TabBarRoot != null)
            {
                TabBarRoot.gameObject.SetActive(isLoggedIn);
            }
        }

        private void SetLoadingRoot(bool active)
        {
            if (LoadingRoot != null)
            {
                LoadingRoot.SetActive(active);
            }
        }
    }
}
