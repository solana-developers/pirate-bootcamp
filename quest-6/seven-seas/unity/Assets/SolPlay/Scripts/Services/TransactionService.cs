using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frictionless;
using Newtonsoft.Json;
using Org.BouncyCastle.Security;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Messages;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK;
using Solana.Unity.SDK.Nft;
using Solana.Unity.Wallet;
using SolPlay.Scripts.Ui;
using UnityEngine;
using SystemProgram = Solana.Unity.Programs.SystemProgram;

namespace SolPlay.Scripts.Services
{
    /// <summary>
    /// Establishes a secure connection with phantom wallet and gets the public key from the wallet. 
    /// </summary>
    public class TransactionService : MonoBehaviour, IMultiSceneSingleton
    {
        public enum TransactionResult
        {
            processed = 0,
            confirmed = 1,
            finalized = 2
        }

        public bool ShowBlimpsForTransactions = true;

        public bool PollLatestBlockHash;
        private string latestBlockHash;

        private void Awake()
        {
            if (ServiceFactory.Resolve<TransactionService>() != null)
            {
                Destroy(gameObject);
                return;
            }

            ServiceFactory.RegisterSingleton(this);
            MessageRouter.AddHandler<WalletLoggedInMessage>(OnLoggedInMessage);
        }

        private void OnLoggedInMessage(WalletLoggedInMessage message)
        {
            if (PollLatestBlockHash)
            {
                StartCoroutine(PollRecentBlockHash());
            }
        }

        private IEnumerator PollRecentBlockHash()
        {
            while (true)
            {
                UpdateRecentBlockHash();
                yield return new WaitForSeconds(5f);
            }
        }

        private async void UpdateRecentBlockHash()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            var blockHash = await walletHolderService.BaseWallet.ActiveRpcClient.GetRecentBlockHashAsync();
            if (blockHash.WasSuccessful)
            {
                latestBlockHash = blockHash.Result.Value.Blockhash;
            }
        }

        /// <summary>
        /// await Task.Delay() does not work properly on webgl so we have to use a coroutine instead
        /// TODO: Switch this function to use UniTask. Will work in WebGL and can be delayed then
        /// </summary>
        public void CheckSignatureStatus(string signature, 
            Action<bool> onSignatureFinalized = null,
            Action<TransactionMetaSlotInfo> onError = null,
            TransactionResult transactionResult = TransactionResult.confirmed)
        {
            if (string.IsNullOrEmpty(signature))
            {
                onSignatureFinalized?.Invoke(false);
                MessageRouter.RaiseMessage(
                    new BlimpSystem.ShowLogMessage($"Signature was empty: {signature}."));
            }
            else
            {
                StartCoroutine(CheckSignatureStatusRoutine(signature, onSignatureFinalized,onError, transactionResult));
            }
        }

        private IEnumerator CheckSignatureStatusRoutine(string signature, 
            Action<bool> onSignatureFinalized,
            Action<TransactionMetaSlotInfo> onError = null,
            TransactionResult transactionResult = TransactionResult.confirmed)
        {
            var wallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;

            bool transactionFinalized = false;

            int counter = 0;
            int maxTries = 30;

            while (!transactionFinalized && counter < maxTries)
            {
                counter++;
                Task<RequestResult<ResponseValue<List<SignatureStatusInfo>>>> task =
                    wallet.ActiveRpcClient.GetSignatureStatusesAsync(new List<string>() {signature}, true);
                yield return new WaitUntil(() => task.IsCompleted);

                RequestResult<ResponseValue<List<SignatureStatusInfo>>> signatureResult = task.Result;

                if (signatureResult.Result == null)
                {
                    MessageRouter.RaiseMessage(
                        new BlimpSystem.ShowLogMessage($"There is no transaction for Signature: {signature}."));
                    yield return new WaitForSeconds(1.5f);
                    continue;
                }

                yield return new WaitForSeconds(0.5f);

                foreach (var signatureStatusInfo in signatureResult.Result.Value)
                {
                    if (signatureStatusInfo == null)
                    {
                        LoggingService
                            .Log($"Waiting for signature. Try: {counter}",
                                ShowBlimpsForTransactions);
                    }
                    else
                    {
                        if (signatureStatusInfo.ConfirmationStatus ==
                            Enum.GetName(typeof(TransactionResult), transactionResult) ||
                            signatureStatusInfo.ConfirmationStatus == Enum.GetName(typeof(TransactionResult),
                                TransactionResult.finalized))
                        {
                            LoggingService
                                .Log("Transaction " + signatureStatusInfo.ConfirmationStatus,
                                    ShowBlimpsForTransactions);

                            if (signatureStatusInfo.Error != null)
                            {
                                LoggingService
                                    .LogWarning("Transaction Error" + signatureStatusInfo.Error.InstructionError.Type,
                                        true);
                                Debug.LogError(signatureStatusInfo.Error.InstructionError);
                                ParseAndCheckError(wallet, signature, onError);
                            }

                            MessageRouter.RaiseMessage(new TokenValueChangedMessage());
                            transactionFinalized = true;
                            onSignatureFinalized?.Invoke(true);
                        }
                        else
                        {
                            LoggingService.Log(
                                $"Signature result {signatureStatusInfo.Confirmations}/31 status: {signatureStatusInfo.ConfirmationStatus} target: {Enum.GetName(typeof(TransactionResult), transactionResult)}",
                                ShowBlimpsForTransactions);
                        }
                    }
                }

                yield return new WaitForSeconds(0.5f);
            }

            if (counter >= maxTries)
            {
                onSignatureFinalized?.Invoke(false);
                MessageRouter.RaiseMessage(
                    new BlimpSystem.ShowLogMessage(
                        $"Tried {counter} times. The transaction probably failed :( "));
            }
        }

        private async void ParseAndCheckError(WalletBase wallet, string signature, Action<TransactionMetaSlotInfo> onError = null)
        {
            RequestResult<TransactionMetaSlotInfo> metaSlot =
                await wallet.ActiveRpcClient.GetTransactionAsync(signature, Commitment.Confirmed);
            if (metaSlot.Result != null)
            {
                onError?.Invoke(metaSlot.Result);
            }
        }

        public async Task<RequestResult<string>> TransferNftToPubkey(PublicKey destination, Nft nft,
            Commitment commitment = Commitment.Confirmed)
        {
            var wallHolderService = ServiceFactory.Resolve<WalletHolderService>();

            PublicKey destinationTokenAccount =
                AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(destination,
                    new PublicKey(nft.metaplexData.data.mint));
            var tokenAccounts =
                await wallHolderService.BaseWallet.ActiveRpcClient.GetTokenAccountsByOwnerAsync(destination,
                    new PublicKey(nft.metaplexData.data.mint));

            var blockHash = await wallHolderService.BaseWallet.ActiveRpcClient.GetRecentBlockHashAsync();

            var transaction = new Transaction
            {
                RecentBlockHash = blockHash.Result.Value.Blockhash,
                FeePayer = wallHolderService.BaseWallet.Account.PublicKey,
                Instructions = new List<TransactionInstruction>(),
                Signatures = new List<SignaturePubKeyPair>()
            };

            // We only need to create the token account for the destination because we cant send anything anyway if 
            // there is no account yet and we only create it if it does not exist yet.
            if (tokenAccounts.Result == null || tokenAccounts.Result.Value.Count == 0)
            {
                transaction.Instructions.Add(
                    AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                        wallHolderService.BaseWallet.Account.PublicKey,
                        destination,
                        new PublicKey(nft.metaplexData.data.mint)));
            }

            var associatedTokenAddress =
                AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(wallHolderService.BaseWallet.Account.PublicKey, new PublicKey(nft.metaplexData.data.mint));
            
            transaction.Instructions.Add(
                TokenProgram.Transfer(
                    associatedTokenAddress,
                    destinationTokenAccount,
                    1,
                    wallHolderService.BaseWallet.Account.PublicKey
                ));

            var signedTransaction = await wallHolderService.BaseWallet.SignTransaction(transaction);

            var transactionSignature =
                await wallHolderService.BaseWallet.ActiveRpcClient.SendTransactionAsync(
                    Convert.ToBase64String(signedTransaction.Serialize()), true, Commitment.Confirmed);

            CheckSignatureStatus(transactionSignature.Result);

            MessageRouter.RaiseMessage(new NftMintFinishedMessage());
            MessageRouter.RaiseMessage(new TokenValueChangedMessage());
            return transactionSignature;
        }

        public async Task<RequestResult<string>> TransferTokenToPubkey(PublicKey destination, PublicKey tokenMint,
            ulong amount, Commitment commitment = Commitment.Confirmed)
        {
            var wallHolderService = ServiceFactory.Resolve<WalletHolderService>();

            PublicKey localTokenAccount = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(
                wallHolderService.BaseWallet.Account.PublicKey, tokenMint);
            PublicKey destinationTokenAccount =
                AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(destination, tokenMint);
            var tokenAccounts =
                await wallHolderService.BaseWallet.ActiveRpcClient.GetTokenAccountsByOwnerAsync(destination, tokenMint,
                    null, commitment);

            var blockHash = await wallHolderService.BaseWallet.ActiveRpcClient.GetRecentBlockHashAsync();

            var transaction = new Transaction
            {
                RecentBlockHash = blockHash.Result.Value.Blockhash,
                FeePayer = wallHolderService.BaseWallet.Account.PublicKey,
                Instructions = new List<TransactionInstruction>(),
                Signatures = new List<SignaturePubKeyPair>()
            };

            // We only need to create the token account for the destination because we cant send anything anyway if 
            // there is no account yet and we only create it if it does not exist yet.
            if (tokenAccounts.Result == null || tokenAccounts.Result.Value.Count == 0)
            {
                transaction.Instructions.Add(
                    AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                        wallHolderService.BaseWallet.Account.PublicKey,
                        destination,
                        tokenMint));
            }

            transaction.Instructions.Add(
                TokenProgram.Transfer(
                    localTokenAccount,
                    destinationTokenAccount,
                    amount,
                    wallHolderService.BaseWallet.Account.PublicKey
                ));

            var signedTransaction = await wallHolderService.BaseWallet.SignTransaction(transaction);

            var transactionSignature =
                await wallHolderService.BaseWallet.ActiveRpcClient.SendTransactionAsync(
                    Convert.ToBase64String(signedTransaction.Serialize()), true, commitment);

            CheckSignatureStatus(transactionSignature.Result);

            MessageRouter.RaiseMessage(new TokenValueChangedMessage());
            return transactionSignature;
        }

        /// <summary>
        /// Use to send a single instruction. This function will wrap it in to a transaction and show it
        /// on screen if the blimp system oder transaction info widget are present. 
        /// </summary>
        /// <param name="transactionName"> Will be shown in the ui when the Transaction info widget is present</param>
        public void SendInstructionInNextBlock(string transactionName, 
            TransactionInstruction instruction,
            WalletBase wallet,
            Action<RequestResult<string>> onTransactionDone = null, 
            Action<TransactionMetaSlotInfo> onError = null,
            Commitment commitment = Commitment.Confirmed)
        {
            SendInstructionInNextBlockInternal(transactionName, new List<TransactionInstruction>(){instruction}, wallet, onTransactionDone, onError, commitment);
        }
        
        public void SendInstructionsInNextBlock(string transactionName, 
            List<TransactionInstruction> instruction,
            WalletBase wallet,
            Action<RequestResult<string>> onTransactionDone = null, 
            Action<TransactionMetaSlotInfo> onError = null,
            Commitment commitment = Commitment.Confirmed)
        {
            SendInstructionInNextBlockInternal(transactionName, instruction, wallet, onTransactionDone, onError, commitment);
        }

        private async void SendInstructionInNextBlockInternal(string transactionName,
            List<TransactionInstruction> instruction,
            WalletBase wallet, 
            Action<RequestResult<string>> onTransactionDone = null,
            Action<TransactionMetaSlotInfo> onError = null,
            Commitment commitment = Commitment.Confirmed)
        {
            TransactionInfoSystem.TransactionInfoObject transactionInfoObject =
                new TransactionInfoSystem.TransactionInfoObject(wallet, commitment, transactionName);
            MessageRouter.RaiseMessage(new TransactionInfoSystem.ShowTransactionInfoMessage(transactionInfoObject));

            if (!PollLatestBlockHash)
            {
                RequestResult<ResponseValue<BlockHash>> blockHashResult =
                    await wallet.ActiveRpcClient.GetRecentBlockHashAsync(Commitment.Confirmed);

                if (blockHashResult != null)
                {
                    if (blockHashResult.Result != null)
                    {
                        latestBlockHash = blockHashResult.Result.Value.Blockhash;
                        Debug.LogWarning(latestBlockHash);
                    }
                    else
                    {
                        PrintBlockHashError(blockHashResult, transactionInfoObject);
                        return;
                    }
                }
                else
                {
                    LoggingService.Log("Could not load latest block hash. Skipping", true);
                    return;
                }
            }

            SendSingleInstruction(transactionName, instruction, transactionInfoObject, wallet, onTransactionDone, onError,
                latestBlockHash);
        }

        private static void PrintBlockHashError(RequestResult<ResponseValue<BlockHash>> blockHashResult,
            TransactionInfoSystem.TransactionInfoObject transactionInfoObject)
        {
            var message = "";
            if (blockHashResult.ServerErrorCode == 429)
            {
                message = $"Rate limit reached!";
                LoggingService.Log(message, true);
            }
            else
            {
                message = $"Rpc error: {blockHashResult.ServerErrorCode}";
            }

            transactionInfoObject.OnError?.Invoke(message);
        }

        public async void SendSingleInstruction(string transactionName, List<TransactionInstruction> instruction,
            TransactionInfoSystem.TransactionInfoObject transactionInfoObject, WalletBase wallet,
            Action<RequestResult<string>> onTransactionDone = null, 
            Action<TransactionMetaSlotInfo> onError = null,
            string blockHashOverride = null,
            Commitment commitment = Commitment.Confirmed)
        {
            string blockHash = null;

            if (blockHashOverride == null)
            {
                var result = await wallet.ActiveRpcClient.GetRecentBlockHashAsync(commitment);
                if (result.Result == null)
                {
                    LoggingService.Log($"Block hash null. Ignore {transactionName}", true);
                    onTransactionDone?.Invoke(null);
                    return;
                }

                blockHash = result.Result.Value.Blockhash;
            }
            else
            {
                blockHash = blockHashOverride;
            }

            Transaction transaction = new Transaction();
            transaction.FeePayer = wallet.Account.PublicKey;
            transaction.RecentBlockHash = blockHash;
            transaction.Signatures = new List<SignaturePubKeyPair>();
            transaction.Instructions = new List<TransactionInstruction>();
            foreach (var instr in instruction)
            {
                transaction.Instructions.Add(instr);
            }

            Transaction signedTransaction = await wallet.SignTransaction(transaction);

            RequestResult<string> signature = await wallet.ActiveRpcClient.SendTransactionAsync(
                Convert.ToBase64String(signedTransaction.Serialize()),
                true, Commitment.Confirmed);

            Debug.Log("Signature: " + signature + " result:" + signature.Result);
            Debug.LogWarning(JsonConvert.SerializeObject(signature, Formatting.Indented));
            transactionInfoObject.OnSignatureReady?.Invoke(signature.Result);

            if (!signature.WasSuccessful)
            {
                LoggingService.LogWarning(signature.Reason, true);
            }

            if (onTransactionDone != null || onError != null)
            {
                ServiceFactory.Resolve<TransactionService>().CheckSignatureStatus(signature.Result, b =>
                {
                    MessageRouter.RaiseMessage(new TokenValueChangedMessage());
                    onTransactionDone?.Invoke(signature);
                }, onError);
            }
        }

        public async Task<RequestResult<string>> TransferSolanaToPubkey(WalletBase wallet, string toPublicKey,
            long lamports)
        {
            Debug.Log($"From {wallet.Account.PublicKey} to {toPublicKey} {lamports}");
            var blockHash = await wallet.ActiveRpcClient.GetRecentBlockHashAsync();

            if (blockHash.Result == null)
            {
                LoggingService.Log("Block hash null. Connected to internet?", true);
                return null;
            }

            var transferSolTransaction =
                CreateUnsignedTransferSolTransaction(wallet.Account.PublicKey, toPublicKey, blockHash, lamports);

            RequestResult<string> requestResult =
                await wallet.SignAndSendTransaction(transferSolTransaction,
                    true, Commitment.Confirmed);

            CheckSignatureStatus(requestResult.Result);

            return requestResult;
        }

        private static byte[] GenerateRandomBytes(int size)
        {
            var buffer = new byte[size];
            new SecureRandom().NextBytes(buffer);
            return buffer;
        }

        private Transaction CreateUnsignedTransferSolTransaction(PublicKey from, string toPublicKey,
            RequestResult<ResponseValue<BlockHash>> blockHash, long lamports)
        {
            Transaction transaction = new Transaction();
            transaction.Instructions = new List<TransactionInstruction>();

            var transactionInstruction = SystemProgram.Transfer(from,
                new PublicKey(toPublicKey), (ulong) lamports);

            transaction.Instructions.Add(transactionInstruction);
            transaction.FeePayer = from;
            transaction.RecentBlockHash = blockHash.Result.Value.Blockhash;
            transaction.Signatures = new List<SignaturePubKeyPair>();
            return transaction;
        }

        public IEnumerator HandleNewSceneLoaded()
        {
            yield return null;
        }
    }
}