using System.Text;
using Solana.Unity.Wallet;
using UnityEngine;

namespace SolPlay.Staking
{
    public class GemBankPDAHelper
    {
        public static PublicKey BankProgramm = new PublicKey(
            "bankHHdqMuaaST4qQk6mkzxGeKPHWmqdgor6Gs8r88m"
        );

        public static PublicKey Bank = new PublicKey(
            "6qow2ZSoCNLupaohidmuuNUCoJ8fFpBeddx4JCc9M6at"
        );
        
        public static PublicKey FindVaultPDA(PublicKey creator, out byte vaultBump)
        {
            if (!PublicKey.TryFindProgramAddress(new[]
                    {
                        Encoding.UTF8.GetBytes("vault"), Bank.KeyBytes, creator.KeyBytes
                    },
                    BankProgramm, out PublicKey vaultPda, out vaultBump))
            {
                Debug.LogError("Could not find vault address");
                return null;
            }

            return vaultPda;
        }   
        
        public static PublicKey findGemBoxPDA(PublicKey vault, PublicKey mint, out byte vaultBump)
        {
            if (!PublicKey.TryFindProgramAddress(new[]
                    {
                        Encoding.UTF8.GetBytes("gem_box"), vault.KeyBytes, mint.KeyBytes
                    },
                    BankProgramm, out PublicKey GemBoxPDA, out vaultBump))
            {
                Debug.LogError("Could not find GemBoxPDA address");
                return null;
            }

            return GemBoxPDA;
        }   
        
        public static PublicKey findGdrPDA(PublicKey vault, PublicKey mint, out byte vaultBump)
        {
            if (!PublicKey.TryFindProgramAddress(new[]
                    {
                        Encoding.UTF8.GetBytes("gem_deposit_receipt"), vault.KeyBytes, mint.KeyBytes
                    },
                    BankProgramm, out PublicKey GdrPDA, out vaultBump))
            {
                Debug.LogError("Could not find GdrPDA address");
                return null;
            }

            return GdrPDA;
        }   
        
        public static PublicKey findVaultAuthorityPDA(PublicKey vault, out byte vaultBump)
        {
            if (!PublicKey.TryFindProgramAddress(new[]
                    {
                        vault.KeyBytes
                    },
                    BankProgramm, out PublicKey VaultAuthority, out vaultBump))
            {
                Debug.LogError("Could not find VaultAuthority address");
                return null;
            }

            return VaultAuthority;
        }   
        
        public static PublicKey findWhitelistProofPDA(PublicKey bank, PublicKey whitelistedAddress, out byte vaultBump)
        {
            if (!PublicKey.TryFindProgramAddress(new[]
                    {
                        Encoding.UTF8.GetBytes("whitelist"), bank.KeyBytes, whitelistedAddress.KeyBytes
                    },
                    BankProgramm, out PublicKey WhitelistProof, out vaultBump))
            {
                Debug.LogError("Could not find WhitelistProof address");
                return null;
            }

            return WhitelistProof;
        }   
        
        public static PublicKey findRarityPDA(PublicKey bank, PublicKey mint, out byte vaultBump)
        {
            if (!PublicKey.TryFindProgramAddress(new[]
                    {
                        Encoding.UTF8.GetBytes("gem_rarity"), bank.KeyBytes, mint.KeyBytes
                    },
                    BankProgramm, out PublicKey Rarity, out vaultBump))
            {
                Debug.LogError("Could not find Rarity address");
                return null;
            }

            return Rarity;
        }   
    }
}