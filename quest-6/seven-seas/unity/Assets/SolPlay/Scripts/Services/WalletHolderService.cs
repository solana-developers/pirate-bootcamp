using System;
using System.Collections;
using System.Threading.Tasks;
using Frictionless;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using Solana.Unity.Wallet.Bip39;
using SolPlay.DeeplinksNftExample.Utils;
using SolPlay.Scripts.Ui;
using UnityEngine;

namespace SolPlay.Scripts.Services
{
    public enum WalletType { Phantom, Backpack }
    public class WalletLoggedInMessage
    {
        public WalletBase Wallet;
    }

    public class SolBalanceChangedMessage
    {
        public double SolBalanceChange;
        public bool IsInGameWallet;

        public SolBalanceChangedMessage(double solBalanceChange = 0, bool isInGameWallet = false)
        {
            SolBalanceChange = solBalanceChange;
            IsInGameWallet = isInGameWallet;
        }
    }
    
    public class WalletHolderService : MonoBehaviour, IMultiSceneSingleton
    {
        // These are the custom urls to connect to local host with socket
        //customRpc: http://localhost:8899/
        //webSocketsRpc: ws://localhost:8090/

        public string DevnetLoginRPCUrl = "";
        public string MainNetRpcUrl = "";

        public PhantomWalletOptions PhantomWalletOptions;
        public SolanaWalletAdapterOptions WebGlWalletOptions;

        [NonSerialized] public WalletBase BaseWallet;

        public bool IsLoggedIn { get; private set; }
        public bool AutomaticallyConnectWebSocket = true;
        public long BaseWalletSolBalance;
        public long InGameWalletSolBalance;

        public SolanaWalletAdapter DeeplinkWallet;

        public InGameWallet InGameWallet;
        public bool TwoWalletSetup;

        private void Awake()
        {
            if (ServiceFactory.Resolve<WalletHolderService>() != null)
            {
                Destroy(gameObject);
                return;
            }

            ServiceFactory.RegisterSingleton(this);
        }

        public enum Network
        {
            Mainnet,
            Devnet,
            LocalNet
        }

        public async Task<Account> Login(Network devNetLogin, bool singleWalletSetup)
        {
            string rpcUrl = null;
            RpcCluster cluster = RpcCluster.DevNet;

            switch (devNetLogin)
            {
                case Network.Mainnet:
                    rpcUrl = MainNetRpcUrl;
                    cluster = RpcCluster.MainNet;
                    break;
                case Network.Devnet:
                    rpcUrl = DevnetLoginRPCUrl;
                    cluster = RpcCluster.DevNet; 
                    break;
                case Network.LocalNet:
                    rpcUrl = "http://localhost:8899/";
                    cluster = RpcCluster.DevNet; 
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(devNetLogin), devNetLogin, null);
            }

            if (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                DeeplinkWallet = new SolanaWalletAdapter(WebGlWalletOptions, cluster, rpcUrl, null, true);
            }

            InGameWallet = new InGameWallet(cluster, rpcUrl, null, true);

            TwoWalletSetup = singleWalletSetup;

            var newMnemonic = new Mnemonic(WordList.English, WordCount.Twelve);

            var account = await InGameWallet.Login("1234") ??
                          await InGameWallet.CreateAccount(newMnemonic.ToString(), "1234");

            if (singleWalletSetup)
            {
                BaseWallet = InGameWallet;

                // Copy this private key if you want to import your wallet into phantom. Dont share it with anyone.
                // var privateKeyString = account.PrivateKey.Key;
                double sol = await BaseWallet.GetBalance();

                if (sol < 0.8)
                {
                    await RequestAirdrop();
                }
            }
            else
            {
                BaseWallet = DeeplinkWallet;
                await BaseWallet.Login();
            }

            IsLoggedIn = true;
            MessageRouter.RaiseMessage(new WalletLoggedInMessage()
            {
                Wallet = BaseWallet
            });

            if (AutomaticallyConnectWebSocket)
            {
                var solPlayWebSocketService = ServiceFactory.Resolve<SolPlayWebSocketService>();
                if (solPlayWebSocketService != null)
                { 
                    solPlayWebSocketService.Connect(BaseWallet.ActiveRpcClient.NodeAddress.ToString());
                }

                SubscribeToWalletAccountChanges();
            }

            var baseSolBalance = await BaseWallet.ActiveRpcClient.GetBalanceAsync(BaseWallet.Account.PublicKey, Commitment.Confirmed);
            if (baseSolBalance.Result != null)
            {
                BaseWalletSolBalance = (long) baseSolBalance.Result.Value;
                MessageRouter.RaiseMessage(new SolBalanceChangedMessage(BaseWalletSolBalance, false));
                Debug.Log("Logged in Base: " + BaseWallet.Account.PublicKey + " balance: " + baseSolBalance.Result.Value);
            }
            
            var ingameSolBalance = await InGameWallet.ActiveRpcClient.GetBalanceAsync(InGameWallet.Account.PublicKey, Commitment.Confirmed);
            if (ingameSolBalance.Result != null)
            {
                InGameWalletSolBalance = (long) ingameSolBalance.Result.Value;
                MessageRouter.RaiseMessage(new SolBalanceChangedMessage(InGameWalletSolBalance, true));
                Debug.Log("Logged in InGameWallet: " + InGameWallet.Account.PublicKey + " balance: " + ingameSolBalance.Result.Value);
            }

            return BaseWallet.Account;
        }

        private void SubscribeToWalletAccountChanges()
        {
            //ServiceFactory.Resolve<SolPlayWebSocketService>().SubscribeToBlocks();
            if (TwoWalletSetup)
            {
                ServiceFactory.Resolve<SolPlayWebSocketService>().SubscribeToPubKeyData(BaseWallet.Account.PublicKey,
                    result =>
                    {
                        long balanceChange = result.result.value.lamports - BaseWalletSolBalance;
                        BaseWalletSolBalance = result.result.value.lamports;
                        InGameWalletSolBalance = result.result.value.lamports;
                        MessageRouter.RaiseMessage(
                            new SolBalanceChangedMessage((float) balanceChange / SolanaUtils.SolToLamports, true));
                        MessageRouter.RaiseMessage(new SolBalanceChangedMessage((float) balanceChange / SolanaUtils.SolToLamports));
                    });
            }
            else
            {
                ServiceFactory.Resolve<SolPlayWebSocketService>().SubscribeToPubKeyData(BaseWallet.Account.PublicKey,
                    result =>
                    {
                        long balanceChange = result.result.value.lamports - BaseWalletSolBalance;
                        BaseWalletSolBalance = result.result.value.lamports;
                        MessageRouter.RaiseMessage(new SolBalanceChangedMessage((float) balanceChange / SolanaUtils.SolToLamports));
                    });
                ServiceFactory.Resolve<SolPlayWebSocketService>().SubscribeToPubKeyData(InGameWallet.Account.PublicKey,
                    result =>
                    {
                        long balanceChange = result.result.value.lamports - InGameWalletSolBalance;
                        InGameWalletSolBalance = result.result.value.lamports;
                        MessageRouter.RaiseMessage(new SolBalanceChangedMessage((float) balanceChange / SolanaUtils.SolToLamports,
                            true));
                    });
            }
        }

        public async Task RequestAirdrop()
        {
            MessageRouter.RaiseMessage(new BlimpSystem.ShowLogMessage("Requesting airdrop"));
            RequestResult<string> result = await BaseWallet.ActiveRpcClient.RequestAirdropAsync(BaseWallet.Account.PublicKey, SolanaUtils.SolToLamports, Commitment.Confirmed);
            if (result.WasSuccessful)
            {
                ServiceFactory.Resolve<TransactionService>().CheckSignatureStatus(result.Result, b => {});
            }
            else
            {
                MessageRouter.RaiseMessage(new BlimpSystem.ShowLogMessage("Airdrop failed: " + result.ErrorData));
            }
        }

        public bool TryGetPhantomPublicKey(out string phantomPublicKey)
        {
            if (BaseWallet.Account == null)
            {
                phantomPublicKey = string.Empty;
                return false;
            }

            phantomPublicKey = BaseWallet.Account.PublicKey;
            return true;
        }

        public IEnumerator HandleNewSceneLoaded()
        {
            yield return null;
        }

        public bool HasEnoughSol(bool inGameWallet, long requiredLamports)
        {
            Debug.Log($"Checking sol balance {inGameWallet} for {requiredLamports}");
            Debug.Log($"Ingame {InGameWalletSolBalance} Base Wallet {BaseWalletSolBalance}");
            bool hasEnoughSol = false;
            if (inGameWallet)
            {
                hasEnoughSol = InGameWalletSolBalance >= requiredLamports;
            }
            else
            {
                hasEnoughSol = BaseWalletSolBalance >= requiredLamports;
            }

            if (!hasEnoughSol)
            {
                ServiceFactory.Resolve<UiService>().OpenPopup(UiService.ScreenType.InGameWalletPopup,
                    new InGameWalletPopupUiData(requiredLamports));
            }

            return hasEnoughSol;
        }
    }
}