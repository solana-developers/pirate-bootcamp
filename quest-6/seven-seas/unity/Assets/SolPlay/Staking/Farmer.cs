using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
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

namespace GemFarm
{
    namespace Accounts
    {
        public partial class AuthorizationProof
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 18391403730389340973UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{45, 147, 166, 62, 64, 100, 59, 255};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "8d9o3kZv4qC";
            public PublicKey AuthorizedFunder { get; set; }

            public PublicKey Farm { get; set; }

            public byte[] Reserved { get; set; }

            public static AuthorizationProof Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                AuthorizationProof result = new AuthorizationProof();
                result.AuthorizedFunder = _data.GetPubKey(offset);
                offset += 32;
                result.Farm = _data.GetPubKey(offset);
                offset += 32;
                result.Reserved = _data.GetBytes(offset, 32);
                offset += 32;
                return result;
            }
        }

        public partial class Farmer
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 15788536735091212286UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{254, 63, 81, 98, 130, 38, 28, 219};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "jXX2LSteZVk";
            public PublicKey Farm { get; set; }

            public PublicKey Identity { get; set; }

            public PublicKey Vault { get; set; }

            public FarmerState State { get; set; }

            public ulong GemsStaked { get; set; }

            public ulong RarityPointsStaked { get; set; }

            public ulong MinStakingEndsTs { get; set; }

            public ulong CooldownEndsTs { get; set; }

            public FarmerReward RewardA { get; set; }

            public FarmerReward RewardB { get; set; }

            public byte[] Reserved { get; set; }

            public static Farmer Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                Farmer result = new Farmer();
                result.Farm = _data.GetPubKey(offset);
                offset += 32;
                result.Identity = _data.GetPubKey(offset);
                offset += 32;
                result.Vault = _data.GetPubKey(offset);
                offset += 32;
                result.State = (FarmerState)_data.GetU8(offset);
                offset += 1;
                result.GemsStaked = _data.GetU64(offset);
                offset += 8;
                result.RarityPointsStaked = _data.GetU64(offset);
                offset += 8;
                result.MinStakingEndsTs = _data.GetU64(offset);
                offset += 8;
                result.CooldownEndsTs = _data.GetU64(offset);
                offset += 8;
                offset += FarmerReward.Deserialize(_data, offset, out var resultRewardA);
                result.RewardA = resultRewardA;
                offset += FarmerReward.Deserialize(_data, offset, out var resultRewardB);
                result.RewardB = resultRewardB;
                result.Reserved = _data.GetBytes(offset, 32);
                offset += 32;
                return result;
            }
        }

        public partial class Farm
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 18029388129992154273UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{161, 156, 211, 253, 250, 64, 53, 250};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "U2qzmZH87sw";
            public ushort Version { get; set; }

            public PublicKey FarmManager { get; set; }

            public PublicKey FarmTreasury { get; set; }

            public PublicKey FarmAuthority { get; set; }

            public PublicKey FarmAuthoritySeed { get; set; }

            public byte[] FarmAuthorityBumpSeed { get; set; }

            public PublicKey Bank { get; set; }

            public FarmConfig Config { get; set; }

            public ulong FarmerCount { get; set; }

            public ulong StakedFarmerCount { get; set; }

            public ulong GemsStaked { get; set; }

            public ulong RarityPointsStaked { get; set; }

            public ulong AuthorizedFunderCount { get; set; }

            public FarmReward RewardA { get; set; }

            public FarmReward RewardB { get; set; }

            public MaxCounts MaxCounts { get; set; }

            public byte[] Reserved { get; set; }

            public byte[] Reserved2 { get; set; }

            public byte[] Reserved3 { get; set; }

            public static Farm Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                Farm result = new Farm();
                result.Version = _data.GetU16(offset);
                offset += 2;
                result.FarmManager = _data.GetPubKey(offset);
                offset += 32;
                result.FarmTreasury = _data.GetPubKey(offset);
                offset += 32;
                result.FarmAuthority = _data.GetPubKey(offset);
                offset += 32;
                result.FarmAuthoritySeed = _data.GetPubKey(offset);
                offset += 32;
                result.FarmAuthorityBumpSeed = _data.GetBytes(offset, 1);
                offset += 1;
                result.Bank = _data.GetPubKey(offset);
                offset += 32;
                offset += FarmConfig.Deserialize(_data, offset, out var resultConfig);
                result.Config = resultConfig;
                result.FarmerCount = _data.GetU64(offset);
                offset += 8;
                result.StakedFarmerCount = _data.GetU64(offset);
                offset += 8;
                result.GemsStaked = _data.GetU64(offset);
                offset += 8;
                result.RarityPointsStaked = _data.GetU64(offset);
                offset += 8;
                result.AuthorizedFunderCount = _data.GetU64(offset);
                offset += 8;
                offset += FarmReward.Deserialize(_data, offset, out var resultRewardA);
                result.RewardA = resultRewardA;
                offset += FarmReward.Deserialize(_data, offset, out var resultRewardB);
                result.RewardB = resultRewardB;
                offset += MaxCounts.Deserialize(_data, offset, out var resultMaxCounts);
                result.MaxCounts = resultMaxCounts;
                result.Reserved = _data.GetBytes(offset, 32);
                offset += 32;
                result.Reserved2 = _data.GetBytes(offset, 16);
                offset += 16;
                result.Reserved3 = _data.GetBytes(offset, 4);
                offset += 4;
                return result;
            }
        }
    }

    namespace Errors
    {
        public enum GemFarmErrorKind : uint
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

        public partial class Number128
        {
            public BigInteger N { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteBigInt(N, offset, 16, true);
                offset += 16;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out Number128 result)
            {
                int offset = initialOffset;
                result = new Number128();
                result.N = _data.GetBigInt(offset, 16, false);
                offset += 16;
                return offset - initialOffset;
            }
        }

        public partial class FarmerReward
        {
            public ulong PaidOutReward { get; set; }

            public ulong AccruedReward { get; set; }

            public FarmerVariableRateReward VariableRate { get; set; }

            public FarmerFixedRateReward FixedRate { get; set; }

            public byte[] Reserved { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU64(PaidOutReward, offset);
                offset += 8;
                _data.WriteU64(AccruedReward, offset);
                offset += 8;
                offset += VariableRate.Serialize(_data, offset);
                offset += FixedRate.Serialize(_data, offset);
                _data.WriteSpan(Reserved, offset);
                offset += Reserved.Length;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out FarmerReward result)
            {
                int offset = initialOffset;
                result = new FarmerReward();
                result.PaidOutReward = _data.GetU64(offset);
                offset += 8;
                result.AccruedReward = _data.GetU64(offset);
                offset += 8;
                offset += FarmerVariableRateReward.Deserialize(_data, offset, out var resultVariableRate);
                result.VariableRate = resultVariableRate;
                offset += FarmerFixedRateReward.Deserialize(_data, offset, out var resultFixedRate);
                result.FixedRate = resultFixedRate;
                result.Reserved = _data.GetBytes(offset, 32);
                offset += 32;
                return offset - initialOffset;
            }
        }

        public partial class FarmerVariableRateReward
        {
            public Number128 LastRecordedAccruedRewardPerRarityPoint { get; set; }

            public byte[] Reserved { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                offset += LastRecordedAccruedRewardPerRarityPoint.Serialize(_data, offset);
                _data.WriteSpan(Reserved, offset);
                offset += Reserved.Length;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out FarmerVariableRateReward result)
            {
                int offset = initialOffset;
                result = new FarmerVariableRateReward();
                offset += Number128.Deserialize(_data, offset, out var resultLastRecordedAccruedRewardPerRarityPoint);
                result.LastRecordedAccruedRewardPerRarityPoint = resultLastRecordedAccruedRewardPerRarityPoint;
                result.Reserved = _data.GetBytes(offset, 16);
                offset += 16;
                return offset - initialOffset;
            }
        }

        public partial class FarmerFixedRateReward
        {
            public ulong BeginStakingTs { get; set; }

            public ulong BeginScheduleTs { get; set; }

            public ulong LastUpdatedTs { get; set; }

            public FixedRateSchedule PromisedSchedule { get; set; }

            public ulong PromisedDuration { get; set; }

            public byte[] Reserved { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU64(BeginStakingTs, offset);
                offset += 8;
                _data.WriteU64(BeginScheduleTs, offset);
                offset += 8;
                _data.WriteU64(LastUpdatedTs, offset);
                offset += 8;
                offset += PromisedSchedule.Serialize(_data, offset);
                _data.WriteU64(PromisedDuration, offset);
                offset += 8;
                _data.WriteSpan(Reserved, offset);
                offset += Reserved.Length;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out FarmerFixedRateReward result)
            {
                int offset = initialOffset;
                result = new FarmerFixedRateReward();
                result.BeginStakingTs = _data.GetU64(offset);
                offset += 8;
                result.BeginScheduleTs = _data.GetU64(offset);
                offset += 8;
                result.LastUpdatedTs = _data.GetU64(offset);
                offset += 8;
                offset += FixedRateSchedule.Deserialize(_data, offset, out var resultPromisedSchedule);
                result.PromisedSchedule = resultPromisedSchedule;
                result.PromisedDuration = _data.GetU64(offset);
                offset += 8;
                result.Reserved = _data.GetBytes(offset, 16);
                offset += 16;
                return offset - initialOffset;
            }
        }

        public partial class FarmConfig
        {
            public ulong MinStakingPeriodSec { get; set; }

            public ulong CooldownPeriodSec { get; set; }

            public ulong UnstakingFeeLamp { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU64(MinStakingPeriodSec, offset);
                offset += 8;
                _data.WriteU64(CooldownPeriodSec, offset);
                offset += 8;
                _data.WriteU64(UnstakingFeeLamp, offset);
                offset += 8;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out FarmConfig result)
            {
                int offset = initialOffset;
                result = new FarmConfig();
                result.MinStakingPeriodSec = _data.GetU64(offset);
                offset += 8;
                result.CooldownPeriodSec = _data.GetU64(offset);
                offset += 8;
                result.UnstakingFeeLamp = _data.GetU64(offset);
                offset += 8;
                return offset - initialOffset;
            }
        }

        public partial class MaxCounts
        {
            public uint MaxFarmers { get; set; }

            public uint MaxGems { get; set; }

            public uint MaxRarityPoints { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU32(MaxFarmers, offset);
                offset += 4;
                _data.WriteU32(MaxGems, offset);
                offset += 4;
                _data.WriteU32(MaxRarityPoints, offset);
                offset += 4;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out MaxCounts result)
            {
                int offset = initialOffset;
                result = new MaxCounts();
                result.MaxFarmers = _data.GetU32(offset);
                offset += 4;
                result.MaxGems = _data.GetU32(offset);
                offset += 4;
                result.MaxRarityPoints = _data.GetU32(offset);
                offset += 4;
                return offset - initialOffset;
            }
        }

        public partial class FundsTracker
        {
            public ulong TotalFunded { get; set; }

            public ulong TotalRefunded { get; set; }

            public ulong TotalAccruedToStakers { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU64(TotalFunded, offset);
                offset += 8;
                _data.WriteU64(TotalRefunded, offset);
                offset += 8;
                _data.WriteU64(TotalAccruedToStakers, offset);
                offset += 8;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out FundsTracker result)
            {
                int offset = initialOffset;
                result = new FundsTracker();
                result.TotalFunded = _data.GetU64(offset);
                offset += 8;
                result.TotalRefunded = _data.GetU64(offset);
                offset += 8;
                result.TotalAccruedToStakers = _data.GetU64(offset);
                offset += 8;
                return offset - initialOffset;
            }
        }

        public partial class TimeTracker
        {
            public ulong DurationSec { get; set; }

            public ulong RewardEndTs { get; set; }

            public ulong LockEndTs { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU64(DurationSec, offset);
                offset += 8;
                _data.WriteU64(RewardEndTs, offset);
                offset += 8;
                _data.WriteU64(LockEndTs, offset);
                offset += 8;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out TimeTracker result)
            {
                int offset = initialOffset;
                result = new TimeTracker();
                result.DurationSec = _data.GetU64(offset);
                offset += 8;
                result.RewardEndTs = _data.GetU64(offset);
                offset += 8;
                result.LockEndTs = _data.GetU64(offset);
                offset += 8;
                return offset - initialOffset;
            }
        }

        public partial class FarmReward
        {
            public PublicKey RewardMint { get; set; }

            public PublicKey RewardPot { get; set; }

            public RewardType RewardType { get; set; }

            public FixedRateReward FixedRate { get; set; }

            public VariableRateReward VariableRate { get; set; }

            public FundsTracker Funds { get; set; }

            public TimeTracker Times { get; set; }

            public byte[] Reserved { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WritePubKey(RewardMint, offset);
                offset += 32;
                _data.WritePubKey(RewardPot, offset);
                offset += 32;
                _data.WriteU8((byte)RewardType, offset);
                offset += 1;
                offset += FixedRate.Serialize(_data, offset);
                offset += VariableRate.Serialize(_data, offset);
                offset += Funds.Serialize(_data, offset);
                offset += Times.Serialize(_data, offset);
                _data.WriteSpan(Reserved, offset);
                offset += Reserved.Length;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out FarmReward result)
            {
                int offset = initialOffset;
                result = new FarmReward();
                result.RewardMint = _data.GetPubKey(offset);
                offset += 32;
                result.RewardPot = _data.GetPubKey(offset);
                offset += 32;
                result.RewardType = (RewardType)_data.GetU8(offset);
                offset += 1;
                offset += FixedRateReward.Deserialize(_data, offset, out var resultFixedRate);
                result.FixedRate = resultFixedRate;
                offset += VariableRateReward.Deserialize(_data, offset, out var resultVariableRate);
                result.VariableRate = resultVariableRate;
                offset += FundsTracker.Deserialize(_data, offset, out var resultFunds);
                result.Funds = resultFunds;
                offset += TimeTracker.Deserialize(_data, offset, out var resultTimes);
                result.Times = resultTimes;
                result.Reserved = _data.GetBytes(offset, 32);
                offset += 32;
                return offset - initialOffset;
            }
        }

        public partial class TierConfig
        {
            public ulong RewardRate { get; set; }

            public ulong RequiredTenure { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU64(RewardRate, offset);
                offset += 8;
                _data.WriteU64(RequiredTenure, offset);
                offset += 8;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out TierConfig result)
            {
                int offset = initialOffset;
                result = new TierConfig();
                result.RewardRate = _data.GetU64(offset);
                offset += 8;
                result.RequiredTenure = _data.GetU64(offset);
                offset += 8;
                return offset - initialOffset;
            }
        }

        public partial class FixedRateSchedule
        {
            public ulong BaseRate { get; set; }

            public TierConfig Tier1 { get; set; }

            public TierConfig Tier2 { get; set; }

            public TierConfig Tier3 { get; set; }

            public ulong Denominator { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU64(BaseRate, offset);
                offset += 8;
                if (Tier1 != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    offset += Tier1.Serialize(_data, offset);
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (Tier2 != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    offset += Tier2.Serialize(_data, offset);
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (Tier3 != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    offset += Tier3.Serialize(_data, offset);
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                _data.WriteU64(Denominator, offset);
                offset += 8;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out FixedRateSchedule result)
            {
                int offset = initialOffset;
                result = new FixedRateSchedule();
                result.BaseRate = _data.GetU64(offset);
                offset += 8;
                if (_data.GetBool(offset++))
                {
                    offset += TierConfig.Deserialize(_data, offset, out var resultTier1);
                    result.Tier1 = resultTier1;
                }

                if (_data.GetBool(offset++))
                {
                    offset += TierConfig.Deserialize(_data, offset, out var resultTier2);
                    result.Tier2 = resultTier2;
                }

                if (_data.GetBool(offset++))
                {
                    offset += TierConfig.Deserialize(_data, offset, out var resultTier3);
                    result.Tier3 = resultTier3;
                }

                result.Denominator = _data.GetU64(offset);
                offset += 8;
                return offset - initialOffset;
            }
        }

        public partial class FixedRateConfig
        {
            public FixedRateSchedule Schedule { get; set; }

            public ulong Amount { get; set; }

            public ulong DurationSec { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                offset += Schedule.Serialize(_data, offset);
                _data.WriteU64(Amount, offset);
                offset += 8;
                _data.WriteU64(DurationSec, offset);
                offset += 8;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out FixedRateConfig result)
            {
                int offset = initialOffset;
                result = new FixedRateConfig();
                offset += FixedRateSchedule.Deserialize(_data, offset, out var resultSchedule);
                result.Schedule = resultSchedule;
                result.Amount = _data.GetU64(offset);
                offset += 8;
                result.DurationSec = _data.GetU64(offset);
                offset += 8;
                return offset - initialOffset;
            }
        }

        public partial class FixedRateReward
        {
            public FixedRateSchedule Schedule { get; set; }

            public ulong ReservedAmount { get; set; }

            public byte[] Reserved { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                offset += Schedule.Serialize(_data, offset);
                _data.WriteU64(ReservedAmount, offset);
                offset += 8;
                _data.WriteSpan(Reserved, offset);
                offset += Reserved.Length;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out FixedRateReward result)
            {
                int offset = initialOffset;
                result = new FixedRateReward();
                offset += FixedRateSchedule.Deserialize(_data, offset, out var resultSchedule);
                result.Schedule = resultSchedule;
                result.ReservedAmount = _data.GetU64(offset);
                offset += 8;
                result.Reserved = _data.GetBytes(offset, 32);
                offset += 32;
                return offset - initialOffset;
            }
        }

        public partial class VariableRateConfig
        {
            public ulong Amount { get; set; }

            public ulong DurationSec { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU64(Amount, offset);
                offset += 8;
                _data.WriteU64(DurationSec, offset);
                offset += 8;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out VariableRateConfig result)
            {
                int offset = initialOffset;
                result = new VariableRateConfig();
                result.Amount = _data.GetU64(offset);
                offset += 8;
                result.DurationSec = _data.GetU64(offset);
                offset += 8;
                return offset - initialOffset;
            }
        }

        public partial class VariableRateReward
        {
            public Number128 RewardRate { get; set; }

            public ulong RewardLastUpdatedTs { get; set; }

            public Number128 AccruedRewardPerRarityPoint { get; set; }

            public byte[] Reserved { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                offset += RewardRate.Serialize(_data, offset);
                _data.WriteU64(RewardLastUpdatedTs, offset);
                offset += 8;
                offset += AccruedRewardPerRarityPoint.Serialize(_data, offset);
                _data.WriteSpan(Reserved, offset);
                offset += Reserved.Length;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out VariableRateReward result)
            {
                int offset = initialOffset;
                result = new VariableRateReward();
                offset += Number128.Deserialize(_data, offset, out var resultRewardRate);
                result.RewardRate = resultRewardRate;
                result.RewardLastUpdatedTs = _data.GetU64(offset);
                offset += 8;
                offset += Number128.Deserialize(_data, offset, out var resultAccruedRewardPerRarityPoint);
                result.AccruedRewardPerRarityPoint = resultAccruedRewardPerRarityPoint;
                result.Reserved = _data.GetBytes(offset, 32);
                offset += 32;
                return offset - initialOffset;
            }
        }

        public enum FarmerState : byte
        {
            Unstaked,
            Staked,
            PendingCooldown
        }

        public enum RewardType : byte
        {
            Variable,
            Fixed
        }
    }

    public partial class GemFarmClient : TransactionalBaseClient<GemFarmErrorKind>
    {
        public GemFarmClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient, PublicKey programId) : base(rpcClient, streamingRpcClient, programId)
        {
        }

        public async Task<ProgramAccountsResultWrapper<List<AuthorizationProof>>> GetAuthorizationProofsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<MemCmp>{new MemCmp{Bytes = AuthorizationProof.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new ProgramAccountsResultWrapper<List<AuthorizationProof>>(res);
            List<AuthorizationProof> resultingAccounts = new List<AuthorizationProof>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => AuthorizationProof.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new ProgramAccountsResultWrapper<List<AuthorizationProof>>(res, resultingAccounts);
        }

        public async Task<ProgramAccountsResultWrapper<List<Farmer>>> GetFarmersAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<MemCmp>{new MemCmp{Bytes = Farmer.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new ProgramAccountsResultWrapper<List<Farmer>>(res);
            List<Farmer> resultingAccounts = new List<Farmer>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Farmer.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new ProgramAccountsResultWrapper<List<Farmer>>(res, resultingAccounts);
        }

        public async Task<ProgramAccountsResultWrapper<List<Farm>>> GetFarmsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<MemCmp>{new MemCmp{Bytes = Farm.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new ProgramAccountsResultWrapper<List<Farm>>(res);
            List<Farm> resultingAccounts = new List<Farm>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Farm.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new ProgramAccountsResultWrapper<List<Farm>>(res, resultingAccounts);
        }

        public async Task<AccountResultWrapper<AuthorizationProof>> GetAuthorizationProofAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new AccountResultWrapper<AuthorizationProof>(res);
            var resultingAccount = AuthorizationProof.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new AccountResultWrapper<AuthorizationProof>(res, resultingAccount);
        }

        public async Task<AccountResultWrapper<Farmer>> GetFarmerAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new AccountResultWrapper<Farmer>(res);
            var resultingAccount = Farmer.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new AccountResultWrapper<Farmer>(res, resultingAccount);
        }

        public async Task<AccountResultWrapper<Farm>> GetFarmAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new AccountResultWrapper<Farm>(res);
            var resultingAccount = Farm.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new AccountResultWrapper<Farm>(res, resultingAccount);
        }

        public async Task<SubscriptionState> SubscribeAuthorizationProofAsync(string accountAddress, Action<SubscriptionState, ResponseValue<AccountInfo>, AuthorizationProof> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                AuthorizationProof parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = AuthorizationProof.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeFarmerAsync(string accountAddress, Action<SubscriptionState, ResponseValue<AccountInfo>, Farmer> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                Farmer parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = Farmer.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeFarmAsync(string accountAddress, Action<SubscriptionState, ResponseValue<AccountInfo>, Farm> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                Farm parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = Farm.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<RequestResult<string>> SendInitFarmAsync(InitFarmAccounts accounts, byte bumpAuth, byte bumpTreasury, RewardType rewardTypeA, RewardType rewardTypeB, FarmConfig farmConfig, MaxCounts maxCounts, PublicKey farmTreasury, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.InitFarm(accounts, bumpAuth, bumpTreasury, rewardTypeA, rewardTypeB, farmConfig, maxCounts, farmTreasury, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendUpdateFarmAsync(UpdateFarmAccounts accounts, FarmConfig config, PublicKey manager, MaxCounts maxCounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.UpdateFarm(accounts, config, manager, maxCounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendPayoutFromTreasuryAsync(PayoutFromTreasuryAccounts accounts, byte bumpAuth, byte bumpTreasury, ulong lamports, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.PayoutFromTreasury(accounts, bumpAuth, bumpTreasury, lamports, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendAddToBankWhitelistAsync(AddToBankWhitelistAccounts accounts, byte bumpAuth, byte whitelistType, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.AddToBankWhitelist(accounts, bumpAuth, whitelistType, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRemoveFromBankWhitelistAsync(RemoveFromBankWhitelistAccounts accounts, byte bumpAuth, byte bumpWl, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.RemoveFromBankWhitelist(accounts, bumpAuth, bumpWl, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendInitFarmerAsync(InitFarmerAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.InitFarmer(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendStakeAsync(StakeAccounts accounts, byte bumpAuth, byte bumpFarmer, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.Stake(accounts, bumpAuth, bumpFarmer, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendUnstakeAsync(UnstakeAccounts accounts, byte bumpAuth, byte bumpTreasury, byte bumpFarmer, bool skipRewards, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.Unstake(accounts, bumpAuth, bumpTreasury, bumpFarmer, skipRewards, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendClaimAsync(ClaimAccounts accounts, byte bumpAuth, byte bumpFarmer, byte bumpPotA, byte bumpPotB, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.Claim(accounts, bumpAuth, bumpFarmer, bumpPotA, bumpPotB, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendFlashDepositAsync(FlashDepositAccounts accounts, byte bumpFarmer, byte bumpVaultAuth, byte bumpRarity, ulong amount, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.FlashDeposit(accounts, bumpFarmer, bumpVaultAuth, bumpRarity, amount, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRefreshFarmerAsync(RefreshFarmerAccounts accounts, byte bump, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.RefreshFarmer(accounts, bump, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRefreshFarmerSignedAsync(RefreshFarmerSignedAccounts accounts, byte bump, bool reenroll, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.RefreshFarmerSigned(accounts, bump, reenroll, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendAuthorizeFunderAsync(AuthorizeFunderAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.AuthorizeFunder(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendDeauthorizeFunderAsync(DeauthorizeFunderAccounts accounts, byte bump, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.DeauthorizeFunder(accounts, bump, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendFundRewardAsync(FundRewardAccounts accounts, byte bumpProof, byte bumpPot, VariableRateConfig variableRateConfig, FixedRateConfig fixedRateConfig, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.FundReward(accounts, bumpProof, bumpPot, variableRateConfig, fixedRateConfig, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendCancelRewardAsync(CancelRewardAccounts accounts, byte bumpAuth, byte bumpPot, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.CancelReward(accounts, bumpAuth, bumpPot, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendLockRewardAsync(LockRewardAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.LockReward(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendAddRaritiesToBankAsync(AddRaritiesToBankAccounts accounts, byte bumpAuth, RarityConfig[] rarityConfigs, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            TransactionInstruction instr = Program.GemFarmProgram.AddRaritiesToBank(accounts, bumpAuth, rarityConfigs, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        protected override Dictionary<uint, ProgramError<GemFarmErrorKind>> BuildErrorsDictionary()
        {
            return new Dictionary<uint, ProgramError<GemFarmErrorKind>>{};
        }
    }

    namespace Program
    {
        public class InitFarmAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmManager { get; set; }

            public PublicKey FarmAuthority { get; set; }

            public PublicKey RewardAPot { get; set; }

            public PublicKey RewardAMint { get; set; }

            public PublicKey RewardBPot { get; set; }

            public PublicKey RewardBMint { get; set; }

            public PublicKey Bank { get; set; }

            public PublicKey GemBank { get; set; }

            public PublicKey Payer { get; set; }

            public PublicKey FeeAcc { get; set; }

            public PublicKey Rent { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class UpdateFarmAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmManager { get; set; }
        }

        public class PayoutFromTreasuryAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmManager { get; set; }

            public PublicKey FarmAuthority { get; set; }

            public PublicKey FarmTreasury { get; set; }

            public PublicKey Destination { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class AddToBankWhitelistAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmManager { get; set; }

            public PublicKey FarmAuthority { get; set; }

            public PublicKey Bank { get; set; }

            public PublicKey AddressToWhitelist { get; set; }

            public PublicKey WhitelistProof { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey GemBank { get; set; }
        }

        public class RemoveFromBankWhitelistAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmManager { get; set; }

            public PublicKey FarmAuthority { get; set; }

            public PublicKey Bank { get; set; }

            public PublicKey AddressToRemove { get; set; }

            public PublicKey WhitelistProof { get; set; }

            public PublicKey GemBank { get; set; }
        }

        public class InitFarmerAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey Farmer { get; set; }

            public PublicKey Identity { get; set; }

            public PublicKey Bank { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey GemBank { get; set; }

            public PublicKey Payer { get; set; }

            public PublicKey FeeAcc { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class StakeAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmAuthority { get; set; }

            public PublicKey Farmer { get; set; }

            public PublicKey Identity { get; set; }

            public PublicKey Bank { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey GemBank { get; set; }

            public PublicKey FeeAcc { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class UnstakeAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmAuthority { get; set; }

            public PublicKey FarmTreasury { get; set; }

            public PublicKey Farmer { get; set; }

            public PublicKey Identity { get; set; }

            public PublicKey Bank { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey GemBank { get; set; }

            public PublicKey FeeAcc { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class ClaimAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmAuthority { get; set; }

            public PublicKey Farmer { get; set; }

            public PublicKey Identity { get; set; }

            public PublicKey RewardAPot { get; set; }

            public PublicKey RewardAMint { get; set; }

            public PublicKey RewardADestination { get; set; }

            public PublicKey RewardBPot { get; set; }

            public PublicKey RewardBMint { get; set; }

            public PublicKey RewardBDestination { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class FlashDepositAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmAuthority { get; set; }

            public PublicKey Farmer { get; set; }

            public PublicKey Identity { get; set; }

            public PublicKey Bank { get; set; }

            public PublicKey Vault { get; set; }

            public PublicKey VaultAuthority { get; set; }

            public PublicKey GemBox { get; set; }

            public PublicKey GemDepositReceipt { get; set; }

            public PublicKey GemSource { get; set; }

            public PublicKey GemMint { get; set; }

            public PublicKey GemRarity { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey Rent { get; set; }

            public PublicKey GemBank { get; set; }

            public PublicKey FeeAcc { get; set; }
        }

        public class RefreshFarmerAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey Farmer { get; set; }

            public PublicKey Identity { get; set; }
        }

        public class RefreshFarmerSignedAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey Farmer { get; set; }

            public PublicKey Identity { get; set; }
        }

        public class AuthorizeFunderAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmManager { get; set; }

            public PublicKey FunderToAuthorize { get; set; }

            public PublicKey AuthorizationProof { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class DeauthorizeFunderAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmManager { get; set; }

            public PublicKey FunderToDeauthorize { get; set; }

            public PublicKey AuthorizationProof { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class FundRewardAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey AuthorizationProof { get; set; }

            public PublicKey AuthorizedFunder { get; set; }

            public PublicKey RewardPot { get; set; }

            public PublicKey RewardSource { get; set; }

            public PublicKey RewardMint { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class CancelRewardAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmManager { get; set; }

            public PublicKey FarmAuthority { get; set; }

            public PublicKey RewardPot { get; set; }

            public PublicKey RewardDestination { get; set; }

            public PublicKey RewardMint { get; set; }

            public PublicKey Receiver { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }

            public PublicKey Rent { get; set; }
        }

        public class LockRewardAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmManager { get; set; }

            public PublicKey RewardMint { get; set; }
        }

        public class AddRaritiesToBankAccounts
        {
            public PublicKey Farm { get; set; }

            public PublicKey FarmManager { get; set; }

            public PublicKey FarmAuthority { get; set; }

            public PublicKey Bank { get; set; }

            public PublicKey GemBank { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public static class GemFarmProgram
        {
            public static TransactionInstruction InitFarm(InitFarmAccounts accounts, byte bumpAuth, byte bumpTreasury, RewardType rewardTypeA, RewardType rewardTypeB, FarmConfig farmConfig, MaxCounts maxCounts, PublicKey farmTreasury, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, true), AccountMeta.ReadOnly(accounts.FarmManager, true), AccountMeta.Writable(accounts.FarmAuthority, false), AccountMeta.Writable(accounts.RewardAPot, false), AccountMeta.ReadOnly(accounts.RewardAMint, false), AccountMeta.Writable(accounts.RewardBPot, false), AccountMeta.ReadOnly(accounts.RewardBMint, false), AccountMeta.Writable(accounts.Bank, true), AccountMeta.ReadOnly(accounts.GemBank, false), AccountMeta.Writable(accounts.Payer, true), AccountMeta.Writable(accounts.FeeAcc, false), AccountMeta.ReadOnly(accounts.Rent, false), AccountMeta.ReadOnly(accounts.TokenProgram, false), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16663941516183059723UL, offset);
                offset += 8;
                _data.WriteU8(bumpAuth, offset);
                offset += 1;
                _data.WriteU8(bumpTreasury, offset);
                offset += 1;
                _data.WriteU8((byte)rewardTypeA, offset);
                offset += 1;
                _data.WriteU8((byte)rewardTypeB, offset);
                offset += 1;
                offset += farmConfig.Serialize(_data, offset);
                if (maxCounts != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    offset += maxCounts.Serialize(_data, offset);
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                _data.WritePubKey(farmTreasury, offset);
                offset += 32;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction UpdateFarm(UpdateFarmAccounts accounts, FarmConfig config, PublicKey manager, MaxCounts maxCounts, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.ReadOnly(accounts.FarmManager, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(13662291217567917406UL, offset);
                offset += 8;
                if (config != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    offset += config.Serialize(_data, offset);
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (manager != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WritePubKey(manager, offset);
                    offset += 32;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (maxCounts != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    offset += maxCounts.Serialize(_data, offset);
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction PayoutFromTreasury(PayoutFromTreasuryAccounts accounts, byte bumpAuth, byte bumpTreasury, ulong lamports, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.ReadOnly(accounts.FarmManager, true), AccountMeta.ReadOnly(accounts.FarmAuthority, false), AccountMeta.Writable(accounts.FarmTreasury, false), AccountMeta.Writable(accounts.Destination, false), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(11282392250456979780UL, offset);
                offset += 8;
                _data.WriteU8(bumpAuth, offset);
                offset += 1;
                _data.WriteU8(bumpTreasury, offset);
                offset += 1;
                _data.WriteU64(lamports, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction AddToBankWhitelist(AddToBankWhitelistAccounts accounts, byte bumpAuth, byte whitelistType, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.ReadOnly(accounts.Farm, false), AccountMeta.Writable(accounts.FarmManager, true), AccountMeta.ReadOnly(accounts.FarmAuthority, false), AccountMeta.Writable(accounts.Bank, false), AccountMeta.ReadOnly(accounts.AddressToWhitelist, false), AccountMeta.Writable(accounts.WhitelistProof, false), AccountMeta.ReadOnly(accounts.SystemProgram, false), AccountMeta.ReadOnly(accounts.GemBank, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(8680217155010113097UL, offset);
                offset += 8;
                _data.WriteU8(bumpAuth, offset);
                offset += 1;
                _data.WriteU8(whitelistType, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction RemoveFromBankWhitelist(RemoveFromBankWhitelistAccounts accounts, byte bumpAuth, byte bumpWl, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.ReadOnly(accounts.Farm, false), AccountMeta.Writable(accounts.FarmManager, true), AccountMeta.Writable(accounts.FarmAuthority, false), AccountMeta.Writable(accounts.Bank, false), AccountMeta.ReadOnly(accounts.AddressToRemove, false), AccountMeta.Writable(accounts.WhitelistProof, false), AccountMeta.ReadOnly(accounts.GemBank, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(7578980433603793535UL, offset);
                offset += 8;
                _data.WriteU8(bumpAuth, offset);
                offset += 1;
                _data.WriteU8(bumpWl, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction InitFarmer(InitFarmerAccounts accounts, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.Writable(accounts.Farmer, false), AccountMeta.ReadOnly(accounts.Identity, true), AccountMeta.Writable(accounts.Bank, false), AccountMeta.Writable(accounts.Vault, false), AccountMeta.ReadOnly(accounts.GemBank, false), AccountMeta.Writable(accounts.Payer, true), AccountMeta.Writable(accounts.FeeAcc, false), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16129082575378154809UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction Stake(StakeAccounts accounts, byte bumpAuth, byte bumpFarmer, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.ReadOnly(accounts.FarmAuthority, false), AccountMeta.Writable(accounts.Farmer, false), AccountMeta.Writable(accounts.Identity, true), AccountMeta.ReadOnly(accounts.Bank, false), AccountMeta.Writable(accounts.Vault, false), AccountMeta.ReadOnly(accounts.GemBank, false), AccountMeta.Writable(accounts.FeeAcc, false), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(7832834834166362318UL, offset);
                offset += 8;
                _data.WriteU8(bumpAuth, offset);
                offset += 1;
                _data.WriteU8(bumpFarmer, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction Unstake(UnstakeAccounts accounts, byte bumpAuth, byte bumpTreasury, byte bumpFarmer, bool skipRewards, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.ReadOnly(accounts.FarmAuthority, false), AccountMeta.Writable(accounts.FarmTreasury, false), AccountMeta.Writable(accounts.Farmer, false), AccountMeta.Writable(accounts.Identity, true), AccountMeta.ReadOnly(accounts.Bank, false), AccountMeta.Writable(accounts.Vault, false), AccountMeta.ReadOnly(accounts.GemBank, false), AccountMeta.Writable(accounts.FeeAcc, false), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16227169627991138138UL, offset);
                offset += 8;
                _data.WriteU8(bumpAuth, offset);
                offset += 1;
                _data.WriteU8(bumpTreasury, offset);
                offset += 1;
                _data.WriteU8(bumpFarmer, offset);
                offset += 1;
                _data.WriteBool(skipRewards, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction Claim(ClaimAccounts accounts, byte bumpAuth, byte bumpFarmer, byte bumpPotA, byte bumpPotB, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.ReadOnly(accounts.FarmAuthority, false), AccountMeta.Writable(accounts.Farmer, false), AccountMeta.Writable(accounts.Identity, true), AccountMeta.Writable(accounts.RewardAPot, false), AccountMeta.ReadOnly(accounts.RewardAMint, false), AccountMeta.Writable(accounts.RewardADestination, false), AccountMeta.Writable(accounts.RewardBPot, false), AccountMeta.ReadOnly(accounts.RewardBMint, false), AccountMeta.Writable(accounts.RewardBDestination, false), AccountMeta.ReadOnly(accounts.TokenProgram, false), AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false), AccountMeta.ReadOnly(accounts.SystemProgram, false), AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(15162669785878545982UL, offset);
                offset += 8;
                _data.WriteU8(bumpAuth, offset);
                offset += 1;
                _data.WriteU8(bumpFarmer, offset);
                offset += 1;
                _data.WriteU8(bumpPotA, offset);
                offset += 1;
                _data.WriteU8(bumpPotB, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction FlashDeposit(FlashDepositAccounts accounts, byte bumpFarmer, byte bumpVaultAuth, byte bumpRarity, ulong amount, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.ReadOnly(accounts.FarmAuthority, false), AccountMeta.Writable(accounts.Farmer, false), AccountMeta.Writable(accounts.Identity, true), AccountMeta.ReadOnly(accounts.Bank, false), AccountMeta.Writable(accounts.Vault, false), AccountMeta.ReadOnly(accounts.VaultAuthority, false), AccountMeta.Writable(accounts.GemBox, false), AccountMeta.Writable(accounts.GemDepositReceipt, false), AccountMeta.Writable(accounts.GemSource, false), AccountMeta.ReadOnly(accounts.GemMint, false), AccountMeta.ReadOnly(accounts.GemRarity, false), AccountMeta.ReadOnly(accounts.TokenProgram, false), AccountMeta.ReadOnly(accounts.SystemProgram, false), AccountMeta.ReadOnly(accounts.Rent, false), AccountMeta.ReadOnly(accounts.GemBank, false), AccountMeta.Writable(accounts.FeeAcc, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(5934815981167224049UL, offset);
                offset += 8;
                _data.WriteU8(bumpFarmer, offset);
                offset += 1;
                _data.WriteU8(bumpVaultAuth, offset);
                offset += 1;
                _data.WriteU8(bumpRarity, offset);
                offset += 1;
                _data.WriteU64(amount, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction RefreshFarmer(RefreshFarmerAccounts accounts, byte bump, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.Writable(accounts.Farmer, false), AccountMeta.ReadOnly(accounts.Identity, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(1423560260898551295UL, offset);
                offset += 8;
                _data.WriteU8(bump, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction RefreshFarmerSigned(RefreshFarmerSignedAccounts accounts, byte bump, bool reenroll, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.Writable(accounts.Farmer, false), AccountMeta.ReadOnly(accounts.Identity, true)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(6401765748556196514UL, offset);
                offset += 8;
                _data.WriteU8(bump, offset);
                offset += 1;
                _data.WriteBool(reenroll, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction AuthorizeFunder(AuthorizeFunderAccounts accounts, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.Writable(accounts.FarmManager, true), AccountMeta.ReadOnly(accounts.FunderToAuthorize, false), AccountMeta.Writable(accounts.AuthorizationProof, false), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(15373327814036179474UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction DeauthorizeFunder(DeauthorizeFunderAccounts accounts, byte bump, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.Writable(accounts.FarmManager, true), AccountMeta.ReadOnly(accounts.FunderToDeauthorize, false), AccountMeta.Writable(accounts.AuthorizationProof, false), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(2689294759396541003UL, offset);
                offset += 8;
                _data.WriteU8(bump, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction FundReward(FundRewardAccounts accounts, byte bumpProof, byte bumpPot, VariableRateConfig variableRateConfig, FixedRateConfig fixedRateConfig, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.ReadOnly(accounts.AuthorizationProof, false), AccountMeta.Writable(accounts.AuthorizedFunder, true), AccountMeta.Writable(accounts.RewardPot, false), AccountMeta.Writable(accounts.RewardSource, false), AccountMeta.ReadOnly(accounts.RewardMint, false), AccountMeta.ReadOnly(accounts.TokenProgram, false), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(4550490901976789692UL, offset);
                offset += 8;
                _data.WriteU8(bumpProof, offset);
                offset += 1;
                _data.WriteU8(bumpPot, offset);
                offset += 1;
                if (variableRateConfig != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    offset += variableRateConfig.Serialize(_data, offset);
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (fixedRateConfig != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    offset += fixedRateConfig.Serialize(_data, offset);
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction CancelReward(CancelRewardAccounts accounts, byte bumpAuth, byte bumpPot, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.Writable(accounts.FarmManager, true), AccountMeta.ReadOnly(accounts.FarmAuthority, false), AccountMeta.Writable(accounts.RewardPot, false), AccountMeta.Writable(accounts.RewardDestination, false), AccountMeta.ReadOnly(accounts.RewardMint, false), AccountMeta.Writable(accounts.Receiver, false), AccountMeta.ReadOnly(accounts.TokenProgram, false), AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false), AccountMeta.ReadOnly(accounts.SystemProgram, false), AccountMeta.ReadOnly(accounts.Rent, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16231431113884910020UL, offset);
                offset += 8;
                _data.WriteU8(bumpAuth, offset);
                offset += 1;
                _data.WriteU8(bumpPot, offset);
                offset += 1;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction LockReward(LockRewardAccounts accounts, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.Writable(accounts.Farm, false), AccountMeta.Writable(accounts.FarmManager, true), AccountMeta.ReadOnly(accounts.RewardMint, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(11331393439723177282UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static TransactionInstruction AddRaritiesToBank(AddRaritiesToBankAccounts accounts, byte bumpAuth, RarityConfig[] rarityConfigs, PublicKey programId)
            {
                List<AccountMeta> keys = new()
                {AccountMeta.ReadOnly(accounts.Farm, false), AccountMeta.Writable(accounts.FarmManager, true), AccountMeta.ReadOnly(accounts.FarmAuthority, false), AccountMeta.ReadOnly(accounts.Bank, false), AccountMeta.ReadOnly(accounts.GemBank, false), AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(2505557016243900731UL, offset);
                offset += 8;
                _data.WriteU8(bumpAuth, offset);
                offset += 1;
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