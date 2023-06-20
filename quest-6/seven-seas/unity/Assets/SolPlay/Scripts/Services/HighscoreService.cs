using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Frictionless;
using Org.BouncyCastle.Utilities.Encoders;
using Solana.Unity.Programs;
using Solana.Unity.Programs.Utilities;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Messages;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK;
using Solana.Unity.SDK.Nft;
using Solana.Unity.Wallet;
using SolPlay.DeeplinksNftExample.Scripts;
using SolPlay.DeeplinksNftExample.Utils;
using SolPlay.Scripts.Ui;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SolPlay.Scripts.Services
{
    public class HighscoreAccount
    {
        public UInt32 Highscore = 0;

        public static long GetAccountSize()
        {
            return sizeof(UInt32);
        }
    }

    public class HighscoreEntry
    {
        public uint Highscore;
        public string Seed;
        public bool AccountLoaded;
    }

    public class HighscoreService : MonoBehaviour, IMultiSceneSingleton
    {
        private Dictionary<string, HighscoreEntry> _allHighscores = new Dictionary<string, HighscoreEntry>();

        private readonly PublicKey _highscoreProgramPublicKey =
            new PublicKey("F3qQ9mJep9hwCkJRtRSUcxov5etdRvQU9NBFpPjh4LKo");

        private readonly PublicKey _highscoreSubmitFeePubkey =
            new PublicKey("pLAY7z6bY7SRvhCm8hSqyXaerAegsdGWkV1aSgjFpab");

        private const string ScoreSeed = "score";

        public bool LoadHighscoresForAllNFtsAutomatically = false;

        public void Awake()
        {
            if (ServiceFactory.Resolve<HighscoreService>() != null)
            {
                Destroy(gameObject);
                return;
            }

            ServiceFactory.RegisterSingleton(this);
        }

        private void Start()
        {
            MessageRouter.AddHandler<NftLoadingFinishedMessage>(OnAllNftsLoadedMessage);
            MessageRouter.AddHandler<NftLoadedMessage>(OnNftArrivedMessage);
        }

        private void OnNftArrivedMessage(NftLoadedMessage message)
        {
            if (!LoadHighscoresForAllNFtsAutomatically)
            {
                return;
            }
            var seedFromPubkey = GetSeedFromPubkey(message.Nft.metaplexData.data.mint);

            var highscoreEntry = new HighscoreEntry()
            {
                Highscore = 0,
                Seed = seedFromPubkey,
                AccountLoaded = false
            };
            _allHighscores[seedFromPubkey] = highscoreEntry;

            // Taking some work from the RPC nodes and delay the high score requests.
            StartCoroutine(GetHighScoreDataDelayed(message.Nft, Random.Range(0, 3)));
        }

        private IEnumerator GetHighScoreDataDelayed(Nft messageNewNFt, int range)
        {
            yield return new WaitForSeconds(range);
            GetHighscoreAccountData(messageNewNFt);
        }

        private async void OnAllNftsLoadedMessage(NftLoadingFinishedMessage message)
        {
            // Nothing to do
        }

        private string GetSeedFromPubkey(string pubkey)
        {
            return pubkey.Substring(0, 8);
        }

        public string GetCurrentAccountSeed()
        {
            var solPlayNft = ServiceFactory.Resolve<NftService>().SelectedNft;
            if (solPlayNft != null)
            {
                return GetSeedFromPubkey(solPlayNft.metaplexData.data.mint);
            }

            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            return GetSeedFromPubkey(walletHolderService.BaseWallet.Account.PublicKey);
        }

        public bool TryGetHighscoreForSeed(string seed, out HighscoreEntry highscoreEntry)
        {
            foreach (var entry in _allHighscores)
            {
                if (entry.Value.Seed == GetSeedFromPubkey(seed))
                {
                    highscoreEntry = entry.Value; 
                    return true;
                }
            }
            highscoreEntry = null;
            return false;
        }

        public bool TryGetCurrentHighscore(out HighscoreEntry highscoreEntry)
        {
            return _allHighscores.TryGetValue(GetCurrentAccountSeed(), out highscoreEntry);
        }

        private PublicKey GetNftRelatedPubkey(Nft nft)
        {
            return new PublicKey(nft.metaplexData.data.mint);
        }

        public async Task<AccountInfo> GetHighscoreAccountData(Nft nft)
        {
            var wallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;

            var nftPubkey = GetNftRelatedPubkey(nft);
            if (!PublicKey.TryFindProgramAddress(
                    new List<byte[]>() {Encoding.ASCII.GetBytes(ScoreSeed), nftPubkey.KeyBytes},
                    _highscoreProgramPublicKey, out var programDerivedAccount, out byte bump)) return null;

            RequestResult<ResponseValue<AccountInfo>> accountInfoResult =
                await wallet.ActiveRpcClient.GetAccountInfoAsync(programDerivedAccount, Commitment.Confirmed);

            var seed = GetSeedFromPubkey(GetNftRelatedPubkey(nft));
            _allHighscores[seed].AccountLoaded = true;

            if (accountInfoResult != null && accountInfoResult.Result != null && accountInfoResult.Result.Value != null)
            {
                foreach (var entry in accountInfoResult.Result.Value.Data)
                {
                    try
                    {
                        byte[] message = Base64.Decode(entry);
                        uint uInt32 = BitConverter.ToUInt32(message);
                        var highscoreText = "High score of your NFT is: " + uInt32;
                        // This will show the high score of the NFT on the screen. If there are many NFTs it can get a bit spammy
                        // MessageRouter.RaiseMessage(new BlimpSystem.ShowBlimpMessage(highscoreText));
                        _allHighscores[seed].Highscore = uInt32;
                        MessageRouter
                            .RaiseMessage(new NewHighScoreLoadedMessage(_allHighscores[seed]));
                    }
                    catch (Exception e)
                    {
                        // Wasn't a base 64 string 
                    }
                }

                return accountInfoResult.Result.Value;
            }

            // In case you want to log that there are no high scores saved on the NFTs yet. It gets a bit spammy quickly.
            /*MessageRouter
                .RaiseMessage(
                    new BlimpSystem.ShowBlimpMessage($"No high score for {nft.MetaplexData.data.name} found yet."));*/

            return null;
        }

        public async Task SafeHighScore(Nft nft, uint highScore)
        {
            var wallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;
            double sol = await wallet.GetBalance() * SolanaUtils.SolToLamports;

            var levelAccount = await GetHighscoreAccountData(nft);

            var blockHash = await wallet.ActiveRpcClient.GetRecentBlockHashAsync();

            if (blockHash.Result == null)
            {
                LoggingService.Log("Block hash null. Connected to internet?", true);
                return;
            }

            ulong fees = blockHash.Result.Value.FeeCalculator.LamportsPerSignature * 100;
            if (levelAccount == null)
            {
                var accountDataSize = HighscoreAccount.GetAccountSize();
                RequestResult<ulong> costPerAccount =
                    await wallet.ActiveRpcClient.GetMinimumBalanceForRentExemptionAsync(accountDataSize);
                fees += costPerAccount.Result;
            }

            LoggingService.Log(
                $"Pubkey: {wallet.Account.PublicKey} - SolAmount = " + sol + " needed for account: " + fees, true);

            if (sol <= fees)
            {
                if (wallet.RpcCluster == RpcCluster.MainNet)
                {
                    LoggingService.Log(
                        $"You dont have enough sol to pay for account creation. Need at least: {fees} ", true);
                }
                else
                {
                    RequestResult<string> result = await wallet.RequestAirdrop(1000000000);
                    if (string.IsNullOrEmpty(result.Result))
                    {
                        LoggingService
                            .Log("Air drop request failed. Are connected to the internet?", true);
                        return;
                    }

                    ServiceFactory.Resolve<TransactionService>().CheckSignatureStatus(result.Result);

                    var balance = await wallet.GetBalance();

                    sol = balance * SolanaUtils.SolToLamports;
                    MessageRouter.RaiseMessage(new SolBalanceChangedMessage());
                    if (sol <= fees)
                    {
                        Debug.Log($"Request airdrop: {result} Are you connected to the internet or on main net?");
                        return;
                    }
                }
            }

            await CreateAndSendUnsignedSafeHighscoreTransaction(blockHash.Result.Value, levelAccount == null, nft,
                highScore);
        }

        private async Task CreateAndSendUnsignedSafeHighscoreTransaction(BlockHash blockHash, bool createAccount,
            Nft nft, uint highScore)
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            var activeRpcClient = walletHolderService.BaseWallet.ActiveRpcClient;

            if (!await CheckIfProgramIsDeployed(activeRpcClient)) return;

            var localPublicKey = walletHolderService.BaseWallet.Account.PublicKey;
            var nftMintPublicKey = new PublicKey(nft.metaplexData.data.mint);

            if (!PublicKey.TryFindProgramAddress(
                    new List<byte[]>() {Encoding.ASCII.GetBytes(ScoreSeed), nftMintPublicKey.KeyBytes},
                    _highscoreProgramPublicKey, out var programDerivedAccount, out byte bump)) return;

            Transaction increasePlayerLevelTransaction = new Transaction();
            increasePlayerLevelTransaction.FeePayer = localPublicKey;
            increasePlayerLevelTransaction.RecentBlockHash = blockHash.Blockhash;
            increasePlayerLevelTransaction.Signatures = new List<SignaturePubKeyPair>();
            increasePlayerLevelTransaction.Instructions = new List<TransactionInstruction>();

            Debug.Log(nft.metaplexData.data.mint);

            List<AccountMeta> accountMetaList = new List<AccountMeta>()
            {
                AccountMeta.Writable(programDerivedAccount, false),
                AccountMeta.Writable(localPublicKey, true),
                AccountMeta.Writable(_highscoreSubmitFeePubkey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(nftMintPublicKey, false)
            };

            byte[] data = new byte[7];
            data.WriteU8(2, 0);
            data.WriteU32(highScore, 1);
            data.WriteU8(bump, 5);
            data.WriteU8(createAccount ? (byte) 1 : (byte) 0, 6);

            TransactionInstruction highscoreInstruction = new TransactionInstruction()
            {
                ProgramId = _highscoreProgramPublicKey,
                Keys = accountMetaList,
                Data = data
            };

            increasePlayerLevelTransaction.Instructions.Add(highscoreInstruction);

            var sendingTransactionUsing = "Sending transaction using: " + walletHolderService.BaseWallet.GetType();
            LoggingService.Log(sendingTransactionUsing, true);

            var signedTransaction =
                await walletHolderService.BaseWallet.SignTransaction(increasePlayerLevelTransaction);

            var transactionSignature =
                await walletHolderService.BaseWallet.ActiveRpcClient.SendTransactionAsync(
                    Convert.ToBase64String(signedTransaction.Serialize()), false, Commitment.Confirmed);

            if (transactionSignature.WasSuccessful)
            {
                var checkingSignatureNow = "Signed via BaseWallet: " + transactionSignature.Result +
                                           " checking signature now. ";
                LoggingService.Log(checkingSignatureNow, true);
                ServiceFactory.Resolve<TransactionService>()
                    .CheckSignatureStatus(transactionSignature.Result, b =>
                {
                    GetHighscoreAccountData(nft);
                    MessageRouter.RaiseMessage(new SolBalanceChangedMessage());
                });
            }
            else
            {
                var error = $"There was an error: {transactionSignature.Reason} {transactionSignature.RawRpcResponse} ";
                LoggingService.LogWarning(error, true);
            }
        }

        private async Task<bool> CheckIfProgramIsDeployed(IRpcClient activeRpcClient)
        {
            RequestResult<ResponseValue<AccountInfo>> programAccountInfo =
                await activeRpcClient.GetAccountInfoAsync(_highscoreProgramPublicKey);

            if (programAccountInfo.Result != null && programAccountInfo.Result.Value != null)
            {
                Debug.Log("Program is available and executable: " + programAccountInfo.Result.Value.Executable);
            }
            else
            {
                Debug.Log("Program probably not deployed: ");
                return false;
            }

            return true;
        }

        public uint GetTotalScore()
        {
            uint result = 0;
            foreach (var entry in _allHighscores)
            {
                result += entry.Value.Highscore;
            }

            return result;
        }

        public IEnumerator HandleNewSceneLoaded()
        {
            yield return null;
        }
    }

    public class NewHighScoreLoadedMessage
    {
        public HighscoreEntry HighscoreEntry;

        public NewHighScoreLoadedMessage(HighscoreEntry highscoreEntry)
        {
            HighscoreEntry = highscoreEntry;
        }
    }
}