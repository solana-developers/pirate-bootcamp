using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Frictionless;
using Solana.Unity.Rpc.Models;
using Solana.Unity.SDK.Nft;
using Solana.Unity.Wallet;
using SolPlay.DeeplinksNftExample.Scripts;
using SolPlay.DeeplinksNftExample.Scripts.OrcaWhirlPool;
using SolPlay.Orca.OrcaWhirlPool;
using SolPlay.Scripts;
using SolPlay.Scripts.Services;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class OrcaSwapWidget : MonoBehaviour
{
    public PoolListItem PoolListItemPrefab;
    public GameObject PoolListItemRoot;

    public List<string> PoolIdWhiteList = new List<string>();

    private void Start()
    {
        MessageRouter.AddHandler<WalletLoggedInMessage>(OnWalletLoggedInMessage);
        if (ServiceFactory.Resolve<WalletHolderService>().IsLoggedIn)
        {
            InitPools(true);
        }
    }

    private void OnWalletLoggedInMessage(WalletLoggedInMessage message)
    {
        InitPools(true);
    }

    /// <summary>
    /// Getting all pools without the white list is very expensive since it uses get programm accounts.
    /// public RPCs will have this call blocked. So only use it when you have our own rpc setup and want to get a list of
    /// pools to use. I added a list of main net pools in the file OrcaMainNetPoolList.txt.
    /// </summary>
    /// <param name="whiteList"></param>
    private async void InitPools(bool whiteList)
    {
        var pools = new List<Whirlpool.Accounts.Whirlpool>();
        /*
         // With this you can get a bunch of pools from the whirl pool list that is saved in the whirlpool service
       var list = ServiceFactory.Resolve<OrcaWhirlpoolService>().OrcaApiPoolsData.whirlpools;
       for (var index = 0; index < 20; index++)
       {
           var entry = list[index];
           try
           {
               Whirlpool.Accounts.Whirlpool pool =
                   await ServiceFactory.Resolve<OrcaWhirlpoolService>().GetPool(entry.address);
               pools.Add(pool);
               Debug.Log($"pool: {entry.address} {entry.tokenA.symbol} {entry.tokenB.symbol}");
           }
           catch (Exception)
           {
               // May not exist on dev net
           }
       }
 
       initPools(pools);
       return; */
        Debug.Log("Start getting pools" + PoolIdWhiteList.Count);
        if (whiteList)
        {
            foreach (var entry in PoolIdWhiteList)
            {
                try
                {
                    Whirlpool.Accounts.Whirlpool pool = await ServiceFactory.Resolve<OrcaWhirlpoolService>().GetPool(entry);
                    pools.Add(pool);

                    //Debug.Log("add pool" + pool.TokenMintA);
                }
                catch (Exception e)
                {
                    // May not exist on dev net
                    Debug.Log($"Getting Pool error {e}");
                }
            }
        }
        else
        {
            // You can get all pools, but its very expensive call. So need a good RPC. public RPCs will permit it in general. 
            // You can also use the ORCA API which is saved in ServiceFactory.Resolve<OrcaWhirlpoolService>().OrcaApiPoolsData.whirlpools
            pools = await ServiceFactory.Resolve<OrcaWhirlpoolService>().GetPools();
            if (pools == null)
            {
                LoggingService
                    .LogWarning("Could not load pools. Are you connected to the internet?", true);
            }
        }

        initPools(pools);
    }

    private async void initPools(List<Whirlpool.Accounts.Whirlpool> pools)
    {
        var wallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;

        Debug.Log("pools" + pools.Count);
        for (var index = 0; index < pools.Count; index++)
        {
            Whirlpool.Accounts.Whirlpool pool = pools[index];
            PublicKey whirlPoolPda = OrcaPDAUtils.GetWhirlpoolPda(OrcaWhirlpoolService.WhirlpoolProgammId,
                pool.WhirlpoolsConfig,
                pool.TokenMintA, pool.TokenMintB, pool.TickSpacing);

            if (!PoolIdWhiteList.Contains(whirlPoolPda))
            {
                //continue;
            }

            PoolData poolData = new PoolData();

            poolData.Pool = pool;
            poolData.PoolPda = whirlPoolPda;

            /* Since this is very slow we get the data from the orca API instead 
            var accountInfoMintA = await wallet.ActiveRpcClient.GetTokenMintInfoAsync(pool.TokenMintA);
            var accountInfoMintB = await wallet.ActiveRpcClient.GetTokenMintInfoAsync(pool.TokenMintB);

            if (accountInfoMintA == null || accountInfoMintA.Result == null || accountInfoMintB == null ||
                accountInfoMintB.Result == null)
            {
                Debug.LogWarning($"Error:{accountInfoMintA.Reason} {accountInfoMintA} {accountInfoMintB}");
                continue;
            }
                poolData.TokenMintInfoA = accountInfoMintA.Result.Value;
                poolData.TokenMintInfoB = accountInfoMintB.Result.Value;
            */

            poolData.TokenA = ServiceFactory.Resolve<OrcaWhirlpoolService>().GetToken(pool.TokenMintA);
            poolData.TokenB = ServiceFactory.Resolve<OrcaWhirlpoolService>().GetToken(pool.TokenMintB);
            
            poolData.SymbolA = poolData.TokenA.symbol;
            poolData.SymbolB = poolData.TokenB.symbol;

            poolData.SpriteA = await OrcaWhirlpoolService.GetTokenIconSprite(pool.TokenMintA, poolData.SymbolA);
            poolData.SpriteB = await OrcaWhirlpoolService.GetTokenIconSprite(pool.TokenMintB, poolData.SymbolB);

            PoolListItem poolListItem = Instantiate(PoolListItemPrefab, PoolListItemRoot.transform);

            poolListItem.SetData(poolData, OpenSwapPopup);
        }
    }
    
    private void OpenSwapPopup(PoolListItem poolListItem)
    {
        var orcaSwapPopup = ServiceFactory.Resolve<OrcaSwapPopup>();
        if (orcaSwapPopup == null)
        {
            LoggingService
                .Log("You need to add the OrcaSwapPopup to the scene.", true);
            return;
        }

        orcaSwapPopup.Open(poolListItem.PoolData);
    }
}