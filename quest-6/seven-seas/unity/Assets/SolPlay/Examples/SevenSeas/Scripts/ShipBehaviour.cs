using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Frictionless;
using SevenSeas.Types;
using Solana.Unity.SDK.Nft;
using Solana.Unity.Wallet;
using SolPlay.FlappyGame.Runtime.Scripts;
using SolPlay.Scripts.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipBehaviour : MonoBehaviour
{
    public List<GameObject> ShotPrefabs;

    public List<GameObject> UpgradeLevels;
    
    public Vector3 TargetPosition;
    public Vector2 LastGridPosition;
    public Vector3 LastPosition;
    public Vector3 UpVector = Vector3.left;
    public HealthBar HealthBar;
    public TextMeshProUGUI PublicKey;
    public RawImage Avatar;
    public GameObject RotationRoot;
    public GameObject SelectionCircleRoot;
    public Tile currentTile;
    public GameObject SelectionCircelFX;
    
    public float ScrenShakePower = 3;
    public float ScrenShakeDuration = 0.05f;
    
    public void Init(Vector2 startPosition, Tile tile, bool isPlayer)
    {
        currentTile = tile;
        transform.position = new Vector3(10 * startPosition.x + 5f, 1.4f, (10 * startPosition.y) - 5f);
        TargetPosition = transform.position;
        LastGridPosition = startPosition;
        HealthBar.SetHealth(tile.Health, tile.StartHealth);
        switch (tile.LookDirection)
        {
            case 0:
                RotationRoot.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 1:
                RotationRoot.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case 2:
                RotationRoot.transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case 3:
                RotationRoot.transform.rotation = Quaternion.Euler(0, 270, 0);
                break;
        }

        if (isPlayer)
        {
            Instantiate(SelectionCircelFX, SelectionCircleRoot.transform);
        }

        int shipLevel = Math.Clamp((int) tile.ShipLevel - 1, 0, 3);
        var model = Instantiate(UpgradeLevels[shipLevel], RotationRoot.transform);
        model.name = "model";
    }

    private void Update()
    {
        var transformPosition = transform.position - LastPosition;
        if (transformPosition.magnitude > 0.04f)
        {
           RotationRoot.transform.rotation = Quaternion.LookRotation(transformPosition, UpVector);   
        }
        else
        {
            switch (currentTile.LookDirection)
            {
                case 0:
                    RotationRoot.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case 1:
                    RotationRoot.transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case 2:
                    RotationRoot.transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case 3:
                    RotationRoot.transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;
            }
        }
        LastPosition = transform.position;
    }
    
    public void SetNewTargetPosition(Vector2 newPosition, Tile tile)
    {
        currentTile = tile;
        HealthBar.SetHealth(tile.Health, tile.StartHealth);
        PublicKey.text = tile.Player.ToString();
        SetNftAvatar(tile.Avatar);
        TargetPosition = new Vector3((10 * newPosition.x) + 5f, 1.4f, (10 * newPosition.y) - 5f);
        
        if ((newPosition - LastGridPosition).magnitude  > 5)
        {
            transform.DOKill();
            transform.position = new Vector3(10 * newPosition.x + 5f, 1.4f, (10 * newPosition.y) - 5f);
            LastPosition = transform.position;
        }
        else
        {
            transform.DOMove(TargetPosition, 2f).SetEase(Ease.Linear);
        }

        LastGridPosition = newPosition;
    }

    private async void SetNftAvatar(PublicKey avatarPublicKey)
    {
        var avatarNft = ServiceFactory.Resolve<NftService>().GetNftByMintAddress(avatarPublicKey);

        if (avatarNft == null)
        {
            avatarNft = Nft.TryLoadNftFromLocal(avatarPublicKey);
        }

        if (avatarNft == null)
        {
            try
            {
                var rpc = ServiceFactory.Resolve<WalletHolderService>().BaseWallet.ActiveRpcClient;
                avatarNft = await Nft.TryGetNftData(avatarPublicKey, rpc).AsUniTask();
            }
            catch (Exception e)
            {
                //Debug.LogError(e);
            }
        }

        if (avatarNft != null)
        {
            Avatar.texture = avatarNft.metaplexData.nftImage.file;
            Avatar.gameObject.SetActive(true);
        }
    }

    public void Shoot()
    {
        CameraShake.Shake(ScrenShakeDuration, ScrenShakePower);
        int shipLevel = Math.Clamp((int) currentTile.ShipLevel - 1, 0, 3);

        var shootInstance = Instantiate(ShotPrefabs[shipLevel]);
        shootInstance.transform.position = RotationRoot.transform.position;
        shootInstance.transform.rotation = RotationRoot.transform.rotation;
        StartCoroutine(KillDelayed(shootInstance));
    }

    private IEnumerator KillDelayed(GameObject shootInstance)
    {
        yield return new WaitForSeconds(2);
        Destroy(shootInstance);
    }
}