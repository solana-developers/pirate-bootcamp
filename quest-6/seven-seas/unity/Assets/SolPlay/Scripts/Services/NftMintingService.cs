using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CandyMachineV2;
using CandyMachineV2.Program;
using Frictionless;
using Solana.Unity.Metaplex.NFT.Library;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Messages;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using SolPlay.DeeplinksNftExample.Utils;
using UnityEngine;
using Creator = Solana.Unity.Metaplex.NFT.Library.Creator;
using MetadataProgram = Solana.Unity.Metaplex.NFT.Library.MetadataProgram;
using PublicKey = Solana.Unity.Wallet.PublicKey;
using Transaction = Solana.Unity.Rpc.Models.Transaction;

namespace SolPlay.Scripts.Services
{
    public class NftMintingService : MonoBehaviour, IMultiSceneSingleton
    {
        public void Awake()
        {
            if (ServiceFactory.Resolve<NftMintingService>() != null)
            {
                Destroy(gameObject);
                return;
            }

            ServiceFactory.RegisterSingleton(this);
        }

        public IEnumerator HandleNewSceneLoaded()
        {
            yield return null;
        }

        public async Task<string> MintNFTFromCandyMachineV2(PublicKey candyMachineKey)
        {
            var baseWallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;

            var account = baseWallet.Account;

            Account mint = new Account();

            PublicKey associatedTokenAccount =
                AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(account, mint.PublicKey);

            var candyMachineClient = new CandyMachineClient(baseWallet.ActiveRpcClient, null);
            var candyMachineWrap = await candyMachineClient.GetCandyMachineAsync(candyMachineKey);
            var candyMachine = candyMachineWrap.ParsedResult;

            var (candyMachineCreator, creatorBump) = CandyMachineUtils.getCandyMachineCreator(candyMachineKey);

            MintNftAccounts mintNftAccounts = new MintNftAccounts
            {
                CandyMachine = candyMachineKey,
                CandyMachineCreator = candyMachineCreator,
                Clock = SysVars.ClockKey,
                InstructionSysvarAccount = CandyMachineUtils.instructionSysVarAccount,
                MasterEdition = CandyMachineUtils.getMasterEdition(mint.PublicKey),
                Metadata = CandyMachineUtils.getMetadata(mint.PublicKey),
                Mint = mint.PublicKey,
                MintAuthority = account,
                Payer = account,
                RecentBlockhashes = SysVars.RecentBlockHashesKey,
                Rent = SysVars.RentKey,
                SystemProgram = SystemProgram.ProgramIdKey,
                TokenMetadataProgram = CandyMachineUtils.TokenMetadataProgramId,
                TokenProgram = TokenProgram.ProgramIdKey,
                UpdateAuthority = account,
                Wallet = candyMachine.Wallet
            };

            var candyMachineInstruction = CandyMachineProgram.MintNft(mintNftAccounts, creatorBump);

            var blockHash = await baseWallet.ActiveRpcClient.GetRecentBlockHashAsync();
            var minimumRent =
                await baseWallet.ActiveRpcClient.GetMinimumBalanceForRentExemptionAsync(
                    TokenProgram.MintAccountDataSize);

            var serializedTransaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(account)
                .AddInstruction(
                    SystemProgram.CreateAccount(
                        account,
                        mint.PublicKey,
                        minimumRent.Result,
                        TokenProgram.MintAccountDataSize,
                        TokenProgram.ProgramIdKey))
                .AddInstruction(
                    TokenProgram.InitializeMint(
                        mint.PublicKey,
                        0,
                        account,
                        account))
                .AddInstruction(
                    AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                        account,
                        account,
                        mint.PublicKey))
                .AddInstruction(
                    TokenProgram.MintTo(
                        mint.PublicKey,
                        associatedTokenAccount,
                        1,
                        account))
                .AddInstruction(candyMachineInstruction)
                .Build(new List<Account>()
                {
                    account,
                    mint
                });
            
            Transaction deserializedTransaction = Transaction.Deserialize(serializedTransaction);

            Debug.Log($"mint transaction length {serializedTransaction.Length}");

            var signedTransaction = await baseWallet.SignTransaction(deserializedTransaction);

            var transactionSignature =
                await baseWallet.ActiveRpcClient.SendTransactionAsync(
                    Convert.ToBase64String(signedTransaction.Serialize()), true, Commitment.Confirmed);

            if (!transactionSignature.WasSuccessful)
            {
                LoggingService
                    .Log("Mint was not successfull: " + transactionSignature.Reason, true);
            }
            else
            {
                ServiceFactory.Resolve<TransactionService>().CheckSignatureStatus(transactionSignature.Result,
                    success =>
                    {
                        if (success)
                        {
                            LoggingService.Log("Mint Successfull! Woop woop!", true);
                        }
                        else
                        {
                            LoggingService.Log("Mint failed!", true);
                        }
                        MessageRouter.RaiseMessage(new NftMintFinishedMessage());
                    });
            }

            Debug.Log(transactionSignature.Reason);
            return transactionSignature.Result;
        }

        public async Task<string> AuthorizeAccount(PublicKey accountToUnfreeze, PublicKey mint)
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            var localWallet = walletHolderService.BaseWallet;
            
            var closeInstruction = TokenProgram.ThawAccount(
                accountToUnfreeze,
                mint,
                localWallet.Account.PublicKey,
                TokenProgram.ProgramIdKey);
            var blockHash = await localWallet.ActiveRpcClient.GetRecentBlockHashAsync();

            var signers = new List<Account> {localWallet.Account};
            var transactionBuilder = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(localWallet.Account)
                .AddInstruction(closeInstruction);

            byte[] transaction = transactionBuilder.Build(signers);
            Transaction deserializedTransaction = Transaction.Deserialize(transaction);
            Transaction signedTransaction =
                await walletHolderService.BaseWallet.SignTransaction(deserializedTransaction);

            // This is a bit hacky, but in case of phantom wallet we need to replace the signature with the one that 
            // phantom produces
            signedTransaction.Signatures.RemoveAt(0);
            Debug.Log("signatures: " + signedTransaction.Signatures);
            foreach (var sig in signedTransaction.Signatures)
            {
                Debug.Log(sig.PublicKey);
            }

            var transactionSignature =
                await walletHolderService.BaseWallet.ActiveRpcClient.SendTransactionAsync(
                    Convert.ToBase64String(signedTransaction.Serialize()), true, Commitment.Confirmed);

            if (!transactionSignature.WasSuccessful)
            {
                LoggingService
                    .Log("Mint was not successfull: " + transactionSignature.Reason, true);
            }
            else
            {
                ServiceFactory.Resolve<TransactionService>().CheckSignatureStatus(transactionSignature.Result,
                    success =>
                    {
                        if (success)
                        {
                            LoggingService.Log("Mint Successfull! Woop woop!", true);
                        }
                        else
                        {
                            LoggingService.Log("Mint failed!", true);
                        }
                        MessageRouter.RaiseMessage(new NftMintFinishedMessage());
                    });
            }

            Debug.Log(transactionSignature.Reason);
            return transactionSignature.Result;
        }
 
        public async Task<string> CloseAccount(PublicKey accountToClose)
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            var localWallet = walletHolderService.BaseWallet;
            var closeInstruction = TokenProgram.CloseAccount(
                accountToClose,
                localWallet.Account.PublicKey,
                localWallet.Account.PublicKey, 
                TokenProgram.ProgramIdKey);
            var blockHash = await localWallet.ActiveRpcClient.GetRecentBlockHashAsync();

            var signers = new List<Account> {localWallet.Account};
            var transactionBuilder = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(localWallet.Account)
                .AddInstruction(closeInstruction);

            byte[] transaction = transactionBuilder.Build(signers);
            Transaction deserializedTransaction = Transaction.Deserialize(transaction);
            Transaction signedTransaction =
                await walletHolderService.BaseWallet.SignTransaction(deserializedTransaction);

            // This is a bit hacky, but in case of phantom wallet we need to replace the signature with the one that 
            // phantom produces
            signedTransaction.Signatures.RemoveAt(0);
            Debug.Log("signatures: " + signedTransaction.Signatures);
            foreach (var sig in signedTransaction.Signatures)
            {
                Debug.Log(sig.PublicKey);
            }

            var transactionSignature =
                await walletHolderService.BaseWallet.ActiveRpcClient.SendTransactionAsync(
                    Convert.ToBase64String(signedTransaction.Serialize()), true, Commitment.Confirmed);

            if (!transactionSignature.WasSuccessful)
            {
                LoggingService
                    .Log("Mint was not successfull: " + transactionSignature.Reason, true);
            }
            else
            {
                ServiceFactory.Resolve<TransactionService>().CheckSignatureStatus(transactionSignature.Result,
                    success =>
                    {
                        if (success)
                        {
                            LoggingService.Log("Mint Successfull! Woop woop!", true);
                        }
                        else
                        {
                            LoggingService.Log("Mint failed!", true);
                        }
                        MessageRouter.RaiseMessage(new NftMintFinishedMessage());
                    });
            }

            Debug.Log(transactionSignature.Reason);
            return transactionSignature.Result;
        }
        
        public async Task<string> MintNftWithMetaData(string metaDataUri, string name, string symbol, Action<bool> mintDone = null)
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            var wallet = walletHolderService.BaseWallet;
            var rpcClient = walletHolderService.BaseWallet.ActiveRpcClient;

            Account mint = new Account();
            var associatedTokenAccount = AssociatedTokenAccountProgram
                .DeriveAssociatedTokenAccount(wallet.Account, mint.PublicKey);
            
            var fromAccount = walletHolderService.BaseWallet.Account;

            RequestResult<ResponseValue<ulong>> balance =
                await rpcClient.GetBalanceAsync(wallet.Account.PublicKey, Commitment.Confirmed);

            if (balance.Result != null && balance.Result.Value < SolanaUtils.SolToLamports / 10)
            {
                LoggingService.Log("Sol balance is low. Minting may fail", true);
            }

            Debug.Log($"Balance: {balance.Result.Value} ");
            Debug.Log($"Mint key : {mint.PublicKey} ");

            var blockHash = await rpcClient.GetRecentBlockHashAsync();
            var rentMint = await rpcClient.GetMinimumBalanceForRentExemptionAsync(
                TokenProgram.MintAccountDataSize,
                Commitment.Confirmed
            );
            var rentToken = await rpcClient.GetMinimumBalanceForRentExemptionAsync(
                TokenProgram.TokenAccountDataSize,
                Commitment.Confirmed
            );


            //2. create a mint and a token
            var createMintAccount = SystemProgram.CreateAccount(
                fromAccount,
                mint,
                rentMint.Result,
                TokenProgram.MintAccountDataSize,
                TokenProgram.ProgramIdKey
            );
            var initializeMint = TokenProgram.InitializeMint(
                mint.PublicKey,
                0,
                fromAccount.PublicKey,
                fromAccount.PublicKey
            );
            var createTokenAccount = AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                fromAccount,
                fromAccount,
                mint.PublicKey);

            var mintTo = TokenProgram.MintTo(
                mint.PublicKey,
                associatedTokenAccount,
                1,
                fromAccount.PublicKey
            );

            // If you freeze the account the users will not be able to transfer the NFTs anywhere or burn them
            /*var freezeAccount = TokenProgram.FreezeAccount(
                tokenAccount,
                mintAccount,
                fromAccount,
                TokenProgram.ProgramIdKey
            );*/

            // PDA Metadata
            PublicKey metadataAddressPDA;
            byte nonce;
            PublicKey.TryFindProgramAddress(
                new List<byte[]>()
                {
                    Encoding.UTF8.GetBytes("metadata"),
                    MetadataProgram.ProgramIdKey,
                    mint.PublicKey
                },
                MetadataProgram.ProgramIdKey,
                out metadataAddressPDA,
                out nonce
            );

            Console.WriteLine($"PDA METADATA: {metadataAddressPDA}");

            // PDA master edition (Makes sure there can only be one minted) 
            PublicKey masterEditionAddress;

            PublicKey.TryFindProgramAddress(
                new List<byte[]>()
                {
                    Encoding.UTF8.GetBytes("metadata"),
                    MetadataProgram.ProgramIdKey,
                    mint.PublicKey,
                    Encoding.UTF8.GetBytes("edition"),
                },
                MetadataProgram.ProgramIdKey,
                out masterEditionAddress,
                out nonce
            );
            Console.WriteLine($"PDA MASTER: {masterEditionAddress}");

            // Craetors
            var creator1 = new Creator(fromAccount.PublicKey, 100, false);

            // Meta Data
            var data = new Metadata()
            {
                name = name,
                symbol = symbol,
                uri = metaDataUri,
                creators = new List<Creator>() {creator1},
                sellerFeeBasisPoints = 77
            };

            var signers = new List<Account> {fromAccount, mint};
            var transactionBuilder = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(createMintAccount)
                .AddInstruction(initializeMint)
                .AddInstruction(createTokenAccount)
                .AddInstruction(mintTo)
                //.AddInstruction(freezeAccount)
                .AddInstruction(
                    MetadataProgram.CreateMetadataAccount(
                        metadataAddressPDA, // PDA
                        mint,
                        fromAccount.PublicKey,
                        fromAccount.PublicKey,
                        fromAccount.PublicKey, // update Authority 
                        data, // DATA
                        TokenStandard.NonFungible,
                        true,
                        true, // ISMUTABLE,
                        masterEditionKey: null,
                        1,
                        0UL,
                        MetadataVersion.V3
                    )
                )
                .AddInstruction(
                    MetadataProgram.SignMetadata(
                        metadataAddressPDA,
                        creator1.key
                    )
                )
               .AddInstruction(
                    MetadataProgram.PuffMetada(
                        metadataAddressPDA
                    )
                )
                /*.AddInstruction(
                    MetadataProgram.CreateMasterEdition(
                        1,
                        masterEditionAddress,
                        mintAccount,
                        fromAccount.PublicKey,
                        fromAccount.PublicKey,
                        fromAccount.PublicKey,
                        metadataAddressPDA
                    )
                )*/;

            var tx = Transaction.Deserialize(transactionBuilder.Build(new List<Account> {fromAccount, mint}));
            var res = await walletHolderService.BaseWallet.SignAndSendTransaction(tx, true, Commitment.Confirmed);
            Debug.Log(res.Result);

            if (!res.WasSuccessful)
            {
                mintDone?.Invoke(false);
                LoggingService
                    .Log("Mint was not successfull: " + res.Reason, true);
            }
            else
            {
                ServiceFactory.Resolve<TransactionService>().CheckSignatureStatus(res.Result,
                    success =>
                    {
                        mintDone?.Invoke(success);
                        LoggingService.Log("Mint Successfull! Woop woop!", true);
                        MessageRouter.RaiseMessage(new NftMintFinishedMessage());
                    }, null, TransactionService.TransactionResult.confirmed);
            }

            return res.Result;
        }

        public async void MintNftWithoutMetaDat()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            var wallet = walletHolderService.BaseWallet;
            var rpcClient = walletHolderService.BaseWallet.ActiveRpcClient;

            Account mintAccount = new Account();
            Account tokenAccount = new Account();

            var fromAccount = walletHolderService.BaseWallet.Account;

            RequestResult<ResponseValue<ulong>> balance = await rpcClient.GetBalanceAsync(wallet.Account.PublicKey);

            Debug.Log($"Balance: {balance.Result.Value} ");
            Debug.Log($"Mint key : {mintAccount.PublicKey} ");

            var blockHash = await rpcClient.GetRecentBlockHashAsync();
            var rentMint = await rpcClient.GetMinimumBalanceForRentExemptionAsync(
                TokenProgram.MintAccountDataSize,
                Commitment.Confirmed
            );
            var rentToken = await rpcClient.GetMinimumBalanceForRentExemptionAsync(
                TokenProgram.TokenAccountDataSize,
                Commitment.Confirmed
            );

            Debug.Log($"Token key : {tokenAccount.PublicKey} ");

            //2. create a mint and a token
            var createMintAccount = SystemProgram.CreateAccount(
                fromAccount,
                mintAccount,
                rentMint.Result,
                TokenProgram.MintAccountDataSize,
                TokenProgram.ProgramIdKey
            );
            var initializeMint = TokenProgram.InitializeMint(
                mintAccount.PublicKey,
                0,
                fromAccount.PublicKey
            );
            var createTokenAccount = SystemProgram.CreateAccount(
                fromAccount,
                tokenAccount,
                rentToken.Result,
                TokenProgram.TokenAccountDataSize,
                TokenProgram.ProgramIdKey
            );
            var initializeMintAccount = TokenProgram.InitializeAccount(
                tokenAccount.PublicKey,
                mintAccount.PublicKey,
                fromAccount.PublicKey
            );
            var mintTo = TokenProgram.MintTo(
                mintAccount.PublicKey,
                tokenAccount,
                1,
                fromAccount.PublicKey
            );

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(createMintAccount)
                .AddInstruction(initializeMint)
                .AddInstruction(createTokenAccount)
                .AddInstruction(initializeMintAccount)
                .AddInstruction(mintTo)
                .Build(new List<Account> {fromAccount, mintAccount, tokenAccount});

            Console.WriteLine($"TX1.Length {transaction.Length}");

            var transactionSignature =
                await walletHolderService.BaseWallet.ActiveRpcClient.SendTransactionAsync(
                    Convert.ToBase64String(transaction), false, Commitment.Confirmed);

            if (transactionSignature.WasSuccessful)
            {
                Debug.Log($"Send transaction to create mint with id: {mintAccount}");
            }
            else
            {
                Debug.Log("There was an error creating mint: {transactionSignature.Reason}");
            }
        }

        /// <summary>
        /// In case you have a mint and want to add meta data to it.
        /// I recommend to use MintNftWithMetaData instead.
        /// </summary>
        /// <param name="mint"></param>
        /// <param name="metaDataUri"></param>
        public async void AddMetaDataToMint(PublicKey mint, string metaDataUri)
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            var wallet = walletHolderService.BaseWallet;
            var rpcClient = walletHolderService.BaseWallet.ActiveRpcClient;

            var fromAccount = wallet.Account;
            var mintAccount = mint;

            var blockHash = await rpcClient.GetRecentBlockHashAsync();

            // PDA METADATA
            PublicKey metadataAddress;
            byte nonce;
            PublicKey.TryFindProgramAddress(
                new List<byte[]>()
                {
                    Encoding.UTF8.GetBytes("metadata"),
                    MetadataProgram.ProgramIdKey,
                    mintAccount
                },
                MetadataProgram.ProgramIdKey,
                out metadataAddress,
                out nonce
            );

            Console.WriteLine($"PDA METADATA: {metadataAddress}");

            // PDA MASTER EDITION
            PublicKey masterEditionAddress;

            PublicKey.TryFindProgramAddress(
                new List<byte[]>()
                {
                    Encoding.UTF8.GetBytes("metadata"),
                    MetadataProgram.ProgramIdKey,
                    mintAccount,
                    Encoding.UTF8.GetBytes("edition")
                },
                MetadataProgram.ProgramIdKey,
                out masterEditionAddress,
                out nonce
            );
            Console.WriteLine($"PDA MASTER: {masterEditionAddress}");

            // CREATORS
            var creator1 = new Creator(fromAccount.PublicKey, 100);

            // DATA
            var data = new Metadata()
            {
                name = "Super NFT",
                symbol = "SolPlay",
                uri = metaDataUri,
                creators = new List<Creator>() {creator1},
                sellerFeeBasisPoints = 77,
            };
            
            var transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount.PublicKey)
                .AddInstruction(
                    MetadataProgram.CreateMetadataAccount(
                        metadataAddress, // PDA
                        mintAccount, // MINT
                        fromAccount.PublicKey, // mint AUTHORITY
                        fromAccount.PublicKey, // PAYER
                        fromAccount.PublicKey, // update Authority 
                        data: data, // DATA
                        TokenStandard.NonFungible,
                        true,
                        true,
                        null, 
                        1,
                        0UL,
                        Solana.Unity.Metaplex.NFT.Library.MetadataVersion.V1
                        
                    )
                )
                .AddInstruction(
                    MetadataProgram.SignMetadata(
                        metadataAddress,
                        creator1.key
                    )
                )
                .AddInstruction(
                    MetadataProgram.PuffMetada(
                        metadataAddress
                    )
                )
                .AddInstruction(
                    MetadataProgram.CreateMasterEdition(
                        1,
                        masterEditionAddress,
                        mintAccount,
                        fromAccount.PublicKey,
                        fromAccount.PublicKey,
                        fromAccount.PublicKey,
                        metadataAddress
                    )
                )
                .Build(new List<Account> {fromAccount});

            Console.WriteLine($"TX2.Length {transaction.Length}");

            var txSim2 = await rpcClient.SimulateTransactionAsync(transaction);

            var transactionSignature =
                await walletHolderService.BaseWallet.ActiveRpcClient.SendTransactionAsync(
                    Convert.ToBase64String(transaction), false, Commitment.Confirmed);

            Debug.Log(transactionSignature.Reason);
        }
    }
}