using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Frictionless;
using Newtonsoft.Json;
using Solana.Unity.Programs;
using Solana.Unity.Programs.Models;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Messages;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK;
using Solana.Unity.SDK.Utility;
using Solana.Unity.Wallet;
using SolPlay.DeeplinksNftExample.Scripts.OrcaWhirlPool;
using SolPlay.Orca;
using SolPlay.Scripts;
using SolPlay.Scripts.Services;
using UnityEngine;
using Whirlpool;
using Whirlpool.Program;
using Whirlpool.Types;
using Vector2 = UnityEngine.Vector2;

namespace SolPlay.DeeplinksNftExample.Scripts
{
    public class OrcaWhirlpoolService : MonoBehaviour, IMultiSceneSingleton
    {
        private WhirlpoolClient _whirlpoolClient;
        public static PublicKey WhirlpoolProgammId = new PublicKey("whirLbMiicVdio4qvUfM5KAg6Ct8VwpYzGff3uctyCc");
        public static PublicKey WhirlpoolConfigId = new PublicKey("2LecshUwdy9xi7meFgHtFJQNSKk4KdTrcpvaB56dP2NQ");
        public static PublicKey NativeMint = new PublicKey("So11111111111111111111111111111111111111112");

        public OrcaApiPoolsData OrcaApiPoolsData;
        public OrcaApiTokenData OrcaApiTokenData;

        const string MAX_SQRT_PRICE = "79226673515401279992447579055";
        const string MIN_SQRT_PRICE = "4295048016";

        public TextAsset PoolsAsset;
        public TextAsset TokensAsset;

        private void Awake()
        {
            if (ServiceFactory.Resolve<OrcaWhirlpoolService>() != null)
            {
                Destroy(gameObject);
                return;
            }

            ServiceFactory.RegisterSingleton(this);
        }

        private void Start()
        {
            MessageRouter.AddHandler<WalletLoggedInMessage>(OnWalletLoggedInMessage);
            OrcaApiPoolsData = JsonConvert.DeserializeObject<OrcaApiPoolsData>(PoolsAsset.text);
            OrcaApiTokenData = JsonConvert.DeserializeObject<OrcaApiTokenData>(TokensAsset.text);
            // Its faster to cache it directly in unity in text assets. But the data may be out of date, so we calculate the
            // prices from the on chain data sqrt price directly.
            RefreshApiData();
            if (ServiceFactory.Resolve<WalletHolderService>().IsLoggedIn)
            {
                Init();
            }
        }

        private async void RefreshApiData()
        {
            //OrcaApiPoolsData =
            //    await FileLoader.LoadFile<OrcaApiPoolsData>("https://api.mainnet.orca.so/v1/whirlpool/list");
            //OrcaApiTokenData = await FileLoader.LoadFile<OrcaApiTokenData>("https://api.mainnet.orca.so/v1/token/list");
            //OrcaApiPoolsData = await FileLoader.LoadFile<OrcaApiPoolsData>("https://api.devnet.orca.so/v1/whirlpool/list");
            //OrcaApiTokenData = await FileLoader.LoadFile<OrcaApiTokenData>("https://api.devnet.orca.so/v1/token/list");
        }

        private void OnWalletLoggedInMessage(WalletLoggedInMessage message)
        {
            Init();
        }

        private void Init()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            if (!walletHolderService.IsLoggedIn)
            {
                return;
            }

            var wallet = walletHolderService.BaseWallet;
            _whirlpoolClient = new WhirlpoolClient(wallet.ActiveRpcClient, null, WhirlpoolProgammId);
        }

        public async Task<Whirlpool.Accounts.Whirlpool> GetPool(string poolPDA)
        {
            if (_whirlpoolClient == null)
            {
                Init();
            }

            var whirlpoolsAsync =
                await _whirlpoolClient.GetWhirlpoolAsync(poolPDA);
            var pool = whirlpoolsAsync.ParsedResult;
            return pool;
        }

        public async Task<List<Whirlpool.Accounts.Whirlpool>> GetPools()
        {
            ProgramAccountsResultWrapper<List<Whirlpool.Accounts.Whirlpool>> whirlpoolsAsync =
                await _whirlpoolClient.GetWhirlpoolsAsync(WhirlpoolProgammId);
            List<Whirlpool.Accounts.Whirlpool> allPools = whirlpoolsAsync.ParsedResult;
            return allPools;
        }

        /// <summary>
        /// WIP
        /// </summary>
        public async Task<string> InitializePool(WalletBase wallet)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await wallet.ActiveRpcClient.GetRecentBlockHashAsync();

            Transaction swapOrcaTokenTransaction = new Transaction();
            swapOrcaTokenTransaction.FeePayer = wallet.Account.PublicKey;
            swapOrcaTokenTransaction.RecentBlockHash = blockHash.Result.Value.Blockhash;
            swapOrcaTokenTransaction.Signatures = new List<SignaturePubKeyPair>();
            swapOrcaTokenTransaction.Instructions = new List<TransactionInstruction>();

            var initializePoolAccounts = new InitializePoolAccounts();
            initializePoolAccounts.Funder = new PublicKey("");
            initializePoolAccounts.Rent = new PublicKey("");
            initializePoolAccounts.Whirlpool = new PublicKey("");
            initializePoolAccounts.FeeTier = new PublicKey("");
            initializePoolAccounts.SystemProgram = new PublicKey("");
            initializePoolAccounts.TokenProgram = new PublicKey("");
            initializePoolAccounts.WhirlpoolsConfig = new PublicKey("");
            initializePoolAccounts.TokenMintA = new PublicKey("");
            initializePoolAccounts.TokenMintB = new PublicKey("");
            initializePoolAccounts.TokenVaultA = new PublicKey("");
            initializePoolAccounts.TokenVaultB = new PublicKey("");

            WhirlpoolProgram.InitializePool(initializePoolAccounts, new WhirlpoolBumps(), UInt16.MinValue,
                BigInteger.One, WhirlpoolProgammId);

            var signedTransaction = await wallet.SignTransaction(swapOrcaTokenTransaction);
            var signature =
                await wallet.ActiveRpcClient.SendTransactionAsync(Convert.ToBase64String(signedTransaction.Serialize()),
                    false,
                    Commitment.Confirmed);
            Debug.Log(signature.Result + signature.RawRpcResponse);

            return signature.Result;
        }

        public async Task<string> Swap(WalletBase wallet, Whirlpool.Accounts.Whirlpool pool, UInt64 amount,
            bool aToB = true)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await wallet.ActiveRpcClient.GetRecentBlockHashAsync();

            PublicKey whirlPoolPda = OrcaPDAUtils.GetWhirlpoolPda(WhirlpoolProgammId, pool.WhirlpoolsConfig,
                pool.TokenMintA, pool.TokenMintB, pool.TickSpacing);

            var getWhirlpool = await _whirlpoolClient.GetWhirlpoolAsync(whirlPoolPda);
            if (getWhirlpool.ParsedResult == null)
            {
                LoggingService.LogWarning($"Could not load whirlpool {whirlPoolPda}", true);
                return null;
            }

            Debug.Log(getWhirlpool.ParsedResult.TickSpacing);

            Transaction swapOrcaTokenTransaction = new Transaction();
            swapOrcaTokenTransaction.FeePayer = wallet.Account.PublicKey;
            swapOrcaTokenTransaction.RecentBlockHash = blockHash.Result.Value.Blockhash;
            swapOrcaTokenTransaction.Signatures = new List<SignaturePubKeyPair>();
            swapOrcaTokenTransaction.Instructions = new List<TransactionInstruction>();

            var tokenOwnerAccountA =
                await CreateAtaInstruction(wallet, pool.TokenMintA, swapOrcaTokenTransaction, amount);
            var tokenOwnerAccountB =
                await CreateAtaInstruction(wallet, pool.TokenMintB, swapOrcaTokenTransaction, amount);

            int startTickIndex = TickUtils.GetStartTickIndex(pool.TickCurrentIndex, pool.TickSpacing, 0);
            var swapAccountsTickArray0 = OrcaPDAUtils.GetTickArray(WhirlpoolProgammId, whirlPoolPda, startTickIndex);

            SwapAccounts swapAccounts = new SwapAccounts();
            swapAccounts.TokenProgram = TokenProgram.ProgramIdKey;
            swapAccounts.TokenAuthority = wallet.Account.PublicKey;
            swapAccounts.TokenOwnerAccountA = tokenOwnerAccountA.PublicKey;
            swapAccounts.TokenVaultA = pool.TokenVaultA;
            swapAccounts.TokenVaultB = pool.TokenVaultB;
            swapAccounts.TokenOwnerAccountB = tokenOwnerAccountB.PublicKey;
            swapAccounts.TickArray0 = swapAccountsTickArray0;
            swapAccounts.TickArray1 = swapAccounts.TickArray0;
            swapAccounts.TickArray2 = swapAccounts.TickArray0;
            swapAccounts.Whirlpool = whirlPoolPda;
            swapAccounts.Oracle = OrcaPDAUtils.GetOracle(WhirlpoolProgammId, whirlPoolPda);

            var srqtPrice = BigInteger.Parse(aToB ? MIN_SQRT_PRICE : MAX_SQRT_PRICE);
            TransactionInstruction swapInstruction = WhirlpoolProgram.Swap(swapAccounts, amount, 0, srqtPrice,
                true, aToB, WhirlpoolProgammId);

            swapOrcaTokenTransaction.Instructions.Add(swapInstruction);

            if (pool.TokenMintA == NativeMint)
            {
                var closeAccount = TokenProgram.CloseAccount(
                    tokenOwnerAccountA,
                    wallet.Account.PublicKey,
                    wallet.Account.PublicKey,
                    TokenProgram.ProgramIdKey);
                swapOrcaTokenTransaction.Instructions.Add(closeAccount);
            }

            if (pool.TokenMintB == NativeMint)
            {
                var closeAccount = TokenProgram.CloseAccount(
                    tokenOwnerAccountB,
                    wallet.Account.PublicKey,
                    wallet.Account.PublicKey,
                    TokenProgram.ProgramIdKey);
                swapOrcaTokenTransaction.Instructions.Add(closeAccount);
            }

            Transaction signedTransaction = await wallet.SignTransaction(swapOrcaTokenTransaction);

            var signature = await wallet.ActiveRpcClient.SendTransactionAsync(
                Convert.ToBase64String(signedTransaction.Serialize()),
                true, Commitment.Confirmed);

            if (!signature.WasSuccessful)
            {
                LoggingService.LogWarning(signature.Reason, true);
            }

            return signature.Result;
        }

        private static async Task<Account> CreateAtaInstruction(WalletBase wallet, PublicKey tokenMint,
            Transaction swapOrcaTokenTransaction, ulong wrappedSolIn = 0)
        {
            if (tokenMint == NativeMint)
            {
                var minimumRent =
                    await wallet.ActiveRpcClient.GetMinimumBalanceForRentExemptionAsync(
                        TokenProgram.MintAccountDataSize);
                PublicKey tokenOwnerAccount = CreateAta(wallet, tokenMint);

                var accountInfo = await wallet.ActiveRpcClient.GetAccountInfoAsync(tokenOwnerAccount);

                if (accountInfo.Result.Value == null)
                {
                    var createWrappedSolTokenAccount = AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                        wallet.Account.PublicKey,
                        wallet.Account.PublicKey,
                        tokenMint);
                    swapOrcaTokenTransaction.Instructions.Add(createWrappedSolTokenAccount);
                }

                var transfer = SystemProgram.Transfer(wallet.Account.PublicKey, tokenOwnerAccount,
                    wrappedSolIn + minimumRent.Result);
                var nativeSync = TokenProgram.SyncNative(tokenOwnerAccount);

                swapOrcaTokenTransaction.Instructions.Add(transfer);
                swapOrcaTokenTransaction.Instructions.Add(nativeSync);

                return new Account("", tokenOwnerAccount.Key);
            }
            else
            {
                PublicKey tokenOwnerAccount = CreateAta(wallet, tokenMint);

                var accountInfo = await wallet.ActiveRpcClient.GetAccountInfoAsync(tokenOwnerAccount);

                if (accountInfo.Result.Value == null)
                {
                    var associatedTokenAccountA = AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                        wallet.Account.PublicKey, wallet.Account.PublicKey, tokenMint);
                    swapOrcaTokenTransaction.Instructions.Add(associatedTokenAccountA);
                }

                return new Account("", tokenOwnerAccount.Key);
            }
        }

        /// <summary>
        /// For some reason when trying to load the icons from token list I get a cross domain error, so for now
        /// I just added some token icons on the resources folder. 
        /// </summary>
        public static async Task<Sprite> GetTokenIconSprite(string mint, string symbol)
        {
            foreach (var token in ServiceFactory.Resolve<OrcaWhirlpoolService>().OrcaApiTokenData.tokens)
            {
                if (token.mint == mint)
                {
                    var spriteFromResources = SolPlayFileLoader.LoadFromResources(symbol);
                    if (spriteFromResources != null)
                    {
                        return spriteFromResources;
                    }

                    string tokenIconUrl = token.logoURI;
                    var texture = await SolPlayFileLoader.LoadFile<Texture2D>(tokenIconUrl);
                    Texture2D compressedTexture = SolPlayNft.Resize(texture, 75, 75);
                    var sprite = Sprite.Create(compressedTexture,
                        new Rect(0.0f, 0.0f, compressedTexture.width, compressedTexture.height),
                        new Vector2(0.5f, 0.5f),
                        100.0f);
                    return sprite;
                }
            }

            return null;
            /*
                Deprecated way of loading token icons from the Solana token-list
             string tokenIconUrl =
                $"https://github.com/solana-labs/token-list/blob/main/assets/mainnet/{mint}/logo.png?raw=true";
            var texture = await SolPlayFileLoader.LoadFile<Texture2D>(tokenIconUrl);
            Texture2D compressedTexture = Nft.Resize(texture, 75, 75);
            var sprite = Sprite.Create(compressedTexture,
                new Rect(0.0f, 0.0f, compressedTexture.width, compressedTexture.height), new Vector2(0.5f, 0.5f),
                100.0f);
            return sprite;*/
        }

        private static PublicKey CreateAta(WalletBase wallet, PublicKey mint)
        {
            return AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(wallet.Account.PublicKey, mint);
        }

        public Token GetToken(PublicKey mint)
        {
            foreach (var token in OrcaApiTokenData.tokens)
            {
                if (mint == token.mint)
                {
                    return token;
                }
            }

            return null;
        }

        public IEnumerator HandleNewSceneLoaded()
        {
            yield return null;
        }
    }
}