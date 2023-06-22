using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Frictionless;
using SevenSeas;
using SevenSeas.Accounts;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using SevenSeas.Program;
using SevenSeas.Types;
using Solana.Unity.Programs.Models;
using SolPlay.DeeplinksNftExample.Utils;
using SolPlay.Scripts.Services;
using SolPlay.Scripts.Ui;
using UnityEngine;
using Random = UnityEngine.Random;

public class SevenSeasService : MonoBehaviour
{
    public static PublicKey ProgramId = new PublicKey("2a4NcnkF5zf14JQXHAv39AsRf7jMFj13wKmTL6ZcDQNd");
    public static PublicKey GoldTokenMint = new PublicKey("goLdQwNaZToyavwkbuPJzTt5XPNR3H7WQBGenWtzPH3");
    public static PublicKey CannonTokenMint = new PublicKey("boomkN8rQpbgGAKcWvR3yyVVkjucNYcq7gTav78NQAG");
    public static PublicKey RumTokenMint = new PublicKey("rumwqxXmjKAmSdkfkc5qDpHTpETYJRyXY22DWYUmWDt");

    public static PublicKey ProgramGoldTokenPDA = new PublicKey("3SPDb9gFpkXxFkmn2NvqFvdSGDHDZuM1ByYetkuJDu7C");
    // Local urls: 
    // https://rpc-devnet.helius.xyz/?api-key=ac3a68f3-50cd-4346-8504-56a01b3f233e
    // http://localhost:8899
    // ws://localhost:8090
    
    public enum Direction
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
    }

    public static byte STATE_EMPTY = 0;
    public static byte STATE_PLAYER = 1;
    public static byte STATE_CHEST = 2;

    public static int TILE_COUNT_X = 10;
    public static int TILE_COUNT_Y = 10;

    public GameDataAccount CurrentGameData;
    public Ship CurrentShipData;

    public PublicKey gameDataAccount;

    private PublicKey chestVaultAccount;
    private PublicKey gameActionsAccount;
    private PublicKey tokenAccountOwnerPDA;
    private PublicKey vaultTokenAccountPDA;

    // This is a little trick to not get the same transaction hash when there are multiple of the same transactions in the 
    // with the same block hash. Since the would result in the same hash one would be dropped. So we just increase this byte 
    // and send it the with instruction. 
    private int InstructionBump;
    private Dictionary<ulong, GameAction> alreadyPrerformedGameActions = new Dictionary<ulong, GameAction>();
    private bool HasPlayerInitialAnimations = false;
    
    public class SolHunterGameDataChangedMessage
    {
        public GameDataAccount GameDataAccount;
    }
    
    public class SolHunterShipDataChangedMessage
    {
        public Ship Ship;
    }
    
    public class CthulhuAttackedMessage
    {
        public ShipBehaviour Ship;
        public ulong Damage;

        public CthulhuAttackedMessage(ShipBehaviour ship, ulong damage)
        {
            Ship = ship;
            Damage = damage;
        }
    }

    public class ShipShotMessage
    {
        public PublicKey ShipOwner;
        public ulong Damage;
    }

    public void OnEnable()
    {
        ServiceFactory.RegisterSingleton(this);
        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("level")
            },
            ProgramId, out gameDataAccount, out var bump);

        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("chestVault")
            },
            ProgramId, out chestVaultAccount, out var bumpChest);
        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("gameActions")
            },
            ProgramId, out gameActionsAccount, out var actionsBump);
        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("token_account_owner_pda")
            },
            ProgramId, out tokenAccountOwnerPDA, out var actionsBump23423);
        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("token_vault"),
                GoldTokenMint.KeyBytes
            },
            ProgramId, out vaultTokenAccountPDA, out var actionsBum234);
    }
    
    private void Start()
    {
        MessageRouter.AddHandler<SocketServerConnectedMessage>(OnSocketConnected);
    }

    private void OnSocketConnected(SocketServerConnectedMessage message)
    {
        GetGameData();
        GetGameActions();
        GetShipData();
        ServiceFactory.Resolve<SolPlayWebSocketService>().SubscribeToPubKeyData(gameDataAccount, result =>
        {
            GameDataAccount gameDataAccount =
                GameDataAccount.Deserialize(Convert.FromBase64String(result.result.value.data[0]));
            SetCachedGameData(gameDataAccount);
            MessageRouter.RaiseMessage(new SolHunterGameDataChangedMessage()
            {
                GameDataAccount = gameDataAccount
            });
        });
        ServiceFactory.Resolve<SolPlayWebSocketService>().SubscribeToPubKeyData(gameActionsAccount, result =>
        {
            GameActionHistory gameActionHistory =
                GameActionHistory.Deserialize(Convert.FromBase64String(result.result.value.data[0]));
            //Debug.Log("Actions len: " + gameActionHistory.GameActions.Length);
            OnNewGameActions(gameActionHistory);
        });
    }

    private void OnNewGameActions(GameActionHistory gameActionHistory)
    {
        if (!HasPlayerInitialAnimations)
        {
            foreach (GameAction gameAction in gameActionHistory.GameActions)
            {
                alreadyPrerformedGameActions.Add(gameAction.ActionId, gameAction);
            }

            HasPlayerInitialAnimations = true;
            return;
        }
        
        foreach (GameAction gameAction in gameActionHistory.GameActions)
        {
            if (!alreadyPrerformedGameActions.ContainsKey(gameAction.ActionId))
            {
                // Ship shot
                if (gameAction.ActionType == 0)
                {
                    MessageRouter.RaiseMessage(new ShipShotMessage()
                    {
                        ShipOwner = gameAction.Player,
                        Damage = gameAction.Damage
                    });
                }

                // Ship has taken damage
                if (gameAction.ActionType == 1)
                {
                    if (ServiceFactory.Resolve<ShipManager>()
                    .TryGetShipByOwner(gameAction.Target, out ShipBehaviour ship))
                    {
                        MessageRouter.RaiseMessage(new BlimpSystem.Show3DBlimpMessage("-"+gameAction.Damage, ship.transform.position));   
                    }
                }
                
                // Chutuluh has attacked a ship
                if (gameAction.ActionType == 2)
                {
                    if (ServiceFactory.Resolve<ShipManager>()
                        .TryGetShipByOwner(gameAction.Target, out ShipBehaviour ship))
                    {
                        MessageRouter.RaiseMessage(new CthulhuAttackedMessage(ship, gameAction.Damage));   
                    }
                }
                
                // Chest collected
                if (gameAction.ActionType == 3)
                {
                    if (ServiceFactory.Resolve<ShipManager>()
                        .TryGetShipByOwner(gameAction.Target, out ShipBehaviour ship))
                    {
                        MessageRouter.RaiseMessage(new BlimpSystem.Show3DBlimpMessage("+"+gameAction.Damage, ship.transform.position, BlimpSystem.BlimpType.CoinBlimp));   
                    }
                }
                
                alreadyPrerformedGameActions.Add(gameAction.ActionId, gameAction);
            }
        }
    }

    public async Task<Ship> GetShipData()
    {
        var baseWallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;
        var selectedNft = ServiceFactory.Resolve<NftService>().SelectedNft;
        if (selectedNft == null)
        {
            return null;
        }
        
        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("ship"),
                new PublicKey(selectedNft.metaplexData.data.mint).KeyBytes
            },
            ProgramId, out PublicKey shipPDA, out var actionsBum234);

        SevenSeasClient client =
            new SevenSeasClient(baseWallet.ActiveRpcClient, baseWallet.ActiveStreamingRpcClient, ProgramId);

        AccountResultWrapper<Ship> shipAccount = null;
        try
        {
            shipAccount = await client.GetShipAsync(shipPDA, Commitment.Confirmed);
            CurrentShipData = shipAccount.ParsedResult;
        }
        catch (Exception e)
        {
            Debug.Log("There is no ship yet: " + e);
            return null;
        }

        MessageRouter.RaiseMessage(new SolHunterShipDataChangedMessage()
        {
            Ship = shipAccount.ParsedResult
        });

        return shipAccount.ParsedResult;
    }

    public async Task<GameDataAccount> GetGameData()
    {
        var gameData = await ServiceFactory.Resolve<WalletHolderService>().InGameWallet.ActiveRpcClient
            .GetAccountInfoAsync(this.gameDataAccount, Commitment.Confirmed, BinaryEncoding.JsonParsed);
        if (gameData == null ||gameData.Result == null || gameData.Result.Value == null || !gameData.WasSuccessful)
        {
            return null;
        }
        GameDataAccount gameDataAccount =
            GameDataAccount.Deserialize(Convert.FromBase64String(gameData.Result.Value.Data[0]));
        SetCachedGameData(gameDataAccount);
        MessageRouter.RaiseMessage(new SolHunterGameDataChangedMessage()
        {
            GameDataAccount = gameDataAccount
        });
        return gameDataAccount;
    }

    public async Task<GameActionHistory> GetGameActions()
    {
        var gameData = await ServiceFactory.Resolve<WalletHolderService>().InGameWallet.ActiveRpcClient
            .GetAccountInfoAsync(this.gameActionsAccount, Commitment.Confirmed, BinaryEncoding.JsonParsed);
        if (gameData == null ||gameData.Result == null || gameData.Result.Value == null || !gameData.WasSuccessful)
        {
            return null;
        }
        GameActionHistory gameActionsHistory =
            GameActionHistory.Deserialize(Convert.FromBase64String(gameData.Result.Value.Data[0]));
        
        OnNewGameActions(gameActionsHistory);
        return gameActionsHistory;
    }

    private void SetCachedGameData(GameDataAccount gameDataAccount)
    {
        bool playerWasAlive = false;
        if (CurrentGameData != null)
        {
            playerWasAlive = IsPlayerSpawned();
        }

        CurrentGameData = gameDataAccount;
        bool playerAliveAfterData = IsPlayerSpawned();

        if (playerWasAlive && !playerAliveAfterData)
        {
            LoggingService.Log("You died :( ", true);
            ServiceFactory.Resolve<NftService>().ResetSelectedNft();
        }
        else
        {
            var avatar = TryGetSpawnedPlayerAvatar();
            if (avatar != null)
            {
                var avatarNft = ServiceFactory.Resolve<NftService>().GetNftByMintAddress(avatar);
                ServiceFactory.Resolve<NftService>().SelectNft(avatarNft);
            }
        }
    }

    public void Initialize()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        if (!walletHolderService.HasEnoughSol(false, 300000000))
        {
            Debug.LogError("Not enough sol");
            return;
        }

        TransactionInstruction initializeInstruction = GetInitializeInstruction();
        ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock("Initialize", initializeInstruction,
            walletHolderService.BaseWallet);
    }

    public void Reset()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        if (!walletHolderService.HasEnoughSol(false, 300000))
        {
            return;
        }

        TransactionInstruction initializeInstruction = GetResetInstruction();
        ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock("Reset", initializeInstruction,
            walletHolderService.InGameWallet);
    }

    public void ResetShip()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        if (!walletHolderService.HasEnoughSol(false, 10000))
        {
            return;
        }

        TransactionInstruction initializeInstruction = GetResetShipInstruction();
        ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock("Reset Ship", initializeInstruction,
            walletHolderService.BaseWallet);
    }

    public void Move(Direction direction)
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        if (!walletHolderService.HasEnoughSol(true, 10000))
        {
            return;
        }

        TransactionInstruction initializeInstruction = GetMovePlayerInstruction((byte) direction);
        ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock($"Move{direction}",
            initializeInstruction, walletHolderService.InGameWallet);
    }

    public void Shoot()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        if (!walletHolderService.HasEnoughSol(true, 10000))
        {
            return;
        }

        TransactionInstruction initializeInstruction = GetShootInstruction();
        ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock($"Shoot",
            initializeInstruction, walletHolderService.InGameWallet);
    }

     public async void UpgradeShip()
    {
        long costForSpawn = (long) 10000;
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        if (!walletHolderService.HasEnoughSol(false, costForSpawn))
        {
            return;
        }

        LoggingService.Log("Spawn player and chest", true);

        TransactionInstruction upgradeShip = GetUpgradeShipInstruction();
        ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock("Upgrade ship", upgradeShip,
            walletHolderService.BaseWallet,
            s =>
            {
                //GetGameData(); not needed since we rely on the socket connection
                GetGameData();
                GetShipData();
            });
    }

     public async void SpawnPlayerAndChest()
     {
         long costForSpawn = (long) (0.11f * SolanaUtils.SolToLamports);
         var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
         if (!walletHolderService.HasEnoughSol(false, costForSpawn))
         {
             return;
         }
         
         var baseWallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;
         var ingameWallet = ServiceFactory.Resolve<WalletHolderService>().InGameWallet;
         var baseWalletAddress = baseWallet.Account.PublicKey;

         var playerTokenAccount =
             AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(baseWalletAddress, GoldTokenMint);

         var tokenAccount = await baseWallet.ActiveRpcClient.GetAccountInfoAsync(playerTokenAccount);
         List<TransactionInstruction> instructions = new List<TransactionInstruction>();

         if (!tokenAccount.WasSuccessful || tokenAccount.Result.Value == null)
         {
             /*var createTokenAccount = AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                 baseWalletAddress,
                 baseWalletAddress,
                 GoldTokenMint);
             instructions.Add(createTokenAccount);*/
         }
         
         // If the player doesnt have much sol left in his auto approve wallet fill it up automatically
         long autoApproveCost = (long) (0.01f * SolanaUtils.SolToLamports);
         if (walletHolderService.InGameWalletSolBalance < autoApproveCost)
         {
             var transactionInstruction = SystemProgram.Transfer(baseWallet.Account.PublicKey,
                 ingameWallet.Account.PublicKey, (ulong) (long) (0.05f * SolanaUtils.SolToLamports));
             instructions.Add(transactionInstruction);
         }

         LoggingService.Log("Spawn player and chest", true);

         var ship = await GetShipData();

         if (ship == null)
         {
             TransactionInstruction initShip = GetInitShipInstruction();
             instructions.Add(initShip);
         }

         TransactionInstruction initializeInstruction = GetSpawnPlayerAndChestInstruction();
         instructions.Add(initializeInstruction);
         ServiceFactory.Resolve<TransactionService>().SendInstructionsInNextBlock("Spawn player", instructions,
            walletHolderService.BaseWallet,
            s =>
            {
                //GetGameData(); not needed since we rely on the socket connection
                GetGameData();
            });
    }

    public void InitShip()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();

        TransactionInstruction initShip = GetInitShipInstruction();
        ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock("Init ship", initShip,
            walletHolderService.BaseWallet,
            s =>
            {
                //GetGameData(); not needed since we rely on the socket connection
                GetGameData();
                GetShipData();
            });
    }

    private TransactionInstruction GetShootInstruction()
    {
        var ingameWalletAddress = ServiceFactory.Resolve<WalletHolderService>().InGameWallet.Account.PublicKey;
        var baseWalletAddress = ServiceFactory.Resolve<WalletHolderService>().BaseWallet.Account.PublicKey;

        var playerTokenAccount =
            AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(baseWalletAddress, GoldTokenMint);
        
        ShootAccounts accounts = new ShootAccounts();
        accounts.GameDataAccount = gameDataAccount;
        accounts.ChestVault = chestVaultAccount;
        accounts.GameActions = gameActionsAccount;
        accounts.Player = ServiceFactory.Resolve<WalletHolderService>().InGameWallet.Account.PublicKey;
        accounts.TokenAccountOwner = baseWalletAddress;
        accounts.PlayerTokenAccount = playerTokenAccount;
        accounts.VaultTokenAccount = vaultTokenAccountPDA;
        accounts.TokenAccountOwnerPda = tokenAccountOwnerPDA;
        accounts.TokenProgram = TokenProgram.ProgramIdKey;
        accounts.MintOfTokenBeingSent = GoldTokenMint;
        accounts.AssociatedTokenProgram = AssociatedTokenAccountProgram.ProgramIdKey;
        accounts.SystemProgram = SystemProgram.ProgramIdKey;

        TransactionInstruction initializeInstruction =
            SevenSeasProgram.Shoot(accounts, GetInstructionBump(), ProgramId);
        return initializeInstruction;
    }

    private TransactionInstruction GetChutuluhInstruction()
    {
        var baseWalletAddress = ServiceFactory.Resolve<WalletHolderService>().BaseWallet.Account.PublicKey;

        var playerTokenAccount =
            AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(baseWalletAddress, GoldTokenMint);
        
        CthulhuAccounts accounts = new CthulhuAccounts();
        accounts.GameDataAccount = gameDataAccount;
        accounts.ChestVault = chestVaultAccount;
        accounts.GameActions = gameActionsAccount;
        accounts.TokenAccountOwner = baseWalletAddress;
        accounts.Player = ServiceFactory.Resolve<WalletHolderService>().InGameWallet.Account.PublicKey;
        accounts.PlayerTokenAccount = playerTokenAccount;
        accounts.VaultTokenAccount = vaultTokenAccountPDA;
        accounts.TokenAccountOwnerPda = tokenAccountOwnerPDA;
        accounts.TokenProgram = TokenProgram.ProgramIdKey;
        accounts.MintOfTokenBeingSent = GoldTokenMint;
        accounts.AssociatedTokenProgram = AssociatedTokenAccountProgram.ProgramIdKey;
        accounts.SystemProgram = SystemProgram.ProgramIdKey;

        TransactionInstruction initializeInstruction =
            SevenSeasProgram.Cthulhu(accounts, GetInstructionBump(), ProgramId);
        return initializeInstruction;
    }
    
    private TransactionInstruction GetMovePlayerInstruction(byte direction)
    {
        var ingameWalletAddress = ServiceFactory.Resolve<WalletHolderService>().InGameWallet.Account.PublicKey;
        var baseWalletAddress = ServiceFactory.Resolve<WalletHolderService>().BaseWallet.Account.PublicKey;

        var playerTokenAccount =
            AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(baseWalletAddress, GoldTokenMint);

        MovePlayerV2Accounts accounts = new MovePlayerV2Accounts();
        accounts.GameDataAccount = gameDataAccount;
        accounts.ChestVault = chestVaultAccount;
        accounts.SystemProgram = SystemProgram.ProgramIdKey;
        accounts.Player = ServiceFactory.Resolve<WalletHolderService>().InGameWallet.Account.PublicKey;
        accounts.PlayerTokenAccount = playerTokenAccount;
        accounts.TokenAccountOwner = baseWalletAddress;
        accounts.VaultTokenAccount = vaultTokenAccountPDA;
        accounts.TokenAccountOwnerPda = tokenAccountOwnerPDA;
        accounts.TokenProgram = TokenProgram.ProgramIdKey;
        accounts.MintOfTokenBeingSent = GoldTokenMint;
        accounts.AssociatedTokenProgram = AssociatedTokenAccountProgram.ProgramIdKey;
        accounts.GameActions = gameActionsAccount;

        TransactionInstruction initializeInstruction =
            SevenSeasProgram.MovePlayerV2(accounts, direction, GetInstructionBump(), ProgramId);
        return initializeInstruction;
    }

    private byte GetInstructionBump()
    {
        return (byte) (InstructionBump++ % 127);
    }

    private TransactionInstruction GetInitializeInstruction()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        var wallet = walletHolderService.InGameWallet;

        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("token_account_owner_pda")
            },
            ProgramId, out PublicKey tokenAccountOwnerPda, out var bump);

        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("token_vault"),
                GoldTokenMint.KeyBytes
            },
            ProgramId, out PublicKey token_vault, out var bump2);
        
        var accounts = new InitializeAccounts();
        accounts.Signer = wallet.Account.PublicKey;
        accounts.NewGameDataAccount = gameDataAccount;
        accounts.SystemProgram = SystemProgram.ProgramIdKey;
        accounts.ChestVault = chestVaultAccount;
        accounts.GameActions = gameActionsAccount;
        accounts.TokenProgram = TokenProgram.ProgramIdKey;
        accounts.VaultTokenAccount = token_vault;
        accounts.MintOfTokenBeingSent = GoldTokenMint;
        accounts.TokenAccountOwnerPda = tokenAccountOwnerPda;

        TransactionInstruction initializeInstruction = SevenSeasProgram.Initialize(accounts, ProgramId);
        return initializeInstruction;
    }

    private TransactionInstruction GetResetInstruction()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();

        var accounts = new ResetAccounts();
        accounts.Signer = walletHolderService.InGameWallet.Account.PublicKey;
        accounts.GameDataAccount = gameDataAccount;

        TransactionInstruction resetInstruction = SevenSeasProgram.Reset(accounts, ProgramId);
        return resetInstruction;
    }

    private TransactionInstruction GetResetShipInstruction()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();

        var accounts = new ResetShipAccounts();
        accounts.Signer = walletHolderService.BaseWallet.Account.PublicKey;
        accounts.GameDataAccount = gameDataAccount;

        TransactionInstruction resetInstruction = SevenSeasProgram.ResetShip(accounts, ProgramId);
        return resetInstruction;
    }

    public TransactionInstruction GetSpawnPlayerAndChestInstruction()
    {
        var selectedNft = ServiceFactory.Resolve<NftService>().SelectedNft;
        if (selectedNft == null)
        {
            LoggingService.Log("Nft is still loading...", true);
            return null;
        }
        var playerCannonTokenAccount =
            AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(ServiceFactory.Resolve<WalletHolderService>().BaseWallet.Account.PublicKey, CannonTokenMint);
        var playerRumTokenAccount =
            AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(ServiceFactory.Resolve<WalletHolderService>().BaseWallet.Account.PublicKey, RumTokenMint);

        SpawnPlayerAccounts accounts = new SpawnPlayerAccounts();
        accounts.GameDataAccount = gameDataAccount;
        accounts.ChestVault = chestVaultAccount;
        accounts.SystemProgram = SystemProgram.ProgramIdKey;
        accounts.Player = ServiceFactory.Resolve<WalletHolderService>().InGameWallet.Account.PublicKey;
        accounts.TokenAccountOwner = ServiceFactory.Resolve<WalletHolderService>().BaseWallet.Account.PublicKey;
        accounts.NftAccount = new PublicKey(ServiceFactory.Resolve<NftService>().SelectedNft.metaplexData.data.mint);
        accounts.CannonMint = CannonTokenMint;
        accounts.CannonTokenAccount = playerCannonTokenAccount;
        accounts.RumMint = RumTokenMint;
        accounts.RumTokenAccount = playerRumTokenAccount;
        accounts.AssociatedTokenProgram = AssociatedTokenAccountProgram.ProgramIdKey;
        accounts.TokenProgram = TokenProgram.ProgramIdKey;
        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("ship"),
                accounts.NftAccount.KeyBytes
            },
            ProgramId, out PublicKey shipPda, out var bump);
        accounts.Ship = shipPda;
        
        TransactionInstruction initializeInstruction =
            SevenSeasProgram.SpawnPlayer(accounts, new PublicKey(selectedNft.metaplexData.data.mint), ProgramId);
        return initializeInstruction;
    }

    public TransactionInstruction GetInitShipInstruction()
    {
        var selectedNft = ServiceFactory.Resolve<NftService>().SelectedNft;
        if (selectedNft == null)
        {
            LoggingService.Log("Nft is still loading...", true);
            return null;
        }

        InitializeShipAccounts accounts = new InitializeShipAccounts();
        accounts.SystemProgram = SystemProgram.ProgramIdKey;
        accounts.NftAccount = new PublicKey(ServiceFactory.Resolve<NftService>().SelectedNft.metaplexData.data.mint);
        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("ship"),
                accounts.NftAccount.KeyBytes
            },
            ProgramId, out PublicKey shipPda, out var bump);
        accounts.NewShip = shipPda;
        accounts.Signer = ServiceFactory.Resolve<WalletHolderService>().BaseWallet.Account.PublicKey;

        TransactionInstruction initializeInstruction = SevenSeasProgram.InitializeShip(accounts, ProgramId);
        return initializeInstruction;
    }
    
    public TransactionInstruction GetUpgradeShipInstruction()
    {
        var selectedNft = ServiceFactory.Resolve<NftService>().SelectedNft;
        if (selectedNft == null)
        {
            LoggingService.Log("Nft is still loading...", true);
            return null;
        }

        UpgradeShipAccounts accounts = new UpgradeShipAccounts();
        accounts.NftAccount = new PublicKey(ServiceFactory.Resolve<NftService>().SelectedNft.metaplexData.data.mint);
        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("ship"),
                accounts.NftAccount.KeyBytes
            },
            ProgramId, out PublicKey shipPda, out var bump);
        PublicKey.TryFindProgramAddress(new[]
            {
                Encoding.UTF8.GetBytes("token_vault"),
                GoldTokenMint.KeyBytes
            },
            ProgramId, out PublicKey token_vault, out var bump2);

        var baseWalletAddress = ServiceFactory.Resolve<WalletHolderService>().BaseWallet.Account.PublicKey;

        var playerTokenAccount =
            AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(baseWalletAddress, GoldTokenMint);

        accounts.NewShip = shipPda;
        accounts.Signer = ServiceFactory.Resolve<WalletHolderService>().BaseWallet.Account.PublicKey;
        accounts.SystemProgram = SystemProgram.ProgramIdKey;
        accounts.PlayerTokenAccount = playerTokenAccount;
        accounts.VaultTokenAccount = token_vault;
        accounts.TokenProgram = TokenProgram.ProgramIdKey;
        accounts.MintOfTokenBeingSent = GoldTokenMint;

        TransactionInstruction initializeInstruction = SevenSeasProgram.UpgradeShip(accounts, ProgramId);
        return initializeInstruction;
    }

    public PublicKey TryGetSpawnedPlayerAvatar()
    {
        if (CurrentGameData == null)
        {
            return null;
        }

        var localWallet = ServiceFactory.Resolve<WalletHolderService>().InGameWallet.Account.PublicKey.Key;
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                var tile = CurrentGameData.Board[y][x];
                if (tile.Player == localWallet && tile.State == STATE_PLAYER)
                {
                    return tile.Avatar;
                }
            }
        }

        return null;
    }

    public bool IsPlayerSpawned()
    {
        if (CurrentGameData == null)
        {
            return false;
        }

        var localWallet = ServiceFactory.Resolve<WalletHolderService>().InGameWallet.Account.PublicKey.Key;
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                var tile = CurrentGameData.Board[y][x];
                if (tile.Player == localWallet && tile.State == STATE_PLAYER)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void Chutuluh()
    {
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
        if (!walletHolderService.HasEnoughSol(true, 10000))
        {
            return;
        }

        TransactionInstruction initializeInstruction = GetChutuluhInstruction();
        ServiceFactory.Resolve<TransactionService>().SendInstructionInNextBlock($"Chutuluh",
            initializeInstruction, walletHolderService.InGameWallet);
    }

    public int GetShipUpgradeCost(ushort upgrades)
    {
        switch (upgrades)
        {
            case 0:
                return 5;
            case 1:
                return 200;
            case 2:
                return 1500;
            case 3:
                return 25000;
            default:
                return 9999999;
        }
    }
}