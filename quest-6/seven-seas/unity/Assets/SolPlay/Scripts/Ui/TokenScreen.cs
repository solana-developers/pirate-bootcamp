using Frictionless;
using Solana.Unity.Wallet;
using SolPlay.DeeplinksNftExample.Scripts;
using SolPlay.DeeplinksNftExample.Utils;
using SolPlay.Orca;
using SolPlay.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace SolPlay.Scripts.Ui
{
    public class TokenScreen : MonoBehaviour
    {
        public Button GetSolPlayTokenButton;
        public Button TransferSolButton;
        public Button TokenTransactionButton;

        void Awake()
        {
            GetSolPlayTokenButton.onClick.AddListener(OnGetSolPlayTokenButtonClicked);
            TransferSolButton.onClick.AddListener(OnTransferSolButtonClicked);
            TokenTransactionButton.onClick.AddListener(OnTokenTransactionButtonClicked);
        }

        private async void OnTokenTransactionButtonClicked()
        {
            Token usdcToken = ServiceFactory.Resolve<OrcaWhirlpoolService>()
                .GetToken(new PublicKey("EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v"));
            ServiceFactory.Resolve<UiService>().OpenPopup(UiService.ScreenType.TransferTokenPopup, new TransferTokenPopupUiData(usdcToken));
        }

        private async void OnTransferSolButtonClicked()
        {
            Token wrappedSolToken = ServiceFactory.Resolve<OrcaWhirlpoolService>()
                .GetToken(new PublicKey("So11111111111111111111111111111111111111112"));
            ServiceFactory.Resolve<UiService>().OpenPopup(UiService.ScreenType.TransferTokenPopup, new TransferTokenPopupUiData(wrappedSolToken));
        }

        private void OnGetSolPlayTokenButtonClicked()
        {
            // To let people buy a token just put the direct raydium link to your token and open it with a phantom deeplink. 
            PhantomUtils.OpenUrlInWalletBrowser(
                "https://raydium.io/swap/?inputCurrency=sol&outputCurrency=PLAyKbtrwQWgWkpsEaMHPMeDLDourWEWVrx824kQN8P&inputAmount=0.1&outputAmount=0.9&fixed=in");
        }
    }
}
