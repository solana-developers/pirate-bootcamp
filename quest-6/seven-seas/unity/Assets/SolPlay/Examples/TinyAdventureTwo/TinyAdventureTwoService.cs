using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Frictionless;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using SolPlay.Scripts.Services;
using TinyAdventureTwo.Program;
using TinyAdventureTwo.Accounts;
using UnityEngine;
using MoveRightAccounts = TinyAdventureTwo.Program.MoveRightAccounts;

public class TinyAdventureTwoService : MonoBehaviour
{
    //public static PublicKey ProgramId = new PublicKey("3UqUVArdz16iFtw8ushL4qU32qY9yB46aTwuxGPeWaRa");
    public static PublicKey ProgramId = new PublicKey("7Sw57PQdnwHM9wBC52kyVyMyPr9kmHR54QW5yVkqiYKr");

    private PublicKey gameDataAccount;
    private PublicKey chestVaultAccount;

    public class GameDataChangedMessage
    {
        public GameDataAccount GameDataAccount;
    }

    private void Awake()
    {
        ServiceFactory.RegisterSingleton(this);
        
        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("level1")
            },
            ProgramId, out gameDataAccount, out var bump);
        
        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("chestVault")
            },
            ProgramId, out chestVaultAccount, out var bump2);
    }

    private void Start()
    {
        MessageRouter.AddHandler<SocketServerConnectedMessage>(OnSocketConnected);
    }

    private void OnSocketConnected(SocketServerConnectedMessage message)
    {
        var solPlayWebSocketService = ServiceFactory.Resolve<SolPlayWebSocketService>();
        solPlayWebSocketService.SubscribeToPubKeyData(gameDataAccount, result =>
        {
            GameDataAccount gameDataAccount =
                GameDataAccount.Deserialize(Convert.FromBase64String(result.result.value.data[0]));
            MessageRouter.RaiseMessage(new GameDataChangedMessage()
            {
                GameDataAccount = gameDataAccount
            });
        });
    }

    public async Task<GameDataAccount> GetGameData()
    {
        var baseWalletActiveRpcClient = ServiceFactory.Resolve<WalletHolderService>().BaseWallet.ActiveRpcClient;
        var gameData = await baseWalletActiveRpcClient
            .GetAccountInfoAsync(this.gameDataAccount, Commitment.Confirmed, BinaryEncoding.JsonParsed);
        GameDataAccount gameDataAccount =
            GameDataAccount.Deserialize(Convert.FromBase64String(gameData.Result.Value.Data[0]));
        Debug.Log(gameDataAccount.PlayerPosition);
        MessageRouter.RaiseMessage(new GameDataChangedMessage()
        {
            GameDataAccount = gameDataAccount
        });
        return gameDataAccount;
    }

    public void Initialize()
    {
        TransactionInstruction initializeInstruction = GetInitializeInstruction();
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        ServiceFactory.Resolve<TransactionService>()
            .SendInstructionInNextBlock("Initialize", initializeInstruction, walletHolderService.BaseWallet);
    }

    public void ResetLevelAndSpawnChest()
    {
        TransactionInstruction resetLevelAndSpawnChestInstruction = GetResetLevelAndSpawnChestInstruction();
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        ServiceFactory.Resolve<TransactionService>()
            .SendInstructionInNextBlock("Reset Level", resetLevelAndSpawnChestInstruction, walletHolderService.BaseWallet,result => {});
    }

    public void MoveRight(string password, Action onWrongPassword)
    {
        TransactionInstruction moveRightInstruction = GetMoveRightInstruction(password);
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        ServiceFactory.Resolve<TransactionService>()
            .SendInstructionInNextBlock("Move Right", moveRightInstruction, walletHolderService.BaseWallet, onError:
                transactionMetaSlotInfo =>
                {
                    if (transactionMetaSlotInfo.Meta.Error.InstructionError.CustomError ==
                        (uint) TinyAdventureTwo.Errors.TinyAdventureTwoErrorKind.WrongPassword)
                    {
                        onWrongPassword();
                    }
                });
    }

    /// <summary>
    /// This is how you can build a transaction without using the transaction service.
    /// The transaction service, 
    /// </summary>
    public async void MoveRightManual(string password, Action onWrongPassword)
    {
        TransactionInstruction moveRightInstruction = GetMoveRightInstruction(password);
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        var result = await walletHolderService.BaseWallet.ActiveRpcClient.GetRecentBlockHashAsync(Commitment.Confirmed);
        
        Transaction transaction = new Transaction();
        transaction.FeePayer = walletHolderService.BaseWallet.Account.PublicKey;
        transaction.RecentBlockHash = result.Result.Value.Blockhash;
        transaction.Signatures = new List<SignaturePubKeyPair>();
        transaction.Instructions = new List<TransactionInstruction>();
        transaction.Instructions.Add(moveRightInstruction);

        Transaction signedTransaction = await walletHolderService.BaseWallet.SignTransaction(transaction);

        RequestResult<string> signature = await walletHolderService.BaseWallet.ActiveRpcClient.SendTransactionAsync(
            Convert.ToBase64String(signedTransaction.Serialize()),
            true, Commitment.Confirmed);
    }

    private TransactionInstruction GetMoveRightInstruction(string password)
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        var wallet = walletHolderService.BaseWallet;

        MoveRightAccounts account = new MoveRightAccounts();
        account.GameDataAccount = gameDataAccount;
        account.ChestVault = chestVaultAccount;
        account.Player = wallet.Account.PublicKey;
        account.SystemProgram = SystemProgram.ProgramIdKey;

        TransactionInstruction initializeInstruction = TinyAdventureTwoProgram.MoveRight(account, password, ProgramId);
        return initializeInstruction;
    }

    private TransactionInstruction GetResetLevelAndSpawnChestInstruction()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        var wallet = walletHolderService.BaseWallet;

        ResetLevelAndSpawnChestAccounts accounts = new ResetLevelAndSpawnChestAccounts();
        accounts.GameDataAccount = gameDataAccount;
        accounts.ChestVault = chestVaultAccount;
        accounts.Payer = wallet.Account.PublicKey;
        accounts.SystemProgram = SystemProgram.ProgramIdKey;

        TransactionInstruction initializeInstruction = TinyAdventureTwoProgram.ResetLevelAndSpawnChest(accounts, ProgramId);
        return initializeInstruction;
    }

    private TransactionInstruction GetInitializeInstruction()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        var wallet = walletHolderService.BaseWallet;

        InitializeLevelOneAccounts account = new InitializeLevelOneAccounts();
        account.Signer = wallet.Account.PublicKey;
        account.NewGameDataAccount = gameDataAccount;
        account.ChestVault = chestVaultAccount;
        account.SystemProgram = SystemProgram.ProgramIdKey;

        TransactionInstruction initializeInstruction = TinyAdventureTwoProgram.InitializeLevelOne(account, ProgramId);
        return initializeInstruction;
    }
}