using System;
using System.Text;
using Solana.Unity.Wallet;
using UnityEngine;

namespace SolPlay.DeeplinksNftExample.Scripts.OrcaWhirlPool
{
    public class OrcaPDAUtils
    {
        const string PDA_WHIRLPOOL_SEED = "whirlpool";
        const string PDA_POSITION_SEED = "position";
        const string PDA_METADATA_SEED = "metadata";
        const string PDA_TICK_ARRAY_SEED = "tick_array";
        const string PDA_FEE_TIER_SEED = "fee_tier";
        const string PDA_ORACLE_SEED = "oracle";
            
        public static PublicKey GetWhirlpoolPda(PublicKey programId, PublicKey whirlpoolsConfigKey,
            PublicKey tokenMintAKey, PublicKey tokenMintBKey, ushort tickSpacing)
        {
            if (!PublicKey.TryFindProgramAddress(new[]
                    {
                        Encoding.UTF8.GetBytes(PDA_WHIRLPOOL_SEED), whirlpoolsConfigKey.KeyBytes, tokenMintAKey.KeyBytes, tokenMintBKey.KeyBytes, BitConverter.GetBytes(tickSpacing)
                    },
                    programId, out PublicKey pda, out var bump))
            {
                Debug.LogError("Could not find whirl pool address");
                return null;
            }

            return pda;
        }
        
        public static PublicKey GetOracle(PublicKey programId, PublicKey whirlpoolAddress)
        {
            if (!PublicKey.TryFindProgramAddress(new[]
                    {
                        Encoding.UTF8.GetBytes(PDA_ORACLE_SEED), whirlpoolAddress.KeyBytes
                    },
                    programId, out PublicKey pda, out _))
            {
                Debug.LogError("Could not find oracle");
                return null;
            }

            return pda;
        }
        
        public static PublicKey GetTickArray(PublicKey programId, PublicKey whirlpoolAddress, int startTick)
        {
            if (!PublicKey.TryFindProgramAddress(new[]
                    {
                        Encoding.UTF8.GetBytes(PDA_TICK_ARRAY_SEED), 
                        whirlpoolAddress.KeyBytes, 
                        Encoding.UTF8.GetBytes(startTick.ToString())
                    },
                    programId, out PublicKey pda, out _))
            {
                Debug.LogError("Could not find tick array");
                return null;
            }

            return pda;
        }
    }
}