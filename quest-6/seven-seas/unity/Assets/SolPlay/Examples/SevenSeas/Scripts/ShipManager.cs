using System;
using System.Collections.Generic;
using Frictionless;
using SevenSeas.Types;
using Solana.Unity.Wallet;
using SolPlay.Scripts.Services;
using SolPlay.Scripts.Ui;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public ShipBehaviour ShipPrefab;
    public TreasureChest TreasureChestPrefab;
    public Dictionary<string, ShipBehaviour> Ships = new Dictionary<string, ShipBehaviour>();
    public Dictionary<string, TreasureChest> Chests = new Dictionary<string, TreasureChest>();
    public Tile[][] Board { get; set; }

    private void Awake()
    {
        ServiceFactory.RegisterSingleton(this);
    }

    private void Start()
    {
        MessageRouter.AddHandler<SevenSeasService.SolHunterGameDataChangedMessage>(OnGameDataChangedMessage);   
        MessageRouter.AddHandler<SevenSeasService.ShipShotMessage>(OnShipShotMessage);   
    }

    private void OnShipShotMessage(SevenSeasService.ShipShotMessage obj)
    {
        if (Ships.ContainsKey(obj.ShipOwner))
        {
            Ships[obj.ShipOwner].Shoot();
        }
    }

    private void OnGameDataChangedMessage(SevenSeasService.SolHunterGameDataChangedMessage obj)
    {
        InitWithData(obj.GameDataAccount.Board);
    }

    public bool TryGetShipByOwner(PublicKey owner, out ShipBehaviour shipBehaviour)
    {
        foreach (var keypair in Ships)
        {
            if (keypair.Value.currentTile.Player == owner)
            {
                shipBehaviour = keypair.Value;
                return true;
            }
        }

        shipBehaviour = null;
        return false;
    }
    
    public void InitWithData(Tile[][] board)
    {
        var length = board.GetLength(0);

        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < length; x++)
            {
                Tile tile = board[x][y];
                if (tile.State == SevenSeasService.STATE_PLAYER)
                {
                    if (!Ships.ContainsKey(tile.Player))
                    {
                        var newShip = SpawnShip(new Vector2(x, -y), tile);
                        Ships.Add(tile.Player, newShip);
                    }
                    else
                    {
                        Ships[tile.Player].SetNewTargetPosition(new Vector2(x, -y), tile);
                    }
                }

                if (tile.State == SevenSeasService.STATE_CHEST)
                {
                    var key = x + "_" + y;
                    if (!Chests.ContainsKey(key))
                    {
                        var newChest = SpawnTreasuryChest(new Vector2(x, -y));
                        Chests.Add(key, newChest);
                    }
                }
            }
        }

        DestroyAllShipsThatAreNotOnTheBoard(board);
        DestroyAllChestsThatAreNotOnTheBoard(board);
    }

    private void DestroyAllShipsThatAreNotOnTheBoard(Tile[][] board)
    {
        var length = board.GetLength(0);

        List<KeyValuePair<string, ShipBehaviour>> deadShips = new List<KeyValuePair<string, ShipBehaviour>>();
        
        foreach (KeyValuePair<string, ShipBehaviour> ship in Ships)
        {
            bool found = false;
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    Tile tile = board[x][y];
                    if (tile.State == SevenSeasService.STATE_PLAYER && tile.Player == ship.Key)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }

            if (!found)
            {
                deadShips.Add(ship);
            }
        }

        foreach (KeyValuePair<string, ShipBehaviour> valuePair in deadShips) 
        {
            MessageRouter.RaiseMessage(new BlimpSystem.Show3DBlimpMessage("Destroyed", valuePair.Value.transform.position));
            Destroy(valuePair.Value.gameObject);
            Ships.Remove(valuePair.Key);
        }
    }

    private void DestroyAllChestsThatAreNotOnTheBoard(Tile[][] board)
    {
        var length = board.GetLength(0);

        List<KeyValuePair<string, TreasureChest>> deadChests = new List<KeyValuePair<string, TreasureChest>>();
        
        foreach (KeyValuePair<string, TreasureChest> chest in Chests)
        {
            bool found = false;
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    Tile tile = board[x][y];
                    if (tile.State == SevenSeasService.STATE_CHEST && x+"_"+y == chest.Key)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }

            if (!found)
            {
                deadChests.Add(chest);
            }
        }

        foreach (var chest in deadChests) 
        {
            Destroy(chest.Value.gameObject);
            Chests.Remove(chest.Key);
        }
    }

    private ShipBehaviour SpawnShip(Vector2 startPosition, Tile tile)
    {
        var newShip = Instantiate(ShipPrefab);
        var walletPubKey = ServiceFactory.Resolve<WalletHolderService>().InGameWallet.Account.PublicKey;
        newShip.Init(startPosition, tile, walletPubKey == tile.Player);
        return newShip;
    }
    
    private TreasureChest SpawnTreasuryChest(Vector2 startPosition)
    {
        var newChest = Instantiate(TreasureChestPrefab);
        newChest.Init(startPosition);
        return newChest;
    }
}