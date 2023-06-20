using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Frictionless;
using Solana.Unity.Metaplex.NFT.Library;
using Solana.Unity.Metaplex.Utilities.Json;
using Solana.Unity.Programs;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Messages;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK;
using Solana.Unity.SDK.Nft;
using Solana.Unity.Wallet;
using UnityEngine;

namespace SolPlay.Scripts.Services
{
    /// <summary>
    /// Handles all logic related to NFTs and calculating their power level or whatever you like to do with the NFTs
    /// </summary>
    public class NftService : MonoBehaviour, IMultiSceneSingleton
    {
        public List<Nft> MetaPlexNFts = new List<Nft>();
        public int NftImageSize = 75;
        public float RateLimitTimeBetweenNftLoads = 0.1f;
        public bool IsLoadingTokenAccounts { get; private set; }
        public const string BeaverNftMintAuthority = "GsfNSuZFrT2r4xzSndnCSs9tTXwt47etPqU8yFVnDcXd";
        public Nft SelectedNft { get; private set; }
        public Texture2D LocalDummyNft;
        public bool LoadNftsOnStartUp = true;
        public bool AddDummyNft = true;

        private const string IgnoredTokenListPlayerPrefsKey = "IgnoredTokenList1";

        public void Awake()
        {
            if (ServiceFactory.Resolve<NftService>() != null)
            {
                Destroy(gameObject);
                return;
            }

            ServiceFactory.RegisterSingleton(this);
        }

        private async void Start()
        {
            if (LoadNftsOnStartUp)
            {
                if (ServiceFactory.Resolve<WalletHolderService>().IsLoggedIn)
                {
                    LoadNfts();
                }
                else
                {
                    MessageRouter.AddHandler<WalletLoggedInMessage>(OnWalletLoggedInMessage);
                }
            }
        }

        public void LoadNfts()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            MetaPlexNFts.Clear();
            object privateFieldValue = walletHolderService.BaseWallet.GetType().BaseType
                .GetField("CustomRpcUri", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(walletHolderService.BaseWallet);
            Web3.Instance.customRpc = privateFieldValue.ToString();
            Debug.Log(privateFieldValue);
            Web3.Instance.WalletBase = walletHolderService.BaseWallet;
            Web3.OnNFTsUpdate += nfts =>
            {
                foreach (var newNft in nfts)
                {
                    bool wasAlreadyLoaded = false;
                    foreach (var oldNft in MetaPlexNFts)
                    {
                        if (newNft.metaplexData.data.mint == oldNft.metaplexData.data.mint)
                        {
                            wasAlreadyLoaded = true;
                        }
                    }

                    if (!wasAlreadyLoaded)
                    {
                        MessageRouter.RaiseMessage(new NftLoadedMessage(newNft));
                        MetaPlexNFts.Add(newNft);
                    }
                }
            };
            if (AddDummyNft)
            {
                var dummyLocalNft = CreateDummyLocalNft(walletHolderService.InGameWallet.Account.PublicKey);
                MetaPlexNFts.Add(dummyLocalNft);
                MessageRouter.RaiseMessage(new NftLoadedMessage(dummyLocalNft));
            }
        }

        private async void OnWalletLoggedInMessage(WalletLoggedInMessage message)
        {
            //await RequestNfts(message.Wallet);
            LoadNfts();
        }

        public Nft CreateDummyLocalNft(string publicKey)
        {
            Nft dummyLocalNft = new Nft();
            //MetadataAccount metaPlexData =  Activator.CreateInstance(typeof(MetadataAccount), true);
            
            var constructor = typeof(MetadataAccount).GetConstructor(BindingFlags.NonPublic|BindingFlags.Instance, null, new Type[0], null);
            MetadataAccount metaPlexData = (MetadataAccount)constructor.Invoke(null);

            metaPlexData.offchainData = new MetaplexTokenStandard();
            metaPlexData.offchainData.symbol = "dummy";
            metaPlexData.offchainData.name = "Dummy Nft";
            metaPlexData.offchainData.description = "A dummy nft which uses the wallet puy key";
            metaPlexData.mint = publicKey;

            dummyLocalNft.metaplexData = new Metaplex(metaPlexData);
            dummyLocalNft.metaplexData.nftImage = new NftImage()
            {
                name = "DummyNft",
                file = LocalDummyNft
            };

            return dummyLocalNft;
        }
        
        private static bool IsTokenMintIgnored(string mint)
        {
            if (GetIgnoreTokenList().TokenList.Contains(mint))
            {
                return true;
            }

            return false;
        }

        private static IgnoreTokenList GetIgnoreTokenList()
        {
            if (!PlayerPrefs.HasKey(IgnoredTokenListPlayerPrefsKey))
            {
                PlayerPrefs.SetString(IgnoredTokenListPlayerPrefsKey, JsonUtility.ToJson(new IgnoreTokenList()));
            }

            var json = PlayerPrefs.GetString(IgnoredTokenListPlayerPrefsKey);
            var ignoreTokenList = JsonUtility.FromJson<IgnoreTokenList>(json);
            return ignoreTokenList;
        }

        public void AddToIgnoredTokenListAndSave(string mint)
        {
            string blimpMessage = $"Added {mint} to ignore list.";
            LoggingService.Log(blimpMessage, false);

            var ignoreTokenList = GetIgnoreTokenList();
            ignoreTokenList.TokenList.Add(mint);
            PlayerPrefs.SetString(IgnoredTokenListPlayerPrefsKey, JsonUtility.ToJson(ignoreTokenList));
            PlayerPrefs.Save();
        }

        private IEnumerator StartNftLoadingDelayed(SolPlayNft nft, IRpcClient connection, int counter)
        {
            yield return new WaitForSeconds(counter * 0.4f);
        }

        public bool IsNftSelected(Nft nft)
        {
            return nft.metaplexData.data.mint == GetSelectedNftPubKey();
        }

        private string GetSelectedNftPubKey()
        {
            return PlayerPrefs.GetString("SelectedNft");
        }

        private async Task<TokenAccount[]> GetOwnedTokenAccounts(string publicKey)
        {
            var wallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;
            try
            {
                RequestResult<ResponseValue<List<TokenAccount>>> result =
                    await wallet.ActiveRpcClient.GetTokenAccountsByOwnerAsync(publicKey, null,
                        TokenProgram.ProgramIdKey, Commitment.Confirmed);

                if (result.Result != null && result.Result.Value != null)
                {
                    return result.Result.Value.ToArray();
                }
            }
            catch (Exception ex)
            {
                LoggingService.Log($"Token loading error: {ex}", true);
                Debug.LogError(ex);
                IsLoadingTokenAccounts = false;
            }

            return null;
        }

        public bool OwnsNftOfMintAuthority(string authority)
        {
            foreach (var nft in MetaPlexNFts)
            {
                if (nft.metaplexData.data.updateAuthority != null && nft.metaplexData.data.updateAuthority == authority)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsBeaverNft(Nft solPlayNft)
        {
            return solPlayNft.metaplexData.data.updateAuthority == BeaverNftMintAuthority;
        }

        public void SelectNft(Nft nft)
        {
            if (nft == null)
            {
                return;
            }

            SelectedNft = nft;
            PlayerPrefs.SetString("SelectedNft", SelectedNft.metaplexData.data.mint);
            MessageRouter.RaiseMessage(new NftSelectedMessage(SelectedNft));
        }

        public void ResetSelectedNft()
        {
            SelectedNft = null;
            PlayerPrefs.DeleteKey("SelectedNft");
            MessageRouter.RaiseMessage(new NftSelectedMessage(SelectedNft));
        }

        public IEnumerator HandleNewSceneLoaded()
        {
            yield return null;
        }

        public Nft GetNftByMintAddress(PublicKey nftMintAddress)
        {
            if (nftMintAddress == null)
            {
                return null;
            }

            foreach (var nft in MetaPlexNFts)
            {
                if (nftMintAddress == nft.metaplexData.data.mint)
                {
                    return nft;
                }
            }

            return null;
        }
    }

    public class NftLoadedMessage
    {
        public Nft Nft;

        public NftLoadedMessage(Nft nft)
        {
            Nft = nft;
        }
    }

    public class NftSelectedMessage
    {
        public Nft NewNFt;

        public NftSelectedMessage(Nft newNFt)
        {
            NewNFt = newNFt;
        }
    }

    public class NftLoadingStartedMessage
    {
    }

    public class NftLoadingFinishedMessage
    {
    }

    public class TokenValueChangedMessage
    {
    }

    public class NftMintFinishedMessage
    {
    }
}