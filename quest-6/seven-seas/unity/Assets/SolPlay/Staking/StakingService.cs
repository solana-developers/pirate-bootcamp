using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Frictionless;
using GemBank.Program;
using GemFarm.Program;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Wallet;
using SolPlay.Scripts.Services;
using SolPlay.Scripts.Ui;
using UnityEngine;

namespace SolPlay.Staking
{
    /// <summary>
    /// Staking service is still work in progress
    /// </summary>
    public class StakingService : MonoBehaviour
    {
        PublicKey BankProgrammId = new PublicKey(
            "bankHHdqMuaaST4qQk6mkzxGeKPHWmqdgor6Gs8r88m"
        );
        
        PublicKey BankId = new PublicKey(
            "6qow2ZSoCNLupaohidmuuNUCoJ8fFpBeddx4JCc9M6at"
        );

        private void Awake()
        {
            ServiceFactory.RegisterSingleton(this);
        }

        public async void RefreshFarm()
        {
            var transation = await BuildRefreshFarmTransaction();
            var signature = await ServiceFactory.Resolve<WalletHolderService>().BaseWallet
                .SignAndSendTransaction(transation);

            if (string.IsNullOrEmpty(signature.Result))
            {
                MessageRouter
                    .RaiseMessage(new BlimpSystem.ShowLogMessage(signature.Reason));
            }
            else
            {
                ServiceFactory.Resolve<TransactionService>()
                    .CheckSignatureStatus(signature.Result, success => { Debug.Log("Farm refreshed successfully."); });
            }
        }

        private async Task<Transaction> BuildRefreshFarmTransaction()
        {
            var wallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;
            Transaction transaction = await CreateEmptyTransaction();
            if (transaction == null) return null;

            var farmAddress = GemFarmPDAHelper.FindFarmerPDA(wallet.Account.PublicKey, out byte farmerBump);

            List<AccountMeta> accountMetaList = new List<AccountMeta>()
            {
                AccountMeta.Writable(GemFarmPDAHelper.Farm, false),
                AccountMeta.Writable(farmAddress, false),
                AccountMeta.Writable(wallet.Account.PublicKey, true),
            };
            
            byte[] data = Encoding.Default.GetBytes(GemFarmPDAHelper.RefreshFarmInstructionIdentifier);
            /*var dataWithHashedInstructionIdentifier = SHA256.Create().ComputeHash(data).Slice(0, 9);
            dataWithHashedInstructionIdentifier.WriteU8(farmerBump, 8);*/

            RefreshFarmerAccounts account = new RefreshFarmerAccounts();
            account.Farm = GemFarmPDAHelper.Farm;
            account.Farmer = farmAddress;
            account.Identity = wallet.Account.PublicKey;
            TransactionInstruction instr = GemFarmProgram.RefreshFarmer(account, farmerBump, GemFarmPDAHelper.FarmProgramm);
            
            TransactionInstruction refreshFarmerInstruction = new TransactionInstruction()
            {
                ProgramId = GemFarmPDAHelper.FarmProgramm,
                Keys = accountMetaList,
                //Data = dataWithHashedInstructionIdentifier
            };
            transaction.Instructions.Add(instr);
            //transaction.Instructions.Add(refreshFarmerInstruction);
            
            return transaction;
        }

        private async Task<Transaction> BuildStakeTransaction(bool unstake = false, bool skipRewards = false)
        {
            var wallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;
            Transaction transaction = await CreateEmptyTransaction();
            if (transaction == null) return null;

            var depositGemAccounts = new DepositGemAccounts();
            TransactionInstruction depositGemInstruction = GemBankProgram.DepositGem(depositGemAccounts, 0, 0, 1, BankId);
            transaction.Instructions.Add(depositGemInstruction);
            return transaction;
        }

        private static async Task<Transaction> CreateEmptyTransaction()
        {
            var wallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;
            var blockHash = await wallet.ActiveRpcClient.GetRecentBlockHashAsync();

            if (blockHash.Result == null)
            {
                MessageRouter
                    .RaiseMessage(new BlimpSystem.ShowLogMessage("Block hash null. Connected to internet?"));
                return null;
            }

            return new Transaction
            {
                RecentBlockHash = blockHash.Result.Value.Blockhash,
                FeePayer = wallet.Account.PublicKey,
                Signatures = new List<SignaturePubKeyPair>(),
                Instructions = new List<TransactionInstruction>()
            };
        }
    }
}