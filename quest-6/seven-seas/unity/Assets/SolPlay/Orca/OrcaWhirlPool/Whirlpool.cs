using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Solana.Unity.Programs.Abstract;
using Solana.Unity.Programs.Utilities;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Core.Sockets;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using Whirlpool.Program;
using Whirlpool.Errors;
using Whirlpool.Accounts;
using Whirlpool.Types;

namespace Whirlpool
{
    namespace Accounts
    {
        public partial class WhirlpoolsConfig
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 18357050149419685021UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{157, 20, 49, 224, 217, 87, 193, 254};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "TGrzdk13ciR";
            public PublicKey FeeAuthority { get; set; }

            public PublicKey CollectProtocolFeesAuthority { get; set; }

            public PublicKey RewardEmissionsSuperAuthority { get; set; }

            public ushort DefaultProtocolFeeRate { get; set; }

            public static WhirlpoolsConfig Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                WhirlpoolsConfig result = new WhirlpoolsConfig();
                result.FeeAuthority = _data.GetPubKey(offset);
                offset += 32;
                result.CollectProtocolFeesAuthority = _data.GetPubKey(offset);
                offset += 32;
                result.RewardEmissionsSuperAuthority = _data.GetPubKey(offset);
                offset += 32;
                result.DefaultProtocolFeeRate = _data.GetU16(offset);
                offset += 2;
                return result;
            }
        }

        public partial class FeeTier
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 7619602997519010616UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{56, 75, 159, 76, 142, 68, 190, 105};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "AR8t9QRDQXa";
            public PublicKey WhirlpoolsConfig { get; set; }

            public ushort TickSpacing { get; set; }

            public ushort DefaultFeeRate { get; set; }

            public static FeeTier Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                FeeTier result = new FeeTier();
                result.WhirlpoolsConfig = _data.GetPubKey(offset);
                offset += 32;
                result.TickSpacing = _data.GetU16(offset);
                offset += 2;
                result.DefaultFeeRate = _data.GetU16(offset);
                offset += 2;
                return result;
            }
        }

        public partial class Position
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 15057574775701355690UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{170, 188, 143, 228, 122, 64, 247, 208};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "VZMoMoKgZQb";
            public PublicKey Whirlpool { get; set; }

            public PublicKey PositionMint { get; set; }

            public BigInteger Liquidity { get; set; }

            public int TickLowerIndex { get; set; }

            public int TickUpperIndex { get; set; }

            public BigInteger FeeGrowthCheckpointA { get; set; }

            public ulong FeeOwedA { get; set; }

            public BigInteger FeeGrowthCheckpointB { get; set; }

            public ulong FeeOwedB { get; set; }

            public PositionRewardInfo[] RewardInfos { get; set; }

            public static Position Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                Position result = new Position();
                result.Whirlpool = _data.GetPubKey(offset);
                offset += 32;
                result.PositionMint = _data.GetPubKey(offset);
                offset += 32;
                result.Liquidity = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.TickLowerIndex = _data.GetS32(offset);
                offset += 4;
                result.TickUpperIndex = _data.GetS32(offset);
                offset += 4;
                result.FeeGrowthCheckpointA = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.FeeOwedA = _data.GetU64(offset);
                offset += 8;
                result.FeeGrowthCheckpointB = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.FeeOwedB = _data.GetU64(offset);
                offset += 8;
                result.RewardInfos = new PositionRewardInfo[3];
                for (uint resultRewardInfosIdx = 0; resultRewardInfosIdx < 3; resultRewardInfosIdx++)
                {
                    offset += PositionRewardInfo.Deserialize(_data, offset, out var resultRewardInfosresultRewardInfosIdx);
                    result.RewardInfos[resultRewardInfosIdx] = resultRewardInfosresultRewardInfosIdx;
                }

                return result;
            }
        }

        public partial class TickArray
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 13493355605783306565UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{69, 97, 189, 190, 110, 7, 66, 187};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "Cc6F4MyvbgN";
            public int StartTickIndex { get; set; }

            public Tick[] Ticks { get; set; }

            public PublicKey Whirlpool { get; set; }

            public static TickArray Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                TickArray result = new TickArray();
                result.StartTickIndex = _data.GetS32(offset);
                offset += 4;
                result.Ticks = new Tick[88];
                for (uint resultTicksIdx = 0; resultTicksIdx < 88; resultTicksIdx++)
                {
                    offset += Tick.Deserialize(_data, offset, out var resultTicksresultTicksIdx);
                    result.Ticks[resultTicksIdx] = resultTicksresultTicksIdx;
                }

                result.Whirlpool = _data.GetPubKey(offset);
                offset += 32;
                return result;
            }
        }

        public partial class Whirlpool
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 676526073106765119UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{63, 149, 209, 12, 225, 128, 99, 9};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "BdrfaPg3xM6";
            public PublicKey WhirlpoolsConfig { get; set; }

            public byte[] WhirlpoolBump { get; set; }

            public ushort TickSpacing { get; set; }

            public byte[] TickSpacingSeed { get; set; }

            public ushort FeeRate { get; set; }

            public ushort ProtocolFeeRate { get; set; }

            public BigInteger Liquidity { get; set; }

            public BigInteger SqrtPrice { get; set; }

            public int TickCurrentIndex { get; set; }

            public ulong ProtocolFeeOwedA { get; set; }

            public ulong ProtocolFeeOwedB { get; set; }

            public PublicKey TokenMintA { get; set; }

            public PublicKey TokenVaultA { get; set; }

            public BigInteger FeeGrowthGlobalA { get; set; }

            public PublicKey TokenMintB { get; set; }

            public PublicKey TokenVaultB { get; set; }

            public BigInteger FeeGrowthGlobalB { get; set; }

            public ulong RewardLastUpdatedTimestamp { get; set; }

            public WhirlpoolRewardInfo[] RewardInfos { get; set; }

            public static Whirlpool Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                Whirlpool result = new Whirlpool();
                result.WhirlpoolsConfig = _data.GetPubKey(offset);
                offset += 32;
                result.WhirlpoolBump = _data.GetBytes(offset, 1);
                offset += 1;
                result.TickSpacing = _data.GetU16(offset);
                offset += 2;
                result.TickSpacingSeed = _data.GetBytes(offset, 2);
                offset += 2;
                result.FeeRate = _data.GetU16(offset);
                offset += 2;
                result.ProtocolFeeRate = _data.GetU16(offset);
                offset += 2;
                result.Liquidity = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.SqrtPrice = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.TickCurrentIndex = _data.GetS32(offset);
                offset += 4;
                result.ProtocolFeeOwedA = _data.GetU64(offset);
                offset += 8;
                result.ProtocolFeeOwedB = _data.GetU64(offset);
                offset += 8;
                result.TokenMintA = _data.GetPubKey(offset);
                offset += 32;
                result.TokenVaultA = _data.GetPubKey(offset);
                offset += 32;
                result.FeeGrowthGlobalA = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.TokenMintB = _data.GetPubKey(offset);
                offset += 32;
                result.TokenVaultB = _data.GetPubKey(offset);
                offset += 32;
                result.FeeGrowthGlobalB = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.RewardLastUpdatedTimestamp = _data.GetU64(offset);
                offset += 8;
                result.RewardInfos = new WhirlpoolRewardInfo[3];
                for (uint resultRewardInfosIdx = 0; resultRewardInfosIdx < 3; resultRewardInfosIdx++)
                {
                    offset += WhirlpoolRewardInfo.Deserialize(_data, offset, out var resultRewardInfosresultRewardInfosIdx);
                    result.RewardInfos[resultRewardInfosIdx] = resultRewardInfosresultRewardInfosIdx;
                }

                return result;
            }
        }
    }

    namespace Errors
    {
        public enum WhirlpoolErrorKind : uint
        {
            InvalidEnum = 6000U,
            InvalidStartTick = 6001U,
            TickArrayExistInPool = 6002U,
            TickArrayIndexOutofBounds = 6003U,
            InvalidTickSpacing = 6004U,
            ClosePositionNotEmpty = 6005U,
            DivideByZero = 6006U,
            NumberCastError = 6007U,
            NumberDownCastError = 6008U,
            TickNotFound = 6009U,
            InvalidTickIndex = 6010U,
            SqrtPriceOutOfBounds = 6011U,
            LiquidityZero = 6012U,
            LiquidityTooHigh = 6013U,
            LiquidityOverflow = 6014U,
            LiquidityUnderflow = 6015U,
            LiquidityNetError = 6016U,
            TokenMaxExceeded = 6017U,
            TokenMinSubceeded = 6018U,
            MissingOrInvalidDelegate = 6019U,
            InvalidPositionTokenAmount = 6020U,
            InvalidTimestampConversion = 6021U,
            InvalidTimestamp = 6022U,
            InvalidTickArraySequence = 6023U,
            InvalidTokenMintOrder = 6024U,
            RewardNotInitialized = 6025U,
            InvalidRewardIndex = 6026U,
            RewardVaultAmountInsufficient = 6027U,
            FeeRateMaxExceeded = 6028U,
            ProtocolFeeRateMaxExceeded = 6029U,
            MultiplicationShiftRightOverflow = 6030U,
            MulDivOverflow = 6031U,
            MulDivInvalidInput = 6032U,
            MultiplicationOverflow = 6033U,
            InvalidSqrtPriceLimitDirection = 6034U,
            ZeroTradableAmount = 6035U,
            AmountOutBelowMinimum = 6036U,
            AmountInAboveMaximum = 6037U,
            TickArraySequenceInvalidIndex = 6038U,
            AmountCalcOverflow = 6039U,
            AmountRemainingOverflow = 6040U
        }
    }

    namespace Types
    {
        public partial class OpenPositionBumps
        {
            public byte PositionBump { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU8(PositionBump, offset);
                offset += 1;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out OpenPositionBumps result)
            {
                int offset = initialOffset;
                result = new OpenPositionBumps();
                result.PositionBump = _data.GetU8(offset);
                offset += 1;
                return offset - initialOffset;
            }
        }

        public partial class OpenPositionWithMetadataBumps
        {
            public byte PositionBump { get; set; }

            public byte MetadataBump { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU8(PositionBump, offset);
                offset += 1;
                _data.WriteU8(MetadataBump, offset);
                offset += 1;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out OpenPositionWithMetadataBumps result)
            {
                int offset = initialOffset;
                result = new OpenPositionWithMetadataBumps();
                result.PositionBump = _data.GetU8(offset);
                offset += 1;
                result.MetadataBump = _data.GetU8(offset);
                offset += 1;
                return offset - initialOffset;
            }
        }

        public partial class PositionRewardInfo
        {
            public BigInteger GrowthInsideCheckpoint { get; set; }

            public ulong AmountOwed { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteBigInt(GrowthInsideCheckpoint, offset, 16, true);
                offset += 16;
                _data.WriteU64(AmountOwed, offset);
                offset += 8;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out PositionRewardInfo result)
            {
                int offset = initialOffset;
                result = new PositionRewardInfo();
                result.GrowthInsideCheckpoint = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.AmountOwed = _data.GetU64(offset);
                offset += 8;
                return offset - initialOffset;
            }
        }

        public partial class Tick
        {
            public bool Initialized { get; set; }

            public BigInteger LiquidityNet { get; set; }

            public BigInteger LiquidityGross { get; set; }

            public BigInteger FeeGrowthOutsideA { get; set; }

            public BigInteger FeeGrowthOutsideB { get; set; }

            public BigInteger[] RewardGrowthsOutside { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteBool(Initialized, offset);
                offset += 1;
                _data.WriteBigInt(LiquidityNet, offset, 16, false);
                offset += 16;
                _data.WriteBigInt(LiquidityGross, offset, 16, true);
                offset += 16;
                _data.WriteBigInt(FeeGrowthOutsideA, offset, 16, true);
                offset += 16;
                _data.WriteBigInt(FeeGrowthOutsideB, offset, 16, true);
                offset += 16;
                foreach (var rewardGrowthsOutsideElement in RewardGrowthsOutside)
                {
                    _data.WriteBigInt(rewardGrowthsOutsideElement, offset, 16, true);
                    offset += 16;
                }

                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out Tick result)
            {
                int offset = initialOffset;
                result = new Tick();
                result.Initialized = _data.GetBool(offset);
                offset += 1;
                result.LiquidityNet = _data.GetBigInt(offset, 16, true);
                offset += 16;
                result.LiquidityGross = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.FeeGrowthOutsideA = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.FeeGrowthOutsideB = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.RewardGrowthsOutside = new BigInteger[3];
                for (uint resultRewardGrowthsOutsideIdx = 0; resultRewardGrowthsOutsideIdx < 3; resultRewardGrowthsOutsideIdx++)
                {
                    result.RewardGrowthsOutside[resultRewardGrowthsOutsideIdx] = _data.GetBigInt(offset, 16, false);
                    offset += 16;
                }

                return offset - initialOffset;
            }
        }

        public partial class WhirlpoolRewardInfo
        {
            public PublicKey Mint { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey Authority { get; set; }

            public BigInteger EmissionsPerSecondX64 { get; set; }

            public BigInteger GrowthGlobalX64 { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WritePubKey(Mint, offset);
                offset += 32;
                _data.WritePubKey(Vault, offset);
                offset += 32;
                _data.WritePubKey(Authority, offset);
                offset += 32;
                _data.WriteBigInt(EmissionsPerSecondX64, offset, 16, true);
                offset += 16;
                _data.WriteBigInt(GrowthGlobalX64, offset, 16, true);
                offset += 16;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out WhirlpoolRewardInfo result)
            {
                int offset = initialOffset;
                result = new WhirlpoolRewardInfo();
                result.Mint = _data.GetPubKey(offset);
                offset += 32;
                result.Vault = _data.GetPubKey(offset);
                offset += 32;
                result.Authority = _data.GetPubKey(offset);
                offset += 32;
                result.EmissionsPerSecondX64 = _data.GetBigInt(offset, 16, false);
                offset += 16;
                result.GrowthGlobalX64 = _data.GetBigInt(offset, 16, false);
                offset += 16;
                return offset - initialOffset;
            }
        }

        public partial class WhirlpoolBumps
        {
            public byte WhirlpoolBump { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU8(WhirlpoolBump, offset);
                offset += 1;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out WhirlpoolBumps result)
            {
                int offset = initialOffset;
                result = new WhirlpoolBumps();
                result.WhirlpoolBump = _data.GetU8(offset);
                offset += 1;
                return offset - initialOffset;
            }
        }
    }

    public partial class WhirlpoolClient : TransactionalBaseClient<WhirlpoolErrorKind>
    {
        public WhirlpoolClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient, PublicKey programId) : base(rpcClient, streamingRpcClient, programId)
        {
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<WhirlpoolsConfig>>> GetWhirlpoolsConfigsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = WhirlpoolsConfig.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<WhirlpoolsConfig>>(res);
            List<WhirlpoolsConfig> resultingAccounts = new List<WhirlpoolsConfig>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => WhirlpoolsConfig.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<WhirlpoolsConfig>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<FeeTier>>> GetFeeTiersAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = FeeTier.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<FeeTier>>(res);
            List<FeeTier> resultingAccounts = new List<FeeTier>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => FeeTier.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<FeeTier>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Position>>> GetPositionsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = Position.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Position>>(res);
            List<Position> resultingAccounts = new List<Position>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Position.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Position>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<TickArray>>> GetTickArraysAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = TickArray.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<TickArray>>(res);
            List<TickArray> resultingAccounts = new List<TickArray>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => TickArray.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<TickArray>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Whirlpool.Accounts.Whirlpool>>> GetWhirlpoolsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = Whirlpool.Accounts.Whirlpool.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Whirlpool.Accounts.Whirlpool>>(res);
            List<Whirlpool.Accounts.Whirlpool> resultingAccounts = new List<Whirlpool.Accounts.Whirlpool>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Whirlpool.Accounts.Whirlpool.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Whirlpool.Accounts.Whirlpool>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<WhirlpoolsConfig>> GetWhirlpoolsConfigAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<WhirlpoolsConfig>(res);
            var resultingAccount = WhirlpoolsConfig.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<WhirlpoolsConfig>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<FeeTier>> GetFeeTierAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<FeeTier>(res);
            var resultingAccount = FeeTier.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<FeeTier>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<Position>> GetPositionAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<Position>(res);
            var resultingAccount = Position.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<Position>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<TickArray>> GetTickArrayAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<TickArray>(res);
            var resultingAccount = TickArray.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<TickArray>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<Whirlpool.Accounts.Whirlpool>> GetWhirlpoolAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<Whirlpool.Accounts.Whirlpool>(res);
            var resultingAccount = Whirlpool.Accounts.Whirlpool.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<Whirlpool.Accounts.Whirlpool>(res, resultingAccount);
        }

        public async Task<SubscriptionState> SubscribeWhirlpoolsConfigAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, WhirlpoolsConfig> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                WhirlpoolsConfig parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = WhirlpoolsConfig.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeFeeTierAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, FeeTier> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                FeeTier parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = FeeTier.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribePositionAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, Position> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                Position parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = Position.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeTickArrayAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, TickArray> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                TickArray parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = TickArray.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeWhirlpoolAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, Whirlpool.Accounts.Whirlpool> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                Whirlpool.Accounts.Whirlpool parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = Whirlpool.Accounts.Whirlpool.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<RequestResult<string>> SendInitializeConfigAsync(InitializeConfigAccounts accounts, PublicKey feeAuthority, PublicKey collectProtocolFeesAuthority, PublicKey rewardEmissionsSuperAuthority, ushort defaultProtocolFeeRate, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.InitializeConfig(accounts, feeAuthority, collectProtocolFeesAuthority, rewardEmissionsSuperAuthority, defaultProtocolFeeRate, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendInitializePoolAsync(InitializePoolAccounts accounts, WhirlpoolBumps bumps, ushort tickSpacing, BigInteger initialSqrtPrice, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.InitializePool(accounts, bumps, tickSpacing, initialSqrtPrice, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendInitializeTickArrayAsync(InitializeTickArrayAccounts accounts, int startTickIndex, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.InitializeTickArray(accounts, startTickIndex, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendInitializeFeeTierAsync(InitializeFeeTierAccounts accounts, ushort tickSpacing, ushort defaultFeeRate, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.InitializeFeeTier(accounts, tickSpacing, defaultFeeRate, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendInitializeRewardAsync(InitializeRewardAccounts accounts, byte rewardIndex, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.InitializeReward(accounts, rewardIndex, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetRewardEmissionsAsync(SetRewardEmissionsAccounts accounts, byte rewardIndex, BigInteger emissionsPerSecondX64, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.SetRewardEmissions(accounts, rewardIndex, emissionsPerSecondX64, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendOpenPositionAsync(OpenPositionAccounts accounts, OpenPositionBumps bumps, int tickLowerIndex, int tickUpperIndex, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.OpenPosition(accounts, bumps, tickLowerIndex, tickUpperIndex, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendOpenPositionWithMetadataAsync(OpenPositionWithMetadataAccounts accounts, OpenPositionWithMetadataBumps bumps, int tickLowerIndex, int tickUpperIndex, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.OpenPositionWithMetadata(accounts, bumps, tickLowerIndex, tickUpperIndex, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendIncreaseLiquidityAsync(IncreaseLiquidityAccounts accounts, BigInteger liquidityAmount, ulong tokenMaxA, ulong tokenMaxB, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.IncreaseLiquidity(accounts, liquidityAmount, tokenMaxA, tokenMaxB, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendDecreaseLiquidityAsync(DecreaseLiquidityAccounts accounts, BigInteger liquidityAmount, ulong tokenMinA, ulong tokenMinB, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.DecreaseLiquidity(accounts, liquidityAmount, tokenMinA, tokenMinB, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendUpdateFeesAndRewardsAsync(UpdateFeesAndRewardsAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.UpdateFeesAndRewards(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendCollectFeesAsync(CollectFeesAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.CollectFees(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendCollectRewardAsync(CollectRewardAccounts accounts, byte rewardIndex, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.CollectReward(accounts, rewardIndex, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendCollectProtocolFeesAsync(CollectProtocolFeesAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.CollectProtocolFees(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSwapAsync(SwapAccounts accounts, ulong amount, ulong otherAmountThreshold, BigInteger sqrtPriceLimit, bool amountSpecifiedIsInput, bool aToB, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.Swap(accounts, amount, otherAmountThreshold, sqrtPriceLimit, amountSpecifiedIsInput, aToB, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendClosePositionAsync(ClosePositionAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.ClosePosition(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetDefaultFeeRateAsync(SetDefaultFeeRateAccounts accounts, ushort defaultFeeRate, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.SetDefaultFeeRate(accounts, defaultFeeRate, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetDefaultProtocolFeeRateAsync(SetDefaultProtocolFeeRateAccounts accounts, ushort defaultProtocolFeeRate, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.SetDefaultProtocolFeeRate(accounts, defaultProtocolFeeRate, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetFeeRateAsync(SetFeeRateAccounts accounts, ushort feeRate, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.SetFeeRate(accounts, feeRate, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetProtocolFeeRateAsync(SetProtocolFeeRateAccounts accounts, ushort protocolFeeRate, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.SetProtocolFeeRate(accounts, protocolFeeRate, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetFeeAuthorityAsync(SetFeeAuthorityAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.SetFeeAuthority(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetCollectProtocolFeesAuthorityAsync(SetCollectProtocolFeesAuthorityAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.SetCollectProtocolFeesAuthority(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetRewardAuthorityAsync(SetRewardAuthorityAccounts accounts, byte rewardIndex, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.SetRewardAuthority(accounts, rewardIndex, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetRewardAuthorityBySuperAuthorityAsync(SetRewardAuthorityBySuperAuthorityAccounts accounts, byte rewardIndex, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.SetRewardAuthorityBySuperAuthority(accounts, rewardIndex, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetRewardEmissionsSuperAuthorityAsync(SetRewardEmissionsSuperAuthorityAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.WhirlpoolProgram.SetRewardEmissionsSuperAuthority(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        protected override Dictionary<uint, ProgramError<WhirlpoolErrorKind>> BuildErrorsDictionary()
        {
            return new Dictionary<uint, ProgramError<WhirlpoolErrorKind>>{{6000U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.InvalidEnum, "Enum value could not be converted")}, {6001U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.InvalidStartTick, "Invalid start tick index provided.")}, {6002U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.TickArrayExistInPool, "Tick-array already exists in this whirlpool")}, {6003U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.TickArrayIndexOutofBounds, "Attempt to search for a tick-array failed")}, {6004U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.InvalidTickSpacing, "Tick-spacing is not supported")}, {6005U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.ClosePositionNotEmpty, "Position is not empty It cannot be closed")}, {6006U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.DivideByZero, "Unable to divide by zero")}, {6007U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.NumberCastError, "Unable to cast number into BigInt")}, {6008U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.NumberDownCastError, "Unable to down cast number")}, {6009U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.TickNotFound, "Tick not found within tick array")}, {6010U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.InvalidTickIndex, "Provided tick index is either out of bounds or uninitializable")}, {6011U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.SqrtPriceOutOfBounds, "Provided sqrt price out of bounds")}, {6012U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.LiquidityZero, "Liquidity amount must be greater than zero")}, {6013U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.LiquidityTooHigh, "Liquidity amount must be less than i64::MAX")}, {6014U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.LiquidityOverflow, "Liquidity overflow")}, {6015U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.LiquidityUnderflow, "Liquidity underflow")}, {6016U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.LiquidityNetError, "Tick liquidity net underflowed or overflowed")}, {6017U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.TokenMaxExceeded, "Exceeded token max")}, {6018U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.TokenMinSubceeded, "Did not meet token min")}, {6019U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.MissingOrInvalidDelegate, "Position token account has a missing or invalid delegate")}, {6020U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.InvalidPositionTokenAmount, "Position token amount must be 1")}, {6021U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.InvalidTimestampConversion, "Timestamp should be convertible from i64 to u64")}, {6022U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.InvalidTimestamp, "Timestamp should be greater than the last updated timestamp")}, {6023U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.InvalidTickArraySequence, "Invalid tick array sequence provided for instruction.")}, {6024U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.InvalidTokenMintOrder, "Token Mint in wrong order")}, {6025U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.RewardNotInitialized, "Reward not initialized")}, {6026U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.InvalidRewardIndex, "Invalid reward index")}, {6027U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.RewardVaultAmountInsufficient, "Reward vault requires amount to support emissions for at least one day")}, {6028U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.FeeRateMaxExceeded, "Exceeded max fee rate")}, {6029U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.ProtocolFeeRateMaxExceeded, "Exceeded max protocol fee rate")}, {6030U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.MultiplicationShiftRightOverflow, "Multiplication with shift right overflow")}, {6031U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.MulDivOverflow, "Muldiv overflow")}, {6032U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.MulDivInvalidInput, "Invalid div_u256 input")}, {6033U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.MultiplicationOverflow, "Multiplication overflow")}, {6034U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.InvalidSqrtPriceLimitDirection, "Provided SqrtPriceLimit not in the same direction as the swap.")}, {6035U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.ZeroTradableAmount, "There are no tradable amount to swap.")}, {6036U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.AmountOutBelowMinimum, "Amount out below minimum threshold")}, {6037U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.AmountInAboveMaximum, "Amount in above maximum threshold")}, {6038U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.TickArraySequenceInvalidIndex, "Invalid index for tick array sequence")}, {6039U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.AmountCalcOverflow, "Amount calculated overflows")}, {6040U, new ProgramError<WhirlpoolErrorKind>(WhirlpoolErrorKind.AmountRemainingOverflow, "Amount remaining overflows")}, };
        }
    }

    namespace Program
    {
        public class InitializeConfigAccounts
        {
            public PublicKey Config { get; set; }

            public PublicKey Funder { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class InitializePoolAccounts
        {
            public PublicKey WhirlpoolsConfig { get; set; }

            public PublicKey TokenMintA { get; set; }

            public PublicKey TokenMintB { get; set; }

            public PublicKey Funder { get; set; }

            public PublicKey Whirlpool { get; set; }

            public PublicKey TokenVaultA { get; set; }

            public PublicKey TokenVaultB { get; set; }

            public PublicKey FeeTier { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class InitializeTickArrayAccounts
        {
            public PublicKey Whirlpool { get; set; }

            public PublicKey Funder { get; set; }

            public PublicKey TickArray { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class InitializeFeeTierAccounts
        {
            public PublicKey Config { get; set; }

            public PublicKey FeeTier { get; set; }

            public PublicKey Funder { get; set; }

            public PublicKey FeeAuthority { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class InitializeRewardAccounts
        {
            public PublicKey RewardAuthority { get; set; }

            public PublicKey Funder { get; set; }

            public PublicKey Whirlpool { get; set; }

            public PublicKey RewardMint { get; set; }

            public PublicKey RewardVault { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class SetRewardEmissionsAccounts
        {
            public PublicKey Whirlpool { get; set; }

            public PublicKey RewardAuthority { get; set; }

            public PublicKey RewardVault { get; set; }
        }

        public class OpenPositionAccounts
        {
            public PublicKey Funder { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey Position { get; set; }

            public PublicKey PositionMint { get; set; }

            public PublicKey PositionTokenAccount { get; set; }

            public PublicKey Whirlpool { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey Rent { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }
        }

        public class OpenPositionWithMetadataAccounts
        {
            public PublicKey Funder { get; set; }

            public PublicKey Owner { get; set; }

            public PublicKey Position { get; set; }

            public PublicKey PositionMint { get; set; }

            public PublicKey PositionMetadataAccount { get; set; }

            public PublicKey PositionTokenAccount { get; set; }

            public PublicKey Whirlpool { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey Rent { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }

            public PublicKey MetadataProgram { get; set; }

            public PublicKey MetadataUpdateAuth { get; set; }
        }

        public class IncreaseLiquidityAccounts
        {
            public PublicKey Whirlpool { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey PositionAuthority { get; set; }

            public PublicKey Position { get; set; }

            public PublicKey PositionTokenAccount { get; set; }

            public PublicKey TokenOwnerAccountA { get; set; }

            public PublicKey TokenOwnerAccountB { get; set; }

            public PublicKey TokenVaultA { get; set; }

            public PublicKey TokenVaultB { get; set; }

            public PublicKey TickArrayLower { get; set; }

            public PublicKey TickArrayUpper { get; set; }
        }

        public class DecreaseLiquidityAccounts
        {
            public PublicKey Whirlpool { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey PositionAuthority { get; set; }

            public PublicKey Position { get; set; }

            public PublicKey PositionTokenAccount { get; set; }

            public PublicKey TokenOwnerAccountA { get; set; }

            public PublicKey TokenOwnerAccountB { get; set; }

            public PublicKey TokenVaultA { get; set; }

            public PublicKey TokenVaultB { get; set; }

            public PublicKey TickArrayLower { get; set; }

            public PublicKey TickArrayUpper { get; set; }
        }

        public class UpdateFeesAndRewardsAccounts
        {
            public PublicKey Whirlpool { get; set; }

            public PublicKey Position { get; set; }

            public PublicKey TickArrayLower { get; set; }

            public PublicKey TickArrayUpper { get; set; }
        }

        public class CollectFeesAccounts
        {
            public PublicKey Whirlpool { get; set; }

            public PublicKey PositionAuthority { get; set; }

            public PublicKey Position { get; set; }

            public PublicKey PositionTokenAccount { get; set; }

            public PublicKey TokenOwnerAccountA { get; set; }

            public PublicKey TokenVaultA { get; set; }

            public PublicKey TokenOwnerAccountB { get; set; }

            public PublicKey TokenVaultB { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class CollectRewardAccounts
        {
            public PublicKey Whirlpool { get; set; }

            public PublicKey PositionAuthority { get; set; }

            public PublicKey Position { get; set; }

            public PublicKey PositionTokenAccount { get; set; }

            public PublicKey RewardOwnerAccount { get; set; }

            public PublicKey RewardVault { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class CollectProtocolFeesAccounts
        {
            public PublicKey WhirlpoolsConfig { get; set; }

            public PublicKey Whirlpool { get; set; }

            public PublicKey CollectProtocolFeesAuthority { get; set; }

            public PublicKey TokenVaultA { get; set; }

            public PublicKey TokenVaultB { get; set; }

            public PublicKey TokenDestinationA { get; set; }

            public PublicKey TokenDestinationB { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class SwapAccounts
        {
            public PublicKey TokenProgram { get; set; }

            public PublicKey TokenAuthority { get; set; }

            public PublicKey Whirlpool { get; set; }

            public PublicKey TokenOwnerAccountA { get; set; }

            public PublicKey TokenVaultA { get; set; }

            public PublicKey TokenOwnerAccountB { get; set; }

            public PublicKey TokenVaultB { get; set; }

            public PublicKey TickArray0 { get; set; }

            public PublicKey TickArray1 { get; set; }

            public PublicKey TickArray2 { get; set; }

            public PublicKey Oracle { get; set; }
        }

        public class ClosePositionAccounts
        {
            public PublicKey PositionAuthority { get; set; }

            public PublicKey Receiver { get; set; }

            public PublicKey Position { get; set; }

            public PublicKey PositionMint { get; set; }

            public PublicKey PositionTokenAccount { get; set; }

            public PublicKey TokenProgram { get; set; }
        }

        public class SetDefaultFeeRateAccounts
        {
            public PublicKey WhirlpoolsConfig { get; set; }

            public PublicKey FeeTier { get; set; }

            public PublicKey FeeAuthority { get; set; }
        }

        public class SetDefaultProtocolFeeRateAccounts
        {
            public PublicKey WhirlpoolsConfig { get; set; }

            public PublicKey FeeAuthority { get; set; }
        }

        public class SetFeeRateAccounts
        {
            public PublicKey WhirlpoolsConfig { get; set; }

            public PublicKey Whirlpool { get; set; }

            public PublicKey FeeAuthority { get; set; }
        }

        public class SetProtocolFeeRateAccounts
        {
            public PublicKey WhirlpoolsConfig { get; set; }

            public PublicKey Whirlpool { get; set; }

            public PublicKey FeeAuthority { get; set; }
        }

        public class SetFeeAuthorityAccounts
        {
            public PublicKey WhirlpoolsConfig { get; set; }

            public PublicKey FeeAuthority { get; set; }

            public PublicKey NewFeeAuthority { get; set; }
        }

        public class SetCollectProtocolFeesAuthorityAccounts
        {
            public PublicKey WhirlpoolsConfig { get; set; }

            public PublicKey CollectProtocolFeesAuthority { get; set; }

            public PublicKey NewCollectProtocolFeesAuthority { get; set; }
        }

        public class SetRewardAuthorityAccounts
        {
            public PublicKey Whirlpool { get; set; }

            public PublicKey RewardAuthority { get; set; }

            public PublicKey NewRewardAuthority { get; set; }
        }

        public class SetRewardAuthorityBySuperAuthorityAccounts
        {
            public PublicKey WhirlpoolsConfig { get; set; }

            public PublicKey Whirlpool { get; set; }

            public PublicKey RewardEmissionsSuperAuthority { get; set; }

            public PublicKey NewRewardAuthority { get; set; }
        }

        public class SetRewardEmissionsSuperAuthorityAccounts
        {
            public PublicKey WhirlpoolsConfig { get; set; }

            public PublicKey RewardEmissionsSuperAuthority { get; set; }

            public PublicKey NewRewardEmissionsSuperAuthority { get; set; }
        }

        public static class WhirlpoolProgram
        {
            public static Solana.Unity.Rpc.Models.TransactionInstruction InitializeConfig(InitializeConfigAccounts accounts, PublicKey feeAuthority, PublicKey collectProtocolFeesAuthority, PublicKey rewardEmissionsSuperAuthority, ushort defaultProtocolFeeRate, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Config, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Funder, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(5099410418541363152UL, offset);
                offset += 8;
                _data.WritePubKey(feeAuthority, offset);
                offset += 32;
                _data.WritePubKey(collectProtocolFeesAuthority, offset);
                offset += 32;
                _data.WritePubKey(rewardEmissionsSuperAuthority, offset);
                offset += 32;
                _data.WriteU16(defaultProtocolFeeRate, offset);
                offset += 2;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction InitializePool(InitializePoolAccounts accounts, WhirlpoolBumps bumps, ushort tickSpacing, BigInteger initialSqrtPrice, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.WhirlpoolsConfig, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMintA, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenMintB, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Funder, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultA, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultB, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.FeeTier, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(2947797634800858207UL, offset);
                offset += 8;
                offset += bumps.Serialize(_data, offset);
                _data.WriteU16(tickSpacing, offset);
                offset += 2;
                _data.WriteBigInt(initialSqrtPrice, offset, 16, true);
                offset += 16;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction InitializeTickArray(InitializeTickArrayAccounts accounts, int startTickIndex, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Funder, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TickArray, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(13300637739260165131UL, offset);
                offset += 8;
                _data.WriteS32(startTickIndex, offset);
                offset += 4;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction InitializeFeeTier(InitializeFeeTierAccounts accounts, ushort tickSpacing, ushort defaultFeeRate, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Config, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.FeeTier, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Funder, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.FeeAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(2173552452913875639UL, offset);
                offset += 8;
                _data.WriteU16(tickSpacing, offset);
                offset += 2;
                _data.WriteU16(defaultFeeRate, offset);
                offset += 2;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction InitializeReward(InitializeRewardAccounts accounts, byte rewardIndex, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RewardAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Funder, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RewardMint, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.RewardVault, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(4964798518905571167UL, offset);
                offset += 8;
                _data.WriteU8(rewardIndex, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SetRewardEmissions(SetRewardEmissionsAccounts accounts, byte rewardIndex, BigInteger emissionsPerSecondX64, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RewardAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RewardVault, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(17589846754647786765UL, offset);
                offset += 8;
                _data.WriteU8(rewardIndex, offset);
                offset += 1;
                _data.WriteBigInt(emissionsPerSecondX64, offset, 16, true);
                offset += 16;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction OpenPosition(OpenPositionAccounts accounts, OpenPositionBumps bumps, int tickLowerIndex, int tickUpperIndex, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Funder, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Owner, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Position, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PositionMint, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PositionTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Rent, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(3598543293755916423UL, offset);
                offset += 8;
                offset += bumps.Serialize(_data, offset);
                _data.WriteS32(tickLowerIndex, offset);
                offset += 4;
                _data.WriteS32(tickUpperIndex, offset);
                offset += 4;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction OpenPositionWithMetadata(OpenPositionWithMetadataAccounts accounts, OpenPositionWithMetadataBumps bumps, int tickLowerIndex, int tickUpperIndex, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Funder, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Owner, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Position, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PositionMint, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PositionMetadataAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PositionTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Rent, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.MetadataProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.MetadataUpdateAuth, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(4327517488150879730UL, offset);
                offset += 8;
                offset += bumps.Serialize(_data, offset);
                _data.WriteS32(tickLowerIndex, offset);
                offset += 4;
                _data.WriteS32(tickUpperIndex, offset);
                offset += 4;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction IncreaseLiquidity(IncreaseLiquidityAccounts accounts, BigInteger liquidityAmount, ulong tokenMaxA, ulong tokenMaxB, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.PositionAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Position, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.PositionTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenOwnerAccountA, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenOwnerAccountB, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultA, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultB, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TickArrayLower, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TickArrayUpper, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(12897127415619492910UL, offset);
                offset += 8;
                _data.WriteBigInt(liquidityAmount, offset, 16, true);
                offset += 16;
                _data.WriteU64(tokenMaxA, offset);
                offset += 8;
                _data.WriteU64(tokenMaxB, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction DecreaseLiquidity(DecreaseLiquidityAccounts accounts, BigInteger liquidityAmount, ulong tokenMinA, ulong tokenMinB, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.PositionAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Position, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.PositionTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenOwnerAccountA, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenOwnerAccountB, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultA, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultB, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TickArrayLower, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TickArrayUpper, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(84542997123835552UL, offset);
                offset += 8;
                _data.WriteBigInt(liquidityAmount, offset, 16, true);
                offset += 16;
                _data.WriteU64(tokenMinA, offset);
                offset += 8;
                _data.WriteU64(tokenMinB, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction UpdateFeesAndRewards(UpdateFeesAndRewardsAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Position, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TickArrayLower, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TickArrayUpper, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16090184905488262810UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction CollectFees(CollectFeesAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.PositionAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Position, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.PositionTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenOwnerAccountA, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultA, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenOwnerAccountB, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultB, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(13120034779146721444UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction CollectReward(CollectRewardAccounts accounts, byte rewardIndex, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.PositionAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Position, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.PositionTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.RewardOwnerAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.RewardVault, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(2500038024235320646UL, offset);
                offset += 8;
                _data.WriteU8(rewardIndex, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction CollectProtocolFees(CollectProtocolFeesAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.WhirlpoolsConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.CollectProtocolFeesAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultA, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultB, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenDestinationA, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenDestinationB, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(15872570295674422038UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction Swap(SwapAccounts accounts, ulong amount, ulong otherAmountThreshold, BigInteger sqrtPriceLimit, bool amountSpecifiedIsInput, bool aToB, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenOwnerAccountA, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultA, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenOwnerAccountB, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TokenVaultB, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TickArray0, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TickArray1, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.TickArray2, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Oracle, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(14449647541112719096UL, offset);
                offset += 8;
                _data.WriteU64(amount, offset);
                offset += 8;
                _data.WriteU64(otherAmountThreshold, offset);
                offset += 8;
                _data.WriteBigInt(sqrtPriceLimit, offset, 16, true);
                offset += 16;
                _data.WriteBool(amountSpecifiedIsInput, offset);
                offset += 1;
                _data.WriteBool(aToB, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction ClosePosition(ClosePositionAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.PositionAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Receiver, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Position, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PositionMint, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PositionTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(7089303740684011131UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SetDefaultFeeRate(SetDefaultFeeRateAccounts accounts, ushort defaultFeeRate, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.WhirlpoolsConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.FeeTier, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.FeeAuthority, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16487930808298297206UL, offset);
                offset += 8;
                _data.WriteU16(defaultFeeRate, offset);
                offset += 2;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SetDefaultProtocolFeeRate(SetDefaultProtocolFeeRateAccounts accounts, ushort defaultProtocolFeeRate, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.WhirlpoolsConfig, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.FeeAuthority, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(24245983252172139UL, offset);
                offset += 8;
                _data.WriteU16(defaultProtocolFeeRate, offset);
                offset += 2;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SetFeeRate(SetFeeRateAccounts accounts, ushort feeRate, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.WhirlpoolsConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.FeeAuthority, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(476972577635038005UL, offset);
                offset += 8;
                _data.WriteU16(feeRate, offset);
                offset += 2;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SetProtocolFeeRate(SetProtocolFeeRateAccounts accounts, ushort protocolFeeRate, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.WhirlpoolsConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.FeeAuthority, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(9483542439018104671UL, offset);
                offset += 8;
                _data.WriteU16(protocolFeeRate, offset);
                offset += 2;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SetFeeAuthority(SetFeeAuthorityAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.WhirlpoolsConfig, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.FeeAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.NewFeeAuthority, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(9539017555791970591UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SetCollectProtocolFeesAuthority(SetCollectProtocolFeesAuthorityAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.WhirlpoolsConfig, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.CollectProtocolFeesAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.NewCollectProtocolFeesAuthority, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(4893690461331232290UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SetRewardAuthority(SetRewardAuthorityAccounts accounts, byte rewardIndex, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RewardAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.NewRewardAuthority, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(9175270962884978466UL, offset);
                offset += 8;
                _data.WriteU8(rewardIndex, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SetRewardAuthorityBySuperAuthority(SetRewardAuthorityBySuperAuthorityAccounts accounts, byte rewardIndex, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.WhirlpoolsConfig, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Whirlpool, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RewardEmissionsSuperAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.NewRewardAuthority, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(1817305343215639280UL, offset);
                offset += 8;
                _data.WriteU8(rewardIndex, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SetRewardEmissionsSuperAuthority(SetRewardEmissionsSuperAuthorityAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.WhirlpoolsConfig, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RewardEmissionsSuperAuthority, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.NewRewardEmissionsSuperAuthority, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(13209682757187798479UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }
        }
    }
}