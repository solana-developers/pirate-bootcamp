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
using ShadowDriveUserStaking;
using ShadowDriveUserStaking.Program;
using ShadowDriveUserStaking.Errors;
using ShadowDriveUserStaking.Accounts;
using ShadowDriveUserStaking.Types;

namespace ShadowDriveUserStaking
{
    namespace Accounts
    {
        public partial class UnstakeInfo
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 14311229206436695075UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{35, 204, 218, 156, 35, 179, 155, 198};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "6zJoNm8Xqvd";
            public long TimeLastUnstaked { get; set; }

            public ulong EpochLastUnstaked { get; set; }

            public PublicKey Unstaker { get; set; }

            public static UnstakeInfo Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                UnstakeInfo result = new UnstakeInfo();
                result.TimeLastUnstaked = _data.GetS64(offset);
                offset += 8;
                result.EpochLastUnstaked = _data.GetU64(offset);
                offset += 8;
                result.Unstaker = _data.GetPubKey(offset);
                offset += 32;
                return result;
            }
        }

        public partial class StorageAccount
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 16991321729293299753UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{41, 48, 231, 194, 22, 77, 205, 235};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "7tc53amxRvJ";
            public bool IsStatic { get; set; }

            public uint InitCounter { get; set; }

            public uint DelCounter { get; set; }

            public bool Immutable { get; set; }

            public bool ToBeDeleted { get; set; }

            public uint DeleteRequestEpoch { get; set; }

            public ulong Storage { get; set; }

            public ulong StorageAvailable { get; set; }

            public PublicKey Owner1 { get; set; }

            public PublicKey Owner2 { get; set; }

            public PublicKey ShdwPayer { get; set; }

            public uint AccountCounterSeed { get; set; }

            public ulong TotalCostOfCurrentStorage { get; set; }

            public ulong TotalFeesPaid { get; set; }

            public uint CreationTime { get; set; }

            public uint CreationEpoch { get; set; }

            public uint LastFeeEpoch { get; set; }

            public string Identifier { get; set; }

            public static StorageAccount Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                StorageAccount result = new StorageAccount();
                result.IsStatic = _data.GetBool(offset);
                offset += 1;
                result.InitCounter = _data.GetU32(offset);
                offset += 4;
                result.DelCounter = _data.GetU32(offset);
                offset += 4;
                result.Immutable = _data.GetBool(offset);
                offset += 1;
                result.ToBeDeleted = _data.GetBool(offset);
                offset += 1;
                result.DeleteRequestEpoch = _data.GetU32(offset);
                offset += 4;
                result.Storage = _data.GetU64(offset);
                offset += 8;
                result.StorageAvailable = _data.GetU64(offset);
                offset += 8;
                result.Owner1 = _data.GetPubKey(offset);
                offset += 32;
                result.Owner2 = _data.GetPubKey(offset);
                offset += 32;
                result.ShdwPayer = _data.GetPubKey(offset);
                offset += 32;
                result.AccountCounterSeed = _data.GetU32(offset);
                offset += 4;
                result.TotalCostOfCurrentStorage = _data.GetU64(offset);
                offset += 8;
                result.TotalFeesPaid = _data.GetU64(offset);
                offset += 8;
                result.CreationTime = _data.GetU32(offset);
                offset += 4;
                result.CreationEpoch = _data.GetU32(offset);
                offset += 4;
                result.LastFeeEpoch = _data.GetU32(offset);
                offset += 4;
                offset += _data.GetBorshString(offset, out var resultIdentifier);
                result.Identifier = resultIdentifier;
                return result;
            }
        }

        public partial class StorageAccountV2
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 15765138380070663557UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{133, 53, 253, 82, 212, 5, 201, 218};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "PHK8U4HuGVf";
            public bool Immutable { get; set; }

            public bool ToBeDeleted { get; set; }

            public uint DeleteRequestEpoch { get; set; }

            public ulong Storage { get; set; }

            public PublicKey Owner1 { get; set; }

            public uint AccountCounterSeed { get; set; }

            public uint CreationTime { get; set; }

            public uint CreationEpoch { get; set; }

            public uint LastFeeEpoch { get; set; }

            public string Identifier { get; set; }

            public static StorageAccountV2 Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                StorageAccountV2 result = new StorageAccountV2();
                result.Immutable = _data.GetBool(offset);
                offset += 1;
                result.ToBeDeleted = _data.GetBool(offset);
                offset += 1;
                result.DeleteRequestEpoch = _data.GetU32(offset);
                offset += 4;
                result.Storage = _data.GetU64(offset);
                offset += 8;
                result.Owner1 = _data.GetPubKey(offset);
                offset += 32;
                result.AccountCounterSeed = _data.GetU32(offset);
                offset += 4;
                result.CreationTime = _data.GetU32(offset);
                offset += 4;
                result.CreationEpoch = _data.GetU32(offset);
                offset += 4;
                result.LastFeeEpoch = _data.GetU32(offset);
                offset += 4;
                offset += _data.GetBorshString(offset, out var resultIdentifier);
                result.Identifier = resultIdentifier;
                return result;
            }
        }

        public partial class UserInfoShadow
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 4470447772197750355UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{83, 134, 200, 56, 144, 56, 10, 62};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "EyK5FU8iAff";
            public uint AccountCounter { get; set; }

            public uint DelCounter { get; set; }

            public bool AgreedToTos { get; set; }

            public bool LifetimeBadCsam { get; set; }

            public static UserInfoShadow Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                UserInfoShadow result = new UserInfoShadow();
                result.AccountCounter = _data.GetU32(offset);
                offset += 4;
                result.DelCounter = _data.GetU32(offset);
                offset += 4;
                result.AgreedToTos = _data.GetBool(offset);
                offset += 1;
                result.LifetimeBadCsam = _data.GetBool(offset);
                offset += 1;
                return result;
            }
        }

        public partial class StorageConfig
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 14506299954658969690UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{90, 136, 182, 122, 243, 186, 80, 201};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "G9J2UU82ide";
            public ulong ShadesPerGib { get; set; }

            public BigInteger StorageAvailable { get; set; }

            public PublicKey TokenAccount { get; set; }

            public PublicKey Admin2 { get; set; }

            public PublicKey Uploader { get; set; }

            public uint? MutableFeeStartEpoch { get; set; }

            public ulong ShadesPerGibPerEpoch { get; set; }

            public ushort CrankBps { get; set; }

            public ulong MaxAccountSize { get; set; }

            public ulong MinAccountSize { get; set; }

            public static StorageConfig Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                StorageConfig result = new StorageConfig();
                result.ShadesPerGib = _data.GetU64(offset);
                offset += 8;
                result.StorageAvailable = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.TokenAccount = _data.GetPubKey(offset);
                offset += 32;
                result.Admin2 = _data.GetPubKey(offset);
                offset += 32;
                result.Uploader = _data.GetPubKey(offset);
                offset += 32;
                if (_data.GetBool(offset++))
                {
                    result.MutableFeeStartEpoch = _data.GetU32(offset);
                    offset += 4;
                }

                result.ShadesPerGibPerEpoch = _data.GetU64(offset);
                offset += 8;
                result.CrankBps = _data.GetU16(offset);
                offset += 2;
                result.MaxAccountSize = _data.GetU64(offset);
                offset += 8;
                result.MinAccountSize = _data.GetU64(offset);
                offset += 8;
                return result;
            }
        }

        public partial class File
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 18172141602764327722UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{42, 139, 221, 240, 129, 106, 48, 252};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "87kfu1G6uc7";
            public bool Immutable { get; set; }

            public bool ToBeDeleted { get; set; }

            public uint DeleteRequestEpoch { get; set; }

            public ulong Size { get; set; }

            public byte[] Sha256Hash { get; set; }

            public uint InitCounterSeed { get; set; }

            public PublicKey StorageAccount { get; set; }

            public string Name { get; set; }

            public static File Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                File result = new File();
                result.Immutable = _data.GetBool(offset);
                offset += 1;
                result.ToBeDeleted = _data.GetBool(offset);
                offset += 1;
                result.DeleteRequestEpoch = _data.GetU32(offset);
                offset += 4;
                result.Size = _data.GetU64(offset);
                offset += 8;
                result.Sha256Hash = _data.GetBytes(offset, 32);
                offset += 32;
                result.InitCounterSeed = _data.GetU32(offset);
                offset += 4;
                result.StorageAccount = _data.GetPubKey(offset);
                offset += 32;
                offset += _data.GetBorshString(offset, out var resultName);
                result.Name = resultName;
                return result;
            }
        }
    }

    namespace Errors
    {
        public enum ShadowDriveUserStakingErrorKind : uint
        {
            NotEnoughStorage = 6000U,
            FileNameLengthExceedsLimit = 6001U,
            InvalidSha256Hash = 6002U,
            HasHadBadCsam = 6003U,
            StorageAccountMarkedImmutable = 6004U,
            ClaimingStakeTooSoon = 6005U,
            SolanaStorageAccountNotMutable = 6006U,
            RemovingTooMuchStorage = 6007U,
            UnsignedIntegerCastFailed = 6008U,
            NonzeroRemainingFileAccounts = 6009U,
            AccountStillInGracePeriod = 6010U,
            AccountNotMarkedToBeDeleted = 6011U,
            FileStillInGracePeriod = 6012U,
            FileNotMarkedToBeDeleted = 6013U,
            FileMarkedImmutable = 6014U,
            NoStorageIncrease = 6015U,
            ExceededStorageLimit = 6016U,
            InsufficientFunds = 6017U,
            NotEnoughStorageOnShadowDrive = 6018U,
            AccountTooSmall = 6019U,
            DidNotAgreeToToS = 6020U,
            InvalidTokenTransferAmounts = 6021U,
            FailedToCloseAccount = 6022U,
            FailedToTransferToEmissionsWallet = 6023U,
            FailedToTransferToEmissionsWalletFromUser = 6024U,
            FailedToReturnUserFunds = 6025U,
            NeedSomeFees = 6026U,
            NeedSomeCrankBps = 6027U,
            AlreadyMarkedForDeletion = 6028U,
            EmptyStakeAccount = 6029U,
            IdentifierExceededMaxLength = 6030U,
            OnlyAdmin1CanChangeAdmins = 6031U,
            OnlyOneOwnerAllowedInV15 = 6032U
        }
    }

    namespace Types
    {
    }

    public partial class ShadowDriveUserStakingClient : TransactionalBaseClient<ShadowDriveUserStakingErrorKind>
    {
        public ShadowDriveUserStakingClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient, PublicKey programId) : base(rpcClient, streamingRpcClient, programId)
        {
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<UnstakeInfo>>> GetUnstakeInfosAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = UnstakeInfo.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<UnstakeInfo>>(res);
            List<UnstakeInfo> resultingAccounts = new List<UnstakeInfo>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => UnstakeInfo.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<UnstakeInfo>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<StorageAccount>>> GetStorageAccountsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = StorageAccount.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<StorageAccount>>(res);
            List<StorageAccount> resultingAccounts = new List<StorageAccount>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => StorageAccount.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<StorageAccount>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<StorageAccountV2>>> GetStorageAccountV2sAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = StorageAccountV2.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<StorageAccountV2>>(res);
            List<StorageAccountV2> resultingAccounts = new List<StorageAccountV2>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => StorageAccountV2.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<StorageAccountV2>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<UserInfoShadow>>> GetUserInfoShadowsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = UserInfoShadow.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<UserInfoShadow>>(res);
            List<UserInfoShadow> resultingAccounts = new List<UserInfoShadow>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => UserInfoShadow.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<UserInfoShadow>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<StorageConfig>>> GetStorageConfigsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = StorageConfig.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<StorageConfig>>(res);
            List<StorageConfig> resultingAccounts = new List<StorageConfig>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => StorageConfig.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<StorageConfig>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<File>>> GetFilesAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = File.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<File>>(res);
            List<File> resultingAccounts = new List<File>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => File.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<File>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<UnstakeInfo>> GetUnstakeInfoAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<UnstakeInfo>(res);
            var resultingAccount = UnstakeInfo.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<UnstakeInfo>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<StorageAccount>> GetStorageAccountAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<StorageAccount>(res);
            var resultingAccount = StorageAccount.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<StorageAccount>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<StorageAccountV2>> GetStorageAccountV2Async(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<StorageAccountV2>(res);
            var resultingAccount = StorageAccountV2.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<StorageAccountV2>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<UserInfoShadow>> GetUserInfoShadowAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<UserInfoShadow>(res);
            var resultingAccount = UserInfoShadow.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<UserInfoShadow>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<StorageConfig>> GetStorageConfigAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<StorageConfig>(res);
            var resultingAccount = StorageConfig.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<StorageConfig>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<File>> GetFileAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<File>(res);
            var resultingAccount = File.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<File>(res, resultingAccount);
        }

        public async Task<SubscriptionState> SubscribeUnstakeInfoAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, UnstakeInfo> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                UnstakeInfo parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = UnstakeInfo.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeStorageAccountAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, StorageAccount> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                StorageAccount parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = StorageAccount.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeStorageAccountV2Async(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, StorageAccountV2> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                StorageAccountV2 parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = StorageAccountV2.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeUserInfoShadowAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, UserInfoShadow> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                UserInfoShadow parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = UserInfoShadow.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeStorageConfigAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, StorageConfig> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                StorageConfig parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = StorageConfig.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeFileAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, File> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                File parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = File.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<RequestResult<string>> SendInitializeConfigAsync(InitializeConfigAccounts accounts, PublicKey uploader, PublicKey admin2, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.InitializeConfig(accounts, uploader, admin2, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendUpdateConfigAsync(UpdateConfigAccounts accounts, ulong? newStorageCost, BigInteger newStorageAvailable, PublicKey newAdmin2, ulong? newMaxAcctSize, ulong? newMinAcctSize, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.UpdateConfig(accounts, newStorageCost, newStorageAvailable, newAdmin2, newMaxAcctSize, newMinAcctSize, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendMutableFeesAsync(MutableFeesAccounts accounts, ulong? shadesPerGbPerEpoch, uint? crankBps, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.MutableFees(accounts, shadesPerGbPerEpoch, crankBps, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendInitializeAccountAsync(InitializeAccountAccounts accounts, string identifier, ulong storage, PublicKey owner2, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.InitializeAccount(accounts, identifier, storage, owner2, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendInitializeAccount2Async(InitializeAccount2Accounts accounts, string identifier, ulong storage, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.InitializeAccount2(accounts, identifier, storage, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendUpdateAccountAsync(UpdateAccountAccounts accounts, string identifier, PublicKey owner2, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.UpdateAccount(accounts, identifier, owner2, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendUpdateAccount2Async(UpdateAccount2Accounts accounts, string identifier, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.UpdateAccount2(accounts, identifier, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRequestDeleteAccountAsync(RequestDeleteAccountAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.RequestDeleteAccount(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRequestDeleteAccount2Async(RequestDeleteAccount2Accounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.RequestDeleteAccount2(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendUnmarkDeleteAccountAsync(UnmarkDeleteAccountAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.UnmarkDeleteAccount(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendUnmarkDeleteAccount2Async(UnmarkDeleteAccount2Accounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.UnmarkDeleteAccount2(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRedeemRentAsync(RedeemRentAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.RedeemRent(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendDeleteAccountAsync(DeleteAccountAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.DeleteAccount(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendDeleteAccount2Async(DeleteAccount2Accounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.DeleteAccount2(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendMakeAccountImmutableAsync(MakeAccountImmutableAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.MakeAccountImmutable(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendMakeAccountImmutable2Async(MakeAccountImmutable2Accounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.MakeAccountImmutable2(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendBadCsamAsync(BadCsamAccounts accounts, ulong storageAvailable, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.BadCsam(accounts, storageAvailable, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendBadCsam2Async(BadCsam2Accounts accounts, ulong storageAvailable, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.BadCsam2(accounts, storageAvailable, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendIncreaseStorageAsync(IncreaseStorageAccounts accounts, ulong additionalStorage, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.IncreaseStorage(accounts, additionalStorage, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendIncreaseStorage2Async(IncreaseStorage2Accounts accounts, ulong additionalStorage, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.IncreaseStorage2(accounts, additionalStorage, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendIncreaseImmutableStorageAsync(IncreaseImmutableStorageAccounts accounts, ulong additionalStorage, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.IncreaseImmutableStorage(accounts, additionalStorage, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendIncreaseImmutableStorage2Async(IncreaseImmutableStorage2Accounts accounts, ulong additionalStorage, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.IncreaseImmutableStorage2(accounts, additionalStorage, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendDecreaseStorageAsync(DecreaseStorageAccounts accounts, ulong removeStorage, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.DecreaseStorage(accounts, removeStorage, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendDecreaseStorage2Async(DecreaseStorage2Accounts accounts, ulong removeStorage, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.DecreaseStorage2(accounts, removeStorage, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendClaimStakeAsync(ClaimStakeAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.ClaimStake(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendClaimStake2Async(ClaimStake2Accounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.ClaimStake2(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendCrankAsync(CrankAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.Crank(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendCrank2Async(Crank2Accounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.Crank2(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRefreshStakeAsync(RefreshStakeAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.RefreshStake(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRefreshStake2Async(RefreshStake2Accounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.RefreshStake2(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendMigrateStep1Async(MigrateStep1Accounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.MigrateStep1(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendMigrateStep2Async(MigrateStep2Accounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.ShadowDriveUserStakingProgram.MigrateStep2(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        protected override Dictionary<uint, ProgramError<ShadowDriveUserStakingErrorKind>> BuildErrorsDictionary()
        {
            return new Dictionary<uint, ProgramError<ShadowDriveUserStakingErrorKind>>{{6000U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.NotEnoughStorage, "Not enough storage available on this Storage Account")}, {6001U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.FileNameLengthExceedsLimit, "The length of the file name exceeds the limit of 32 bytes")}, {6002U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.InvalidSha256Hash, "Invalid sha256 hash")}, {6003U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.HasHadBadCsam, "User at some point had a bad csam scan")}, {6004U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.StorageAccountMarkedImmutable, "Storage account is marked as immutable")}, {6005U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.ClaimingStakeTooSoon, "User has not waited enough time to claim stake")}, {6006U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.SolanaStorageAccountNotMutable, "The storage account needs to be marked as mutable to update last fee collection epoch")}, {6007U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.RemovingTooMuchStorage, "Attempting to decrease storage by more than is available")}, {6008U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.UnsignedIntegerCastFailed, "u128 -> u64 cast failed")}, {6009U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.NonzeroRemainingFileAccounts, "This storage account still has some file accounts associated with it that have not been deleted")}, {6010U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.AccountStillInGracePeriod, "This account is still within deletion grace period")}, {6011U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.AccountNotMarkedToBeDeleted, "This account is not marked to be deleted")}, {6012U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.FileStillInGracePeriod, "This file is still within deletion grace period")}, {6013U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.FileNotMarkedToBeDeleted, "This file is not marked to be deleted")}, {6014U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.FileMarkedImmutable, "File has been marked as immutable and cannot be edited")}, {6015U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.NoStorageIncrease, "User requested an increase of zero bytes")}, {6016U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.ExceededStorageLimit, "Requested a storage account with storage over the limit")}, {6017U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.InsufficientFunds, "User does not have enough funds to store requested number of bytes.")}, {6018U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.NotEnoughStorageOnShadowDrive, "There is not available storage on Shadow Drive. Good job!")}, {6019U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.AccountTooSmall, "Requested a storage account with storage under the limit")}, {6020U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.DidNotAgreeToToS, "User did not agree to terms of service")}, {6021U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.InvalidTokenTransferAmounts, "Invalid token transfers. Stake account nonempty.")}, {6022U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.FailedToCloseAccount, "Failed to close spl token account")}, {6023U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.FailedToTransferToEmissionsWallet, "Failed to transfer to emissions wallet")}, {6024U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.FailedToTransferToEmissionsWalletFromUser, "Failed to transfer to emissions wallet from user")}, {6025U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.FailedToReturnUserFunds, "Failed to return user funds")}, {6026U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.NeedSomeFees, "Turning on fees and passing in None for storage cost per epoch")}, {6027U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.NeedSomeCrankBps, "Turning on fees and passing in None for crank bps")}, {6028U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.AlreadyMarkedForDeletion, "This account is already marked to be deleted")}, {6029U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.EmptyStakeAccount, "User has an empty stake account and must refresh stake account before unmarking account for deletion")}, {6030U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.IdentifierExceededMaxLength, "New identifier exceeds maximum length of 64 bytes")}, {6031U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.OnlyAdmin1CanChangeAdmins, "Only admin1 can change admins")}, {6032U, new ProgramError<ShadowDriveUserStakingErrorKind>(ShadowDriveUserStakingErrorKind.OnlyOneOwnerAllowedInV15, "(As part of on-chain storage optimizations, only one owner is allowed in Shadow Drive v1.5)")}, };
        }
    }

    namespace Program
    {
        public class InitializeConfigAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey Admin1 { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class UpdateConfigAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey Admin { get; set; }
        }

        public class MutableFeesAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey Admin { get; set; }
        }

        public class InitializeAccountAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey UserInfoShadow { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey Owner1 { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey Owner1TokenAccount { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class InitializeAccount2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey UserInfoShadow { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey Owner1 { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey Owner1TokenAccount { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class UpdateAccountAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class UpdateAccount2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class RequestDeleteAccountAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class RequestDeleteAccount2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class UnmarkDeleteAccountAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class UnmarkDeleteAccount2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class RedeemRentAccounts
        {
            public PublicKey StorageAccount { get; set; }

            public PublicKey File { get; set; }

            public PublicKey Owner { get; set; }
        }

        public class DeleteAccountAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey UserInfoShadow { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey ShdwPayer { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class DeleteAccount2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey UserInfoShadow { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey ShdwPayer { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class MakeAccountImmutableAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class MakeAccountImmutable2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class BadCsamAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey UserInfoShadow { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class BadCsam2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey UserInfoShadow { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class IncreaseStorageAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class IncreaseStorage2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class IncreaseImmutableStorageAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class IncreaseImmutableStorage2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class DecreaseStorageAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey UnstakeInfo { get; set; }

            public PublicKey UnstakeAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class DecreaseStorage2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey UnstakeInfo { get; set; }

            public PublicKey UnstakeAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey Uploader { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class ClaimStakeAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey UnstakeInfo { get; set; }

            public PublicKey UnstakeAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class ClaimStake2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey UnstakeInfo { get; set; }

            public PublicKey UnstakeAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class CrankAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey Cranker { get; set; }

            public PublicKey CrankerAta { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class Crank2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey Cranker { get; set; }

            public PublicKey CrankerAta { get; set; }

            public PublicKey EmissionsWallet { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class RefreshStakeAccounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class RefreshStake2Accounts
        {
            public PublicKey StorageConfig { get; set; }

            public PublicKey StorageAccount { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey OwnerAta { get; set; }

            public PublicKey StakeAccount { get; set; }

            public PublicKey TokenMint { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class MigrateStep1Accounts
        {
            public PublicKey StorageAccount { get; set; }

            public PublicKey Migration { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class MigrateStep2Accounts
        {
            public PublicKey StorageAccount { get; set; }

            public PublicKey Migration { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public static class ShadowDriveUserStakingProgram
        {
            public static Solana.Unity.Rpc.Models.TransactionInstruction InitializeConfig(InitializeConfigAccounts accounts, PublicKey uploader, PublicKey admin2, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Admin1, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(5099410418541363152UL, offset);
                offset += 8;
                _data.WritePubKey(uploader, offset);
                offset += 32;
                if (admin2 != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WritePubKey(admin2, offset);
                    offset += 32;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction UpdateConfig(UpdateConfigAccounts accounts, ulong? newStorageCost, BigInteger newStorageAvailable, PublicKey newAdmin2, ulong? newMaxAcctSize, ulong? newMinAcctSize, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Admin, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(7195436135290281501UL, offset);
                offset += 8;
                if (newStorageCost != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WriteU64(newStorageCost.Value, offset);
                    offset += 8;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (newStorageAvailable != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WriteBigInt(newStorageAvailable, offset, 16, true);
                    offset += 16;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (newAdmin2 != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WritePubKey(newAdmin2, offset);
                    offset += 32;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (newMaxAcctSize != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WriteU64(newMaxAcctSize.Value, offset);
                    offset += 8;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (newMinAcctSize != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WriteU64(newMinAcctSize.Value, offset);
                    offset += 8;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction MutableFees(MutableFeesAccounts accounts, ulong? shadesPerGbPerEpoch, uint? crankBps, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Admin, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16712723855922571002UL, offset);
                offset += 8;
                if (shadesPerGbPerEpoch != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WriteU64(shadesPerGbPerEpoch.Value, offset);
                    offset += 8;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (crankBps != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WriteU32(crankBps.Value, offset);
                    offset += 4;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction InitializeAccount(InitializeAccountAccounts accounts, string identifier, ulong storage, PublicKey owner2, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UserInfoShadow, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner1, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner1TokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(533471794844365642UL, offset);
                offset += 8;
                offset += _data.WriteBorshString(identifier, offset);
                _data.WriteU64(storage, offset);
                offset += 8;
                if (owner2 != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WritePubKey(owner2, offset);
                    offset += 32;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction InitializeAccount2(InitializeAccount2Accounts accounts, string identifier, ulong storage, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UserInfoShadow, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner1, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner1TokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(7624910525970101768UL, offset);
                offset += 8;
                offset += _data.WriteBorshString(identifier, offset);
                _data.WriteU64(storage, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction UpdateAccount(UpdateAccountAccounts accounts, string identifier, PublicKey owner2, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(10990336994403950567UL, offset);
                offset += 8;
                if (identifier != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    offset += _data.WriteBorshString(identifier, offset);
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (owner2 != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WritePubKey(owner2, offset);
                    offset += 32;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction UpdateAccount2(UpdateAccount2Accounts accounts, string identifier, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(643376074133755486UL, offset);
                offset += 8;
                if (identifier != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    offset += _data.WriteBorshString(identifier, offset);
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction RequestDeleteAccount(RequestDeleteAccountAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(3379883273098101983UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction RequestDeleteAccount2(RequestDeleteAccount2Accounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(13344733152882101256UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction UnmarkDeleteAccount(UnmarkDeleteAccountAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(13246319461832072031UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction UnmarkDeleteAccount2(UnmarkDeleteAccount2Accounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(10120226077749033990UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction RedeemRent(RedeemRentAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.File, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(10437528202632931105UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction DeleteAccount(DeleteAccountAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UserInfoShadow, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ShdwPayer, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(18131218944165807740UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction DeleteAccount2(DeleteAccount2Accounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UserInfoShadow, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ShdwPayer, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(12428048064582981222UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction MakeAccountImmutable(MakeAccountImmutableAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16689531941082841189UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction MakeAccountImmutable2(MakeAccountImmutable2Accounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(10039830089828325699UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction BadCsam(BadCsamAccounts accounts, ulong storageAvailable, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UserInfoShadow, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16901289059335420451UL, offset);
                offset += 8;
                _data.WriteU64(storageAvailable, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction BadCsam2(BadCsam2Accounts accounts, ulong storageAvailable, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UserInfoShadow, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(14661184810958305607UL, offset);
                offset += 8;
                _data.WriteU64(storageAvailable, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction IncreaseStorage(IncreaseStorageAccounts accounts, ulong additionalStorage, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(6513646267456996UL, offset);
                offset += 8;
                _data.WriteU64(additionalStorage, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction IncreaseStorage2(IncreaseStorage2Accounts accounts, ulong additionalStorage, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(5669200893673823314UL, offset);
                offset += 8;
                _data.WriteU64(additionalStorage, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction IncreaseImmutableStorage(IncreaseImmutableStorageAccounts accounts, ulong additionalStorage, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(2665064272846582287UL, offset);
                offset += 8;
                _data.WriteU64(additionalStorage, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction IncreaseImmutableStorage2(IncreaseImmutableStorage2Accounts accounts, ulong additionalStorage, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(11225338989571294314UL, offset);
                offset += 8;
                _data.WriteU64(additionalStorage, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction DecreaseStorage(DecreaseStorageAccounts accounts, ulong removeStorage, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UnstakeInfo, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UnstakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(9956023283176508739UL, offset);
                offset += 8;
                _data.WriteU64(removeStorage, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction DecreaseStorage2(DecreaseStorage2Accounts accounts, ulong removeStorage, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UnstakeInfo, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UnstakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Uploader, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(12663776002556896536UL, offset);
                offset += 8;
                _data.WriteU64(removeStorage, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction ClaimStake(ClaimStakeAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UnstakeInfo, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UnstakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(10030989668264546622UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction ClaimStake2(ClaimStake2Accounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UnstakeInfo, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.UnstakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(4572198549117872859UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction Crank(CrankAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Cranker, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.CrankerAta, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(3848736535273007104UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction Crank2(Crank2Accounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Cranker, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.CrankerAta, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.EmissionsWallet, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(3937087695381268965UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction RefreshStake(RefreshStakeAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(8608609960058190786UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction RefreshStake2(RefreshStake2Accounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.StorageConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.OwnerAta, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StakeAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(4106208278219831736UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction MigrateStep1(MigrateStep1Accounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Migration, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(7159016320904643893UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction MigrateStep2(MigrateStep2Accounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.StorageAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Migration, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Owner, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(4447385444859359468UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }
        }
    }
}