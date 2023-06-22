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
using TinyAdventure.Accounts;
using TinyAdventure.Program;
using UnityEngine;

public class TinyAdventureService : MonoBehaviour
{
    public static PublicKey ProgramId = new PublicKey("tinyiK8HcU7WuLW6tfss8PJqpcVzS5Ce9M9ATXEhJGr");

    private PublicKey gameDataAccount;

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
    }

    private void Start()
    {
        MessageRouter.AddHandler<SocketServerConnectedMessage>(OnSocketConnected);
    }

    private void OnSocketConnected(SocketServerConnectedMessage message)
    {
        ServiceFactory.Resolve<SolPlayWebSocketService>().SubscribeToPubKeyData(gameDataAccount, result =>
        {
            GameDataAccount gameDataAccount = GameDataAccount.Deserialize(Convert.FromBase64String(result.result.value.data[0]));
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
        GameDataAccount gameDataAccount = GameDataAccount.Deserialize(Convert.FromBase64String(gameData.Result.Value.Data[0]));
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
        ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock("Initializes",initializeInstruction, walletHolderService.BaseWallet, result => {});
    }

    public void MoveRight()
    {
        TransactionInstruction initializeInstruction = GetMoveRightInstruction();
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock("Move Right", initializeInstruction, walletHolderService.BaseWallet, result => {});
    }

    public void MoveLeft()
    {
        TransactionInstruction initializeInstruction = GetMoveLeftInstruction();
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock("Move Left", initializeInstruction, walletHolderService.BaseWallet, result => {});
    }

    /// <summary>
    /// This is how you can build a transaction without using the transaction service.
    /// The transaction service just removes some code duplication and does error handling and checks the
    /// signature
    /// </summary>
    public async void MoveLeftManual()
    {
        TransactionInstruction moveLeftInstruction = GetMoveLeftInstruction();
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        var result = await walletHolderService.BaseWallet.ActiveRpcClient.GetRecentBlockHashAsync(Commitment.Confirmed);
        
        Transaction transaction = new Transaction();
        transaction.FeePayer = walletHolderService.BaseWallet.Account.PublicKey;
        transaction.RecentBlockHash = result.Result.Value.Blockhash;
        transaction.Signatures = new List<SignaturePubKeyPair>();
        transaction.Instructions = new List<TransactionInstruction>();
        transaction.Instructions.Add(moveLeftInstruction);

        Transaction signedTransaction = await walletHolderService.BaseWallet.SignTransaction(transaction);

        RequestResult<string> signature = await walletHolderService.BaseWallet.ActiveRpcClient.SendTransactionAsync(
            Convert.ToBase64String(signedTransaction.Serialize()),
            true, Commitment.Confirmed);
    }
    
    private TransactionInstruction GetMoveLeftInstruction()
    {
        MoveLeftAccounts account = new MoveLeftAccounts();
        account.GameDataAccount = gameDataAccount;

        TransactionInstruction initializeInstruction = TinyAdventureProgram.MoveLeft(account, ProgramId);
        return initializeInstruction;
    }

    private TransactionInstruction GetMoveRightInstruction()
    {
        MoveRightAccounts account = new MoveRightAccounts();
        account.GameDataAccount = gameDataAccount;

        TransactionInstruction initializeInstruction = TinyAdventureProgram.MoveRight(account, ProgramId);
        return initializeInstruction;
    }

    private TransactionInstruction GetInitializeInstruction()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        var wallet = walletHolderService.BaseWallet;

        InitializeAccounts account = new InitializeAccounts();
        account.Signer = wallet.Account.PublicKey;
        account.NewGameDataAccount = gameDataAccount;
        account.SystemProgram = SystemProgram.ProgramIdKey;

        TransactionInstruction initializeInstruction = TinyAdventureProgram.Initialize(account, ProgramId);
        return initializeInstruction;
    }
}