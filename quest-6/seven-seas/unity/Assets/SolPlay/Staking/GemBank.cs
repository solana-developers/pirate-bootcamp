using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using GemBank.Accounts;
using GemBank.Errors;
using GemBank.Program;
using GemFarm.Program;
using GemFarm.Errors;
using GemFarm.Accounts;
using GemFarm.Types;
using Solana.Unity.Programs.Abstract;
using Solana.Unity.Programs.Models;
using Solana.Unity.Programs.Utilities;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Core.Sockets;
using Solana.Unity.Rpc.Messages;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using RewardType = GemFarm.Types.RewardType;

namespace GemBank
{
    namespace Accounts
    {
        public partial class Bank
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 13574203538458161550UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{142, 49, 166, 242, 50, 66, 97, 188};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "QnTef4UXSzF";
            public ushort Version { get; set; }

            public PublicKey BankManager { get; set; }

            public uint Flags { get; set; }

            public uint WhitelistedCreators { get; set; }

            public uint WhitelistedMints { get; set; }

            public ulong VaultCount { get; set; }

            public byte[] Reserved { get; set; }

            public static Bank Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                Bank result = new Bank();
                result.Version = _data.GetU16(offset);
                offset += 2;
                result.BankManager = _data.GetPubKey(offset);
                offset += 32;
                result.Flags = _data.GetU32(offset);
                offset += 4;
                result.WhitelistedCreators = _data.GetU32(offset);
                offset += 4;
                result.WhitelistedMints = _data.GetU32(offset);
                offset += 4;
                result.VaultCount = _data.GetU64(offset);
                offset += 8;
                result.Reserved = _data.GetBytes(offset, 64);
                offset += 64;
                return result;
            }
        }

        public partial class GemDepositReceipt
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 13481704650073353942UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{214, 174, 90, 58, 243, 162, 24, 187};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "cugAXF7JS7L";
            public PublicKey Vault { get; set; }

            public PublicKey GemBoxAddress { get; set; }

            public PublicKey GemMint { get; set; }

            public ulong GemCount { get; set; }

            public byte[] Reserved { get; set; }

            public static GemDepositReceipt Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                GemDepositReceipt result = new GemDepositReceipt();
                result.Vault = _data.GetPubKey(offset);
                offset += 32;
                result.GemBoxAddress = _data.GetPubKey(offset);
                offset += 32;
                result.GemMint = _data.GetPubKey(offset);
                offset += 32;
                result.GemCount = _data.GetU64(offset);
                offset += 8;
                result.Reserved = _data.GetBytes(offset, 32);
                offset += 32;
                return result;
            }
        }

        public partial class Rarity
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 3845974823817967121UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{17, 246, 195, 180, 185, 165, 95, 53};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "41GouGRdMkg";
            public ushort Points { get; set; }

            public static Rarity Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                Rarity result = new Rarity();
                result.Points = _data.GetU16(offset);
                offset += 2;
                return result;
            }
        }

        public partial class Vault
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 8607953397882554579UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{211, 8, 232, 43, 2, 152, 117, 119};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "cJJWPqNMczr";
            public PublicKey Bank { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey Creator { get; set; }

            public PublicKey Authority { get; set; }

            public PublicKey AuthoritySeed { get; set; }

            public byte[] AuthorityBumpSeed { get; set; }

            public bool Locked { get; set; }

            public byte[] Name { get; set; }

            public ulong GemBoxCount { get; set; }

            public ulong GemCount { get; set; }

            public ulong RarityPoints { get; set; }

            public byte[] Reserved { get; set; }

            public static Vault Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                Vault result = new Vault();
                result.Bank = _data.GetPubKey(offset);
                offset += 32;
                result.Owner = _data.GetPubKey(offset);
                offset += 32;
                result.Creator = _data.GetPubKey(offset);
                offset += 32;
                result.Authority = _data.GetPubKey(offset);
                offset += 32;
                result.AuthoritySeed = _data.GetPubKey(offset);
                offset += 32;
                result.AuthorityBumpSeed = _data.GetBytes(offset, 1);
                offset += 1;
                result.Locked = _data.GetBool(offset);
                offset += 1;
                result.Name = _data.GetBytes(offset, 32);
                offset += 32;
                result.GemBoxCount = _data.GetU64(offset);
                offset += 8;
                result.GemCount = _data.GetU64(offset);
                offset += 8;
                result.RarityPoints = _data.GetU64(offset);
                offset += 8;
                result.Reserved = _data.GetBytes(offset, 64);
                offset += 64;
                return result;
            }
        }

        public partial class WhitelistProof
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 2876782271992227522UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{194, 230, 60, 10, 60, 98, 236, 39};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "ZbmY7scC3Mp";
            public byte WhitelistType { get; set; }

            public PublicKey WhitelistedAddress { get; set; }

            public PublicKey Bank { get; set; }

            public static WhitelistProof Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                WhitelistProof result = new WhitelistProof();
                result.WhitelistType = _data.GetU8(offset);
                offset += 1;
                result.WhitelistedAddress = _data.GetPubKey(offset);
                offset += 32;
                result.Bank = _data.GetPubKey(offset);
                offset += 32;
                return result;
            }
        }
    }

    namespace Errors
    {
        public enum GemBankErrorKind : uint
        {
        }
    }

    namespace Types
    {
        public partial class RarityConfig
        {
            public PublicKey Mint { get; set; }

            public ushort RarityPoints { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WritePubKey(Mint, offset);
                offset += 32;
                _data.WriteU16(RarityPoints, offset);
                offset += 2;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out RarityConfig result)
            {
                int offset = initialOffset;
                result = new RarityConfig();
                result.Mint = _data.GetPubKey(offset);
                offset += 32;
                result.RarityPoints = _data.GetU16(offset);
                offset += 2;
                return offset - initialOffset;
            }
        }
    }

    public partial class GemBankClient : TransactionalBaseClient<GemBankErrorKind>
    {
        public GemBankClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient, PublicKey programId) : base(rpcClient, streamingRpcClient, programId)
        {
        }

        public async Task<ProgramAccountsResultWrapper<List<Bank>>> GetBanksAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<MemCmp>{new MemCmp{Bytes = Bank.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new ProgramAccountsResultWrapper<List<Bank>>(res);
            List<Bank> resultingAccounts = new List<Bank>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Bank.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new ProgramAccountsResultWrapper<List<Bank>>(res, resultingAccounts);
        }

        public async Task<ProgramAccountsResultWrapper<List<GemDepositReceipt>>> GetGemDepositReceiptsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<MemCmp>{new MemCmp{Bytes = GemDepositReceipt.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new ProgramAccountsResultWrapper<List<GemDepositReceipt>>(res);
            List<GemDepositReceipt> resultingAccounts = new List<GemDepositReceipt>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => GemDepositReceipt.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new ProgramAccountsResultWrapper<List<GemDepositReceipt>>(res, resultingAccounts);
        }

        public async Task<ProgramAccountsResultWrapper<List<Rarity>>> GetRaritysAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<MemCmp>{new MemCmp{Bytes = Rarity.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new ProgramAccountsResultWrapper<List<Rarity>>(res);
            List<Rarity> resultingAccounts = new List<Rarity>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Rarity.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new ProgramAccountsResultWrapper<List<Rarity>>(res, resultingAccounts);
        }

        public async Task<ProgramAccountsResultWrapper<List<Vault>>> GetVaultsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<MemCmp>{new MemCmp{Bytes = Vault.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new ProgramAccountsResultWrapper<List<Vault>>(res);
            List<Vault> resultingAccounts = new List<Vault>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Vault.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new ProgramAccountsResultWrapper<List<Vault>>(res, resultingAccounts);
        }

        public async Task<ProgramAccountsResultWrapper<List<WhitelistProof>>> GetWhitelistProofsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<MemCmp>{new MemCmp{Bytes = WhitelistProof.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new ProgramAccountsResultWrapper<List<WhitelistProof>>(res);
            List<WhitelistProof> resultingAccounts = new List<WhitelistProof>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => WhitelistProof.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new ProgramAccountsResultWrapper<List<WhitelistProof>>(res, resultingAccounts);
        }

        public async Task<AccountResultWrapper<Bank>> GetBankAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new AccountResultWrapper<Bank>(res);
            var resultingAccount = Bank.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new AccountResultWrapper<Bank>(res, resultingAccount);
        }

        public async Task<AccountResultWrapper<GemDepositReceipt>> GetGemDepositReceiptAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new AccountResultWrapper<GemDepositReceipt>(res);
            var resultingAccount = GemDepositReceipt.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new AccountResultWrapper<GemDepositReceipt>(res, resultingAccount);
        }

        public async Task<AccountResultWrapper<Rarity>> GetRarityAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new AccountResultWrapper<Rarity>(res);
            var resultingAccount = Rarity.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new AccountResultWrapper<Rarity>(res, resultingAccount);
        }

        public async Task<AccountResultWrapper<Vault>> GetVaultAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new AccountResultWrapper<Vault>(res);
            var resultingAccount = Vault.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new AccountResultWrapper<Vault>(res, resultingAccount);
        }

        public async Task<AccountResultWrapper<WhitelistProof>> GetWhitelistProofAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new AccountResultWrapper<WhitelistProof>(res);
            var resultingAccount = WhitelistProof.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new AccountResultWrapper<WhitelistProof>(res, resultingAccount);
        }

        public async Task<SubscriptionState> SubscribeBankAsync(string accountAddress, Action<SubscriptionState, ResponseValue<AccountInfo>, Bank> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                Bank parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = Bank.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeGemDepositReceiptAsync(string accountAddress, Action<SubscriptionState, ResponseValue<AccountInfo>, GemDepositReceipt> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                GemDepositReceipt parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = GemDepositReceipt.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeRarityAsync(string accountAddress, Action<SubscriptionState, ResponseValue<AccountInfo>, Rarity> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                Rarity parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = Rarity.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeVaultAsync(string accountAddress, Action<SubscriptionState, ResponseValue<AccountInfo>, Vault> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                Vault parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = Vault.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeWhitelistProofAsync(string accountAddress, Action<SubscriptionState, ResponseValue<AccountInfo>, WhitelistProof> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                WhitelistProof parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = WhitelistProof.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<RequestResult<string>> SendInitBankAsync(InitBankAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemBankProgram.InitBank(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetBankFlagsAsync(SetBankFlagsAccounts accounts, uint flags, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemBankProgram.SetBankFlags(accounts, flags, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendInitVaultAsync(InitVaultAccounts accounts, PublicKey owner, string name, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemBankProgram.InitVault(accounts, owner, name, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetVaultLockAsync(SetVaultLockAccounts accounts, bool vaultLock, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemBankProgram.SetVaultLock(accounts, vaultLock, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendUpdateVaultOwnerAsync(UpdateVaultOwnerAccounts accounts, PublicKey newOwner, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemBankProgram.UpdateVaultOwner(accounts, newOwner, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendDepositGemAsync(DepositGemAccounts accounts, byte bumpAuth, byte bumpRarity, ulong amount, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemBankProgram.DepositGem(accounts, bumpAuth, bumpRarity, amount, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendWithdrawGemAsync(WithdrawGemAccounts accounts, byte bumpAuth, byte bumpGemBox, byte bumpGdr, byte bumpRarity, ulong amount, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemBankProgram.WithdrawGem(accounts, bumpAuth, bumpGemBox, bumpGdr, bumpRarity, amount, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendAddToWhitelistAsync(AddToWhitelistAccounts accounts, byte whitelistType, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemBankProgram.AddToWhitelist(accounts, whitelistType, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRemoveFromWhitelistAsync(RemoveFromWhitelistAccounts accounts, byte bump, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemBankProgram.RemoveFromWhitelist(accounts, bump, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendUpdateBankManagerAsync(UpdateBankManagerAccounts accounts, PublicKey newManager, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemBankProgram.UpdateBankManager(accounts, newManager, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRecordRarityPointsAsync(RecordRarityPointsAccounts accounts, RarityConfig[] rarityConfigs, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemBankProgram.RecordRarityPoints(accounts, rarityConfigs, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        protected override Dictionary<uint, ProgramError<GemBankErrorKind>> BuildErrorsDictionary()
        {
            return new Dictionary<uint, ProgramError<GemBankErrorKind>>{};
        }
    }

    namespace Program
    {
        public class InitBankAccounts
        {
            public PublicKey Bank { get; set; }

            public PublicKey BankManager { get; set; }

            public PublicKey Payer { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class SetBankFlagsAccounts
        {
            public PublicKey Bank { get; set; }

            public PublicKey BankManager { get; set; }
        }

        public class InitVaultAccounts
        {
            public PublicKey Bank { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey Creator { get; set; }

            public PublicKey Payer { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class SetVaultLockAccounts
        {
            public PublicKey Bank { get; set; }

            public PublicKey BankManager { get; set; }

            public PublicKey Vault { get; set; }
        }

        public class UpdateVaultOwnerAccounts
        {
            public PublicKey Bank { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey Owner { get; set; }
        }

        public class DepositGemAccounts
        {
            public PublicKey Bank { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey Authority { get; set; }

            public PublicKey GemBox { get; set; }

            public PublicKey GemDepositReceipt { get; set; }

            public PublicKey GemSource { get; set; }

            public PublicKey GemMint { get; set; }

            public PublicKey GemRarity { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class WithdrawGemAccounts
        {
            public PublicKey Bank { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey Authority { get; set; }

            public PublicKey GemBox { get; set; }

            public PublicKey GemDepositReceipt { get; set; }

            public PublicKey GemDestination { get; set; }

            public PublicKey GemMint { get; set; }

            public PublicKey GemRarity { get; set; }

            public PublicKey Receiver { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class AddToWhitelistAccounts
        {
            public PublicKey Bank { get; set; }

            public PublicKey BankManager { get; set; }

            public PublicKey AddressToWhitelist { get; set; }

            public PublicKey WhitelistProof { get; set; }

            public PublicKey Payer { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class RemoveFromWhitelistAccounts
        {
            public PublicKey Bank { get; set; }

            public PublicKey BankManager { get; set; }

            public PublicKey FundsReceiver { get; set; }

            public PublicKey AddressToRemove { get; set; }

            public PublicKey WhitelistProof { get; set; }
        }

        public class UpdateBankManagerAccounts
        {
            public PublicKey Bank { get; set; }

            public PublicKey BankManager { get; set; }
        }

        public class RecordRarityPointsAccounts
        {
            public PublicKey Bank { get; set; }

            public PublicKey BankManager { get; set; }

            public PublicKey Payer { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public static class GemBankProgram
        {
            public static TransactionInstruction InitBank(InitBankAccounts accounts, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Bank, true), AccountMeta.ReadOnly(accounts.BankManager, true), AccountMeta.Writable(accounts.Payer, true), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(5809504752993267529UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction SetBankFlags(SetBankFlagsAccounts accounts, uint flags, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Bank, false), AccountMeta.ReadOnly(accounts.BankManager, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(4617862328513415535UL, offset);
                offset += 8;
                _data.WriteU32(flags, offset);
                offset += 4;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction InitVault(InitVaultAccounts accounts, PublicKey owner, string name, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Bank, false), AccountMeta.Writable(accounts.Vault, false), AccountMeta.ReadOnly(accounts.Creator, true), AccountMeta.Writable(accounts.Payer, true), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(7652980405088636749UL, offset);
                offset += 8;
                _data.WritePubKey(owner, offset);
                offset += 32;
                offset += _data.WriteBorshString(name, offset);
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction SetVaultLock(SetVaultLockAccounts accounts, bool vaultLock, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.ReadOnly(accounts.Bank, false), AccountMeta.ReadOnly(accounts.BankManager, true), AccountMeta.Writable(accounts.Vault, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(9865180796695945732UL, offset);
                offset += 8;
                _data.WriteBool(vaultLock, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction UpdateVaultOwner(UpdateVaultOwnerAccounts accounts, PublicKey newOwner, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.ReadOnly(accounts.Bank, false), AccountMeta.Writable(accounts.Vault, false), AccountMeta.ReadOnly(accounts.Owner, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(6581210188386450043UL, offset);
                offset += 8;
                _data.WritePubKey(newOwner, offset);
                offset += 32;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction DepositGem(DepositGemAccounts accounts, byte bumpAuth, byte bumpRarity, ulong amount, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.ReadOnly(accounts.Bank, false), AccountMeta.Writable(accounts.Vault, false), AccountMeta.Writable(accounts.Owner, true), AccountMeta.ReadOnly(accounts.Authority, false), AccountMeta.Writable(accounts.GemBox, false), AccountMeta.Writable(accounts.GemDepositReceipt, false), AccountMeta.Writable(accounts.GemSource, false), AccountMeta.ReadOnly(accounts.GemMint, false), AccountMeta.ReadOnly(accounts.GemRarity, false), AccountMeta.ReadOnly(accounts.TokenProgram, false), AccountMeta.ReadOnly(accounts.SystemProgram, false), AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(17284335414940532991UL, offset);
                offset += 8;
                _data.WriteU8(bumpAuth, offset);
                offset += 1;
                _data.WriteU8(bumpRarity, offset);
                offset += 1;
                _data.WriteU64(amount, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction WithdrawGem(WithdrawGemAccounts accounts, byte bumpAuth, byte bumpGemBox, byte bumpGdr, byte bumpRarity, ulong amount, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.ReadOnly(accounts.Bank, false), AccountMeta.Writable(accounts.Vault, false), AccountMeta.Writable(accounts.Owner, true), AccountMeta.ReadOnly(accounts.Authority, false), AccountMeta.Writable(accounts.GemBox, false), AccountMeta.Writable(accounts.GemDepositReceipt, false), AccountMeta.Writable(accounts.GemDestination, false), AccountMeta.ReadOnly(accounts.GemMint, false), AccountMeta.ReadOnly(accounts.GemRarity, false), AccountMeta.Writable(accounts.Receiver, false), AccountMeta.ReadOnly(accounts.TokenProgram, false), AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false), AccountMeta.ReadOnly(accounts.SystemProgram, false), AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(1396909096445614567UL, offset);
                offset += 8;
                _data.WriteU8(bumpAuth, offset);
                offset += 1;
                _data.WriteU8(bumpGemBox, offset);
                offset += 1;
                _data.WriteU8(bumpGdr, offset);
                offset += 1;
                _data.WriteU8(bumpRarity, offset);
                offset += 1;
                _data.WriteU64(amount, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction AddToWhitelist(AddToWhitelistAccounts accounts, byte whitelistType, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Bank, false), AccountMeta.ReadOnly(accounts.BankManager, true), AccountMeta.ReadOnly(accounts.AddressToWhitelist, false), AccountMeta.Writable(accounts.WhitelistProof, false), AccountMeta.Writable(accounts.Payer, true), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(3964664726796161949UL, offset);
                offset += 8;
                _data.WriteU8(whitelistType, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction RemoveFromWhitelist(RemoveFromWhitelistAccounts accounts, byte bump, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Bank, false), AccountMeta.ReadOnly(accounts.BankManager, true), AccountMeta.Writable(accounts.FundsReceiver, false), AccountMeta.ReadOnly(accounts.AddressToRemove, false), AccountMeta.Writable(accounts.WhitelistProof, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16988119801863376903UL, offset);
                offset += 8;
                _data.WriteU8(bump, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction UpdateBankManager(UpdateBankManagerAccounts accounts, PublicKey newManager, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Bank, false), AccountMeta.ReadOnly(accounts.BankManager, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(1129668442566628873UL, offset);
                offset += 8;
                _data.WritePubKey(newManager, offset);
                offset += 32;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction RecordRarityPoints(RecordRarityPointsAccounts accounts, RarityConfig[] rarityConfigs, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.ReadOnly(accounts.Bank, false), AccountMeta.ReadOnly(accounts.BankManager, true), AccountMeta.Writable(accounts.Payer, true), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(14827787719381918023UL, offset);
                offset += 8;
                _data.WriteS32(rarityConfigs.Length, offset);
                offset += 4;
                foreach (var rarityConfigsElement in rarityConfigs)
                {
                    offset += rarityConfigsElement.Serialize(_data, offset);
                }

                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }
        }
    }
}