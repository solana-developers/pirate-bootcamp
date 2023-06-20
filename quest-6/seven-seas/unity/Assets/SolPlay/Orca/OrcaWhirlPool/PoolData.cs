using System;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Wallet;
using UnityEngine;

namespace SolPlay.Orca.OrcaWhirlPool
{
    [Serializable]
    public class PoolData
    {
        public Sprite SpriteA;
        public Sprite SpriteB;
        public string MintA;
        public string MintB;
        public string SymbolA;
        public string SymbolB;
        public Whirlpool.Accounts.Whirlpool Pool;
        //public TokenMintInfo TokenMintInfoA;
        //public TokenMintInfo TokenMintInfoB;
        public Token TokenA;
        public Token TokenB;
        public PublicKey PoolPda;
    }
}