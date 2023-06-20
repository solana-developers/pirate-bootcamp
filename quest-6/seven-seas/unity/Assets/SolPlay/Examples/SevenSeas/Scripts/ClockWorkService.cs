using System;
using System.Text;
using Frictionless;
using SevenSeas.Program;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Wallet;
using SolPlay.Scripts.Services;
using UnityEngine;

namespace SolPlay.Examples.SevenSeas.Scripts
{
    public class ClockWorkService : MonoBehaviour
    {
        public static PublicKey ThreadProgram = new PublicKey("CLoCKyJ6DXBJqqu2VWx9RLbgnwwR6BMHHuyasVmfMzBh");
        public static string ThreadId = "thread-wind";
        private void Awake()
        {
            ServiceFactory.RegisterSingleton(this);
        }

        public void StartThread()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            if (!walletHolderService.HasEnoughSol(true, 300000000))
            {
                Debug.LogError("Not enough sol");
                return;
            }
            
            PublicKey.TryFindProgramAddress(new[]
                {
                    Encoding.UTF8.GetBytes("authority")
                },
                SevenSeasService.ProgramId, out PublicKey threadAuthority, out var bump);
            
            PublicKey.TryFindProgramAddress(new[]
                {
                    Encoding.UTF8.GetBytes("thread"),
                    threadAuthority.KeyBytes,
                    Encoding.UTF8.GetBytes(ThreadId)
                },
                ThreadProgram, out PublicKey thread, out var bump2);
            
            var accounts = new StartThreadAccounts();
            accounts.SystemProgram = SystemProgram.ProgramIdKey;
            accounts.Payer = walletHolderService.BaseWallet.Account.PublicKey;
            accounts.Thread = thread;
            accounts.ClockworkProgram = ThreadProgram;
            accounts.ThreadAuthority = threadAuthority;
            accounts.GameDataAccount = ServiceFactory.Resolve<SevenSeasService>().gameDataAccount;
            
            TransactionInstruction startThreadInstruction = SevenSeasProgram.StartThread(accounts, Encoding.UTF8.GetBytes(ThreadId) ,SevenSeasService.ProgramId);

            ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock("Start Thread", startThreadInstruction,
                walletHolderService.BaseWallet);
        }

        public void PauseThread()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            if (!walletHolderService.HasEnoughSol(true, 10000))
            {
                Debug.LogError("Not enough sol");
                return;
            }
            
            PublicKey.TryFindProgramAddress(new[]
                {
                    Encoding.UTF8.GetBytes("authority")
                },
                SevenSeasService.ProgramId, out PublicKey threadAuthority, out var bump);
            
            PublicKey.TryFindProgramAddress(new[]
                {
                    Encoding.UTF8.GetBytes("thread"),
                    threadAuthority.KeyBytes,
                    Encoding.UTF8.GetBytes(ThreadId)
                },
                ThreadProgram, out PublicKey thread, out var bump2);
            
            var accounts = new PauseThreadAccounts();
            accounts.Payer = walletHolderService.BaseWallet.Account.PublicKey;
            accounts.Thread = thread;
            accounts.ClockworkProgram = ThreadProgram;
            accounts.ThreadAuthority = threadAuthority;
            
            TransactionInstruction startThreadInstruction = SevenSeasProgram.PauseThread(accounts, Encoding.UTF8.GetBytes(ThreadId) ,SevenSeasService.ProgramId);

            ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock("Pause Thread", startThreadInstruction,
                walletHolderService.BaseWallet);
        }

        public void ResumeThread()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            if (!walletHolderService.HasEnoughSol(true, 10000))
            {
                Debug.LogError("Not enough sol");
                return;
            }
            
            PublicKey.TryFindProgramAddress(new[]
                {
                    Encoding.UTF8.GetBytes("authority")
                },
                SevenSeasService.ProgramId, out PublicKey threadAuthority, out var bump);
            
            PublicKey.TryFindProgramAddress(new[]
                {
                    Encoding.UTF8.GetBytes("thread"),
                    threadAuthority.KeyBytes,
                    Encoding.UTF8.GetBytes(ThreadId)
                },
                ThreadProgram, out PublicKey thread, out var bump2);
            
            var accounts = new ResumeThreadAccounts();
            accounts.Payer = walletHolderService.BaseWallet.Account.PublicKey;
            accounts.Thread = thread;
            accounts.ClockworkProgram = ThreadProgram;
            accounts.ThreadAuthority = threadAuthority;
            
            TransactionInstruction startThreadInstruction = SevenSeasProgram.ResumeThread(accounts, Encoding.UTF8.GetBytes(ThreadId) ,SevenSeasService.ProgramId);

            ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock("Resume Thread", startThreadInstruction,
                walletHolderService.BaseWallet);
        }
    }
}