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
using TinyAdventureTwo;
using TinyAdventureTwo.Program;
using TinyAdventureTwo.Errors;
using TinyAdventureTwo.Accounts;

namespace TinyAdventureTwo
{
    namespace Accounts
    {
        public partial class GameDataAccount
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 2830422829680616787UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{83, 229, 68, 63, 145, 174, 71, 39};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "F2tkBUcrpHt";
            public byte PlayerPosition { get; set; }

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
                result.PlayerPosition = _data.GetU8(offset);
                offset += 1;
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
    }

    namespace Errors
    {
        public enum TinyAdventureTwoErrorKind : uint
        {
            WrongPassword = 6000U
        }
    }

    public partial class TinyAdventureTwoClient : TransactionalBaseClient<TinyAdventureTwoErrorKind>
    {
        public TinyAdventureTwoClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient, PublicKey programId) : base(rpcClient, streamingRpcClient, programId)
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

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<GameDataAccount>> GetGameDataAccountAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<GameDataAccount>(res);
            var resultingAccount = GameDataAccount.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<GameDataAccount>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<ChestVaultAccount>> GetChestVaultAccountAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<ChestVaultAccount>(res);
            var resultingAccount = ChestVaultAccount.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<ChestVaultAccount>(res, resultingAccount);
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

        public async Task<RequestResult<string>> SendInitializeLevelOneAsync(InitializeLevelOneAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TinyAdventureTwoProgram.InitializeLevelOne(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendResetLevelAndSpawnChestAsync(ResetLevelAndSpawnChestAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TinyAdventureTwoProgram.ResetLevelAndSpawnChest(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendMoveRightAsync(MoveRightAccounts accounts, string password, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TinyAdventureTwoProgram.MoveRight(accounts, password, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        protected override Dictionary<uint, ProgramError<TinyAdventureTwoErrorKind>> BuildErrorsDictionary()
        {
            return new Dictionary<uint, ProgramError<TinyAdventureTwoErrorKind>>{{6000U, new ProgramError<TinyAdventureTwoErrorKind>(TinyAdventureTwoErrorKind.WrongPassword, "Password was wrong")}, };
        }
    }

    namespace Program
    {
        public class InitializeLevelOneAccounts
        {
            public PublicKey NewGameDataAccount { get; set; }

            public PublicKey ChestVault { get; set; }

            public PublicKey Signer { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class ResetLevelAndSpawnChestAccounts
        {
            public PublicKey Payer { get; set; }

            public PublicKey ChestVault { get; set; }

            public PublicKey GameDataAccount { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class MoveRightAccounts
        {
            public PublicKey ChestVault { get; set; }

            public PublicKey GameDataAccount { get; set; }

            public PublicKey Player { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public static class TinyAdventureTwoProgram
        {
            public static Solana.Unity.Rpc.Models.TransactionInstruction InitializeLevelOne(InitializeLevelOneAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.NewGameDataAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ChestVault, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(17344740784559472625UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction ResetLevelAndSpawnChest(ResetLevelAndSpawnChestAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Payer, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ChestVault, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameDataAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(3341080949523873196UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction MoveRight(MoveRightAccounts accounts, string password, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ChestVault, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.GameDataAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Player, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(10990983061962034633UL, offset);
                offset += 8;
                offset += _data.WriteBorshString(password, offset);
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }
        }
    }
}