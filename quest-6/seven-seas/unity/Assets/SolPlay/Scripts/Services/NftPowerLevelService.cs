using System.Collections;
using Frictionless;
using Solana.Unity.SDK.Nft;
using UnityEngine;

namespace SolPlay.Scripts.Services
{
    /// <summary>
    /// Handles logic to calculate NFT power level or whatever you like to do with the NFTs.
    /// It merely an example on how to use NFT attributes. 
    /// </summary>
    public class NftPowerLevelService : MonoBehaviour, IMultiSceneSingleton
    {
        private void Awake()
        {
            if (ServiceFactory.Resolve<NftPowerLevelService>() != null)
            {
                Destroy(gameObject);
                return;
            }

            ServiceFactory.RegisterSingleton(this);
        }

        public int GetPowerLevelFromNft(Nft solPlayNft)
        {
            var nftService = ServiceFactory.Resolve<NftService>();
            if (nftService.IsBeaverNft(solPlayNft))
            {
                return CalculateBeaverPower(solPlayNft);
            }

            return 1;
        }

        // Just some power level calculations, you could to what ever with this. For example take the value from 
        // one of the attributes as damage for you character for example.
        private int CalculateBeaverPower(Nft beaverSolPlayNft)
        {
            int bonusBeaverPower = 0;
            foreach (var entry in beaverSolPlayNft.metaplexData.data.offchainData.attributes)
            {
                switch (entry.value)
                {
                    case "none":
                        bonusBeaverPower += 1;
                        break;
                    case "EvilOtter":
                        bonusBeaverPower += 450;
                        break;
                    case "BlueEyes":
                        bonusBeaverPower += 25;
                        break;
                    case "GreenEyes":
                        bonusBeaverPower += 25;
                        break;
                    case "Sharingan":
                        bonusBeaverPower += 350;
                        break;
                    case "RabbitKing":
                        bonusBeaverPower += 9999;
                        break;
                    case "Chicken":
                        bonusBeaverPower += 7;
                        break;
                    case "Beer":
                        bonusBeaverPower += 6;
                        break;
                    case "SimpleStick":
                        bonusBeaverPower += 5;
                        break;
                    case "GolTooth":
                        bonusBeaverPower += 16;
                        break;
                    case "Brezel":
                        bonusBeaverPower += 22;
                        break;
                    case "PrideCap":
                        bonusBeaverPower += 17;
                        break;
                    case "Santahat":
                        bonusBeaverPower += 12;
                        break;
                    case "SunGlasses":
                        bonusBeaverPower += 13;
                        break;
                    case "Suit":
                        bonusBeaverPower += 12;
                        break;
                    case "LederHosen":
                        bonusBeaverPower += 14;
                        break;
                    case "XmasBulbs":
                        bonusBeaverPower += 17;
                        break;
                    case "BPhone":
                        bonusBeaverPower += 8;
                        break;
                    default:
                        bonusBeaverPower += 3;
                        break;
                }
            }

            return 5 + bonusBeaverPower;
        }

        public float GetTotalPowerLevel()
        {
            var nftService = ServiceFactory.Resolve<NftService>();
            int result = 0;
            foreach (Nft nft in nftService.MetaPlexNFts)
            {
                result += GetPowerLevelFromNft(nft);
            }

            return result;
        }

        public IEnumerator HandleNewSceneLoaded()
        {
            yield return null;
        }
    }
}