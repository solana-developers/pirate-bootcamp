using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Solana.Unity;
using Solana.Unity.Programs.Abstract;
using Solana.Unity.Programs.Utilities;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Core.Sockets;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using SevenSeas;
using SevenSeas.Program;
using SevenSeas.Errors;
using SevenSeas.Accounts;
using SevenSeas.Types;

namespace SevenSeas
{
    namespace Accounts
    {
        public partial class GameDataAccount
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 2830422829680616787UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{83, 229, 68, 63, 145, 174, 71, 39};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "F2tkBUcrpHt";
            public Tile[][] Board { get; set; }

            public ulong ActionId { get; set; }

            public static GameDataAccount Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                GameDataAccount result = new GameDataAccount();
                result.Board = new Tile[10][];
                for (uint resultBoardIdx = 0; resultBoardIdx < 10; resultBoardIdx++)
                {
                    result.Board[resultBoardIdx] = new Tile[10];
                    for (uint resultBoardresultBoardIdxIdx = 0; resultBoardresultBoardIdxIdx < 10; resultBoardresultBoardIdxIdx++)
                    {
                        offset += Tile.Deserialize(_data, offset, out var resultBoardresultBoardIdxresultBoardresultBoardIdxIdx);
                        result.Board[resultBoardIdx][resultBoardresultBoardIdxIdx] = resultBoardresultBoardIdxresultBoardresultBoardIdxIdx;
                    }
                }

                result.ActionId = _data.GetU64(offset);
                offset += 8;
                return result;
            }
        }

        public partial class GameActionHistory
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 8873408368832920456UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{136, 187, 67, 235, 229, 173, 36, 123};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "PsU5aGMBQbU";
            public ulong IdCounter { get; set; }

            public GameAction[] GameActions { get; set; }

            public static GameActionHistory Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                GameActionHistory result = new GameActionHistory();
                result.IdCounter = _data.GetU64(offset);
                offset += 8;
                int resultGameActionsLength = (int)_data.GetU32(offset);
                offset += 4;
                result.GameActions = new GameAction[resultGameActionsLength];
                for (uint resultGameActionsIdx = 0; resultGameActionsIdx < resultGameActionsLength; resultGameActionsIdx++)
                {
                    offset += GameAction.Deserialize(_data, offset, out var resultGameActionsresultGameActionsIdx);
                    result.GameActions[resultGameActionsIdx] = resultGameActionsresultGameActionsIdx;
                }

                return result;
            }
        }

        public partial class ChestVaultAccount
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 9406927803919968769UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{1, 42, 101, 100, 255, 30, 140, 130};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "CJrgsQPtJV";
            public static ChestVaultAccount Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                ChestVaultAccount result = new ChestVaultAccount();
                return result;
            }
        }

        public partial class Ship
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 11451028881204914546UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{114, 41, 245, 232, 24, 58, 234, 158};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "L6Xuk4LKTnd";
            public ulong Health { get; set; }

            public ushort Kills { get; set; }

            public ulong Cannons { get; set; }

            public ushort Upgrades { get; set; }

            public ushort Xp { get; set; }

            public ushort Level { get; set; }

            public ulong StartHealth { get; set; }

            public static Ship Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                Ship result = new Ship();
                result.Health = _data.GetU64(offset);
                offset += 8;
                result.Kills = _data.GetU16(offset);
                offset += 2;
                result.Cannons = _data.GetU64(offset);
                offset += 8;
                result.Upgrades = _data.GetU16(offset);
                offset += 2;
                result.Xp = _data.GetU16(offset);
                offset += 2;
                result.Level = _data.GetU16(offset);
                offset += 2;
                result.StartHealth = _data.GetU64(offset);
                offset += 8;
                return result;
            }
        }
    }

    namespace Errors
    {
        public enum SevenSeasErrorKind : uint
        {
            TileOutOfBounds = 6000U,
            BoardIsFull = 6001U,
            PlayerAlreadyExists = 6002U,
            TriedToMovePlayerThatWasNotOnTheBoard = 6003U,
            TriedToShootWithPlayerThatWasNotOnTheBoard = 6004U,
            WrongDirectionInput = 6005U,
            MaxShipLevelReached = 6006U,
            CouldNotFindAShipToAttack = 6007U
        }
    }

    namespace Types
    {
        public partial class Tile
        {
            public PublicKey Player { get; set; }

            public byte State { get; set; }

            public ulong Health { get; set; }

            public ulong Damage { get; set; }

            public ushort Range { get; set; }

            public ulong CollectReward { get; set; }

            public PublicKey Avatar { get; set; }

            public byte LookDirection { get; set; }

            public ushort ShipLevel { get; set; }

            public ulong StartHealth { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WritePubKey(Player, offset);
                offset += 32;
                _data.WriteU8(State, offset);
                offset += 1;
                _data.WriteU64(Health, offset);
                offset += 8;
                _data.WriteU64(Damage, offset);
                offset += 8;
                _data.WriteU16(Range, offset);
                offset += 2;
                _data.WriteU64(CollectReward, offset);
                offset += 8;
                _data.WritePubKey(Avatar, offset);
                offset += 32;
                _data.WriteU8(LookDirection, offset);
                offset += 1;
                _data.WriteU16(ShipLevel, offset);
                offset += 2;
                _data.WriteU64(StartHealth, offset);
                offset += 8;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out Tile result)
            {
                int offset = initialOffset;
                result = new Tile();
                result.Player = _data.GetPubKey(offset);
                offset += 32;
                result.State = _data.GetU8(offset);
                offset += 1;
                result.Health = _data.GetU64(offset);
                offset += 8;
                result.Damage = _data.GetU64(offset);
                offset += 8;
                result.Range = _data.GetU16(offset);
                offset += 2;
                result.CollectReward = _data.GetU64(offset);
                offset += 8;
                result.Avatar = _data.GetPubKey(offset);
                offset += 32;
                result.LookDirection = _data.GetU8(offset);
                offset += 1;
                result.ShipLevel = _data.GetU16(offset);
                offset += 2;
                result.StartHealth = _data.GetU64(offset);
                offset += 8;
                return offset - initialOffset;
            }
        }

        public partial class GameAction
        {
            public ulong ActionId { get; set; }

            public byte ActionType { get; set; }

            public PublicKey Player { get; set; }

            public PublicKey Target { get; set; }

            public ulong Damage { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU64(ActionId, offset);
                offset += 8;
                _data.WriteU8(ActionType, offset);
                offset += 1;
                _data.WritePubKey(Player, offset);
                offset += 32;
                _data.WritePubKey(Target, offset);
                offset += 32;
                _data.WriteU64(Damage, offset);
                offset += 8;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out GameAction result)
            {
                int offset = initialOffset;
                result = new GameAction();
                result.ActionId = _data.GetU64(offset);
                offset += 8;
                result.ActionType = _data.GetU8(offset);
                offset += 1;
                result.Player = _data.GetPubKey(offset);
                offset += 32;
                result.Target = _data.GetPubKey(offset);
                offset += 32;
                result.Damage = _data.GetU64(offset);
                offset += 8;
                return offset - initialOffset;
            }
        }
    }

    public partial class SevenSeasClient : TransactionalBaseClient<SevenSeasErrorKind>
    {
        public SevenSeasClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient, PublicKey programId) : base(rpcClient, streamingRpcClient, programId)
        {
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameDataAccount>>> GetGameDataAccountsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = GameDataAccount.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameDataAccount>>(res);
            List<GameDataAccount> resultingAccounts = new List<GameDataAccount>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => GameDataAccount.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameDataAccount>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameActionHistory>>> GetGameActionHistorysAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = GameActionHistory.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameActionHistory>>(res);
            List<GameActionHistory> resultingAccounts = new List<GameActionHistory>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => GameActionHistory.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameActionHistory>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<ChestVaultAccount>>> GetChestVaultAccountsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = ChestVaultAccount.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<ChestVaultAccount>>(res);
            List<ChestVaultAccount> resultingAccounts = new List<ChestVaultAccount>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => ChestVaultAccount.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<ChestVaultAccount>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Ship>>> GetShipsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = Ship.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Ship>>(res);
            List<Ship> resultingAccounts = new List<Ship>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Ship.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Ship>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<GameDataAccount>> GetGameDataAccountAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<GameDataAccount>(res);
            var resultingAccount = GameDataAccount.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<GameDataAccount>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<GameActionHistory>> GetGameActionHistoryAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<GameActionHistory>(res);
            var resultingAccount = GameActionHistory.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<GameActionHistory>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<ChestVaultAccount>> GetChestVaultAccountAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<ChestVaultAccount>(res);
            var resultingAccount = ChestVaultAccount.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<ChestVaultAccount>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<Ship>> GetShipAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<Ship>(res);
            var resultingAccount = Ship.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<Ship>(res, resultingAccount);
        }

        public async Task<SubscriptionState> SubscribeGameDataAccountAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, GameDataAccount> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                GameDataAccount parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = GameDataAccount.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeGameActionHistoryAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, GameActionHistory> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                GameActionHistory parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = GameActionHistory.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeChestVaultAccountAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, ChestVaultAccount> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                ChestVaultAccount parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = ChestVaultAccount.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeShipAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, Ship> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                Ship parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = Ship.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<RequestResult<string>> SendInitializeAsync(InitializeAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.Initialize(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendInitializeShipAsync(InitializeShipAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.InitializeShip(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendUpgradeShipAsync(UpgradeShipAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.UpgradeShip(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendResetAsync(ResetAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.Reset(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendResetShipAsync(ResetShipAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.ResetShip(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendStartThreadAsync(StartThreadAccounts accounts, byte[] threadId, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.StartThread(accounts, threadId, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendPauseThreadAsync(PauseThreadAccounts accounts, byte[] threadId, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.PauseThread(accounts, threadId, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendResumeThreadAsync(ResumeThreadAccounts accounts, byte[] threadId, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.ResumeThread(accounts, threadId, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendOnThreadTickAsync(OnThreadTickAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.OnThreadTick(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSpawnPlayerAsync(SpawnPlayerAccounts accounts, PublicKey avatar, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.SpawnPlayer(accounts, avatar, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendCthulhuAsync(CthulhuAccounts accounts, byte blockBump, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.Cthulhu(accounts, blockBump, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendShootAsync(ShootAccounts accounts, byte blockBump, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.Shoot(accounts, blockBump, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendMovePlayerV2Async(MovePlayerV2Accounts accounts, byte direction, byte blockBump, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.SevenSeasProgram.MovePlayerV2(accounts, direction, blockBump, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        protected override Dictionary<uint, ProgramError<SevenSeasErrorKind>> BuildErrorsDictionary()
        {
            return new Dictionary<uint, ProgramError<SevenSeasErrorKind>>{};
        }
    }

    namespace Program
    {
        public class InitializeAccounts
        {
            public PublicKey Signer { get; set; }

            public PublicKey NewGameDataAccount { get; set; }

            public PublicKey ChestVault { get; set; }

            public PublicKey GameActions { get; set; }

            public PublicKey TokenAccountOwnerPda { get; set; }

            public PublicKey VaultTokenAccount { get; set; }

            public PublicKey MintOfTokenBeingSent { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class InitializeShipAccounts
        {
            public PublicKey Signer { get; set; }

            public PublicKey NewShip { get; set; }

            public PublicKey NftAccount { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class UpgradeShipAccounts
        {
            public PublicKey Signer { get; set; }

            public PublicKey NewShip { get; set; }

            public PublicKey NftAccount { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey PlayerTokenAccount { get; set; }

            public PublicKey VaultTokenAccount { get; set; }

            public PublicKey MintOfTokenBeingSent { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class ResetAccounts
        {
            public PublicKey Signer { get; set; }

            public PublicKey GameDataAccount { get; set; }
        }

        public class ResetShipAccounts
        {
            public PublicKey Signer { get; set; }

            public PublicKey GameDataAccount { get; set; }
        }

        public class StartThreadAccounts
        {
            public PublicKey GameDataAccount { get; set; }

            public PublicKey ClockworkProgram { get; set; }

            public PublicKey Payer { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey Thread { get; set; }

            public PublicKey ThreadAuthority { get; set; }
        }

        public class PauseThreadAccounts
        {
            public PublicKey Payer { get; set; }

            public PublicKey ClockworkProgram { get; set; }

            public PublicKey Thread { get; set; }

            public PublicKey ThreadAuthority { get; set; }
        }

        public class ResumeThreadAccounts
        {
            public PublicKey Payer { get; set; }

            public PublicKey ClockworkProgram { get; set; }

            public PublicKey Thread { get; set; }

            public PublicKey ThreadAuthority { get; set; }
        }

        public class OnThreadTickAccounts
        {
            public PublicKey GameData { get; set; }

            public PublicKey Thread { get; set; }

            public PublicKey ThreadAuthority { get; set; }
        }

        public class SpawnPlayerAccounts
        {
            public PublicKey Player { get; set; }

            public PublicKey TokenAccountOwner { get; set; }

            public PublicKey ChestVault { get; set; }

            public PublicKey GameDataAccount { get; set; }

            public PublicKey Ship { get; set; }

            public PublicKey NftAccount { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey CannonTokenAccount { get; set; }

            public PublicKey CannonMint { get; set; }

            public PublicKey RumTokenAccount { get; set; }

            public PublicKey RumMint { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }
        }

        public class CthulhuAccounts
        {
            public PublicKey ChestVault { get; set; }

            public PublicKey GameDataAccount { get; set; }

            public PublicKey GameActions { get; set; }

            public PublicKey Player { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenAccountOwner { get; set; }

            public PublicKey PlayerTokenAccount { get; set; }

            public PublicKey VaultTokenAccount { get; set; }

            public PublicKey TokenAccountOwnerPda { get; set; }

            public PublicKey MintOfTokenBeingSent { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }
        }

        public class ShootAccounts
        {
            public PublicKey ChestVault { get; set; }

            public PublicKey GameDataAccount { get; set; }

            public PublicKey GameActions { get; set; }

            public PublicKey Player { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenAccountOwner { get; set; }

            public PublicKey PlayerTokenAccount { get; set; }

            public PublicKey VaultTokenAccount { get; set; }

            public PublicKey TokenAccountOwnerPda { get; set; }

            public PublicKey MintOfTokenBeingSent { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }
        }

        public class MovePlayerV2Accounts
        {
            public PublicKey ChestVault { get; set; }

            public PublicKey GameDataAccount { get; set; }

            public PublicKey Player { get; set; }

            public PublicKey TokenAccountOwner { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey PlayerTokenAccount { get; set; }

            public PublicKey VaultTokenAccount { get; set; }

            public PublicKey TokenAccountOwnerPda { get; set; }

            public PublicKey MintOfTokenBeingSent { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }

            public PublicKey GameActions { get; set; }
        }

        public static class SevenSeasProgram
        {
            public static Solana.Unity.Rpc.Models.TransactionInstruction Initialize(InitializeAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.NewGameDataAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ChestVault, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameActions, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenAccountOwnerPda, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.VaultTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.MintOfTokenBeingSent, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(17121445590508351407UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction InitializeShip(InitializeShipAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.NewShip, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.NftAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(15946413791577830052UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction UpgradeShip(UpgradeShipAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.NewShip, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.NftAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PlayerTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.VaultTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.MintOfTokenBeingSent, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(3599852141310396080UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction Reset(ResetAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameDataAccount, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(15488080923286262039UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction ResetShip(ResetShipAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameDataAccount, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(9955096133398885290UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction StartThread(StartThreadAccounts accounts, byte[] threadId, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameDataAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ClockworkProgram, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Payer, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Thread, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ThreadAuthority, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(15046500361448809729UL, offset);
                offset += 8;
                _data.WriteS32(threadId.Length, offset);
                offset += 4;
                _data.WriteSpan(threadId, offset);
                offset += threadId.Length;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction PauseThread(PauseThreadAccounts accounts, byte[] threadId, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Payer, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ClockworkProgram, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Thread, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ThreadAuthority, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(8210672603296534645UL, offset);
                offset += 8;
                _data.WriteS32(threadId.Length, offset);
                offset += 4;
                _data.WriteSpan(threadId, offset);
                offset += threadId.Length;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction ResumeThread(ResumeThreadAccounts accounts, byte[] threadId, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Payer, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ClockworkProgram, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Thread, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ThreadAuthority, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(5675786826980878817UL, offset);
                offset += 8;
                _data.WriteS32(threadId.Length, offset);
                offset += 4;
                _data.WriteSpan(threadId, offset);
                offset += threadId.Length;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction OnThreadTick(OnThreadTickAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameData, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Thread, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ThreadAuthority, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(5812526563082725654UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SpawnPlayer(SpawnPlayerAccounts accounts, PublicKey avatar, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Player, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenAccountOwner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ChestVault, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameDataAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Ship, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.NftAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.CannonTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.CannonMint, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.RumTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RumMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(3695486382544324736UL, offset);
                offset += 8;
                _data.WritePubKey(avatar, offset);
                offset += 32;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction Cthulhu(CthulhuAccounts accounts, byte blockBump, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ChestVault, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameDataAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameActions, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Player, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenAccountOwner, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PlayerTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.VaultTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenAccountOwnerPda, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.MintOfTokenBeingSent, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(1430635477224443476UL, offset);
                offset += 8;
                _data.WriteU8(blockBump, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction Shoot(ShootAccounts accounts, byte blockBump, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ChestVault, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameDataAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameActions, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Player, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenAccountOwner, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PlayerTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.VaultTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenAccountOwnerPda, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.MintOfTokenBeingSent, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(7423935530772343593UL, offset);
                offset += 8;
                _data.WriteU8(blockBump, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction MovePlayerV2(MovePlayerV2Accounts accounts, byte direction, byte blockBump, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ChestVault, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameDataAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Player, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenAccountOwner, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PlayerTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.VaultTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenAccountOwnerPda, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.MintOfTokenBeingSent, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameActions, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(15560957635555335921UL, offset);
                offset += 8;
                _data.WriteU8(direction, offset);
                offset += 1;
                _data.WriteU8(blockBump, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }
        }
    }
}