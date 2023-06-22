using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using ShadowDriveUserStaking.Program;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using UnityEngine;

namespace SolPlay.Scripts.Services
{
    public class ShadowDriveService : MonoBehaviour, IMultiSceneSingleton
    {
        public PublicKey programAddress = new PublicKey("2e1wdyNhUvE76y6yUCvah2KaviavMJYKoRun8acMRBZZ");
        public PublicKey tokenMint = new PublicKey("SHDWyBxihqiCj6YekG2GUr7wqKLeLAMK1gHZck9pL6y");
        public PublicKey uploader = new PublicKey("972oJTFyjmVNsWM4GHEGPWUomAiJf2qrVotLtwnKmWem");
        public PublicKey emissions = new PublicKey("SHDWRWMZ6kmRG9CvKFSD7kVcnUqXMtd3SaMrLvWscbj");
        public string SHDW_DRIVE_ENDPOINT = "https://shadow-storage.genesysgo.net";

        public void Awake()
        {
            if (ServiceFactory.Resolve<ShadowDriveService>() != null)
            {
                Destroy(gameObject);
                return;
            }

            ServiceFactory.RegisterSingleton(this);
        }

        // TODO: WIP 
        public async void UploadMetaData()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            var wallet = walletHolderService.BaseWallet;
            var localAccount = walletHolderService.BaseWallet.Account.PublicKey;
            var rpcClient = walletHolderService.BaseWallet.ActiveRpcClient;
            var blockHash = await rpcClient.GetRecentBlockHashAsync();

            InitializeAccount2Accounts accountsInstrucation = new InitializeAccount2Accounts();
            accountsInstrucation.Owner1 = localAccount;
            accountsInstrucation.Rent = new PublicKey("");
            accountsInstrucation.Uploader = new PublicKey("");
            accountsInstrucation.StakeAccount = new PublicKey("");
            accountsInstrucation.StorageAccount = new PublicKey("");
            accountsInstrucation.StorageConfig = new PublicKey("");
            accountsInstrucation.SystemProgram = SystemProgram.ProgramIdKey;
            accountsInstrucation.TokenMint = new PublicKey("");
            accountsInstrucation.Owner1TokenAccount = new PublicKey("");
            accountsInstrucation.UserInfoShadow = new PublicKey("");
            var initializeAccountInstruction =
                ShadowDriveUserStakingProgram.InitializeAccount2(accountsInstrucation, "MyShadow", 10000,
                    programAddress);

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(localAccount)
                .AddInstruction(initializeAccountInstruction) // create
                .Build(new List<Account> {walletHolderService.BaseWallet.Account});

            var transactionSignature =
                await walletHolderService.BaseWallet.ActiveRpcClient.SendTransactionAsync(
                    Convert.ToBase64String(transaction), false, Commitment.Confirmed);

            Debug.Log(transactionSignature.Reason);
        }

        public IEnumerator HandleNewSceneLoaded()
        {
            yield return null;
        }
    }
}