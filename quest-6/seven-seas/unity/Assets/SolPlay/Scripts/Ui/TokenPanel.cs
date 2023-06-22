using Frictionless;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using SolPlay.Scripts.Services;
using TMPro;
using UnityEngine;

namespace SolPlay.Scripts.Ui
{
    /// <summary>
    /// Shows the amount of the token "TokenMintAddress" from the connected Wallet.
    /// TODO: Add socket connection for constant updates 
    /// </summary>
    public class TokenPanel : MonoBehaviour
    {
        public TextMeshProUGUI TokenAmount;

        public string
            TokenMintAdress =
                "PLAyKbtrwQWgWkpsEaMHPMeDLDourWEWVrx824kQN8P"; // Solplay Token, replace with whatever token you like.

        private PublicKey _associatedTokenAddress;
        
        void Start()
        {
            MessageRouter.AddHandler<WalletLoggedInMessage>(OnWalletLoggedInMessage);
            MessageRouter.AddHandler<SocketServerConnectedMessage>(OnSocketConnectedMessage);
            MessageRouter.AddHandler<TokenValueChangedMessage>(OnTokenValueChangedMessage);
            if (ServiceFactory.Resolve<WalletHolderService>().IsLoggedIn)
            {
                UpdateTokenAmount();
            }
        }

        private void OnSocketConnectedMessage(SocketServerConnectedMessage obj)
        {
            // TODO: instead of only updating the token on the Token value updated message it would be better to subscribe to the 
            // associated token account for live updates. 
            /*
            ServiceFactory.Resolve<SolPlayWebSocketService>().SubscribeToPubKeyData(new PublicKey("CsEhX6bJDwaPZ9on7fFw6rdK44mRpeiVwe2oZ3uBfk8j"),
                result =>
                {
                    Debug.Log(result);
                });*/
        }

        private void OnTokenValueChangedMessage(TokenValueChangedMessage message)
        {
            UpdateTokenAmount();
        }

        private void OnWalletLoggedInMessage(WalletLoggedInMessage message)
        {
            UpdateTokenAmount();
        }

        private async void UpdateTokenAmount()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            if (!walletHolderService.IsLoggedIn)
            {
                return;
            } 
            var wallet = walletHolderService.BaseWallet;

            if (wallet != null && wallet.Account.PublicKey != null)
            {
                _associatedTokenAddress =
                    AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(wallet.Account.PublicKey, new PublicKey(TokenMintAdress));
            }
            
            if (_associatedTokenAddress == null)
            {
                return;
            }
            
            var tokenBalance = await wallet.ActiveRpcClient.GetTokenAccountBalanceAsync(_associatedTokenAddress, Commitment.Confirmed);
            if (tokenBalance.Result == null || tokenBalance.Result.Value == null)
            {
                TokenAmount.text = "0";
                return;
            }
            TokenAmount.text = tokenBalance.Result.Value.UiAmountString;
        }
    }
}