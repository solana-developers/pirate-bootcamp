using System.Collections;
using CandyMachineV2;
using Frictionless;
using Solana.Unity.Wallet;
using UnityEngine;

namespace SolPlay.Scripts.Services
{
    /// <summary>
    /// WIP This will make the game hopefully compliant with Apple Store Requirements.
    /// The say we are not allowed to put it in if we use NFTs which are not also available via In app purchases.
    /// </summary>
    public class IapService : MonoBehaviour, IMultiSceneSingleton
    {
        void Awake()
        {
            if (ServiceFactory.Resolve<IapService>() != null)
            {
                Destroy(gameObject);
                return;
            }

            ServiceFactory.RegisterSingleton(this);
        }

        public async void OnNftIapDone(bool wasBought)
        {
            var baseWallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;
            var account = baseWallet.Account;
            var candy = await CandyMachineUtils.MintOneToken(account,
                new PublicKey("D1cEd7k6BK6cX8rwBMp5hSUzRheZxqdrmXQvMLst4Mrn"), baseWallet.ActiveRpcClient);
            Debug.Log(candy);
        }

        public IEnumerator HandleNewSceneLoaded()
        {
            yield return null;
        }
    }
}