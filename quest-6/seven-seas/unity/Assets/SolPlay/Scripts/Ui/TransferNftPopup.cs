using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using AllArt.Solana.Example;
using Frictionless;
using Solana.Unity.SDK.Nft;
using Solana.Unity.Wallet;
using SolPlay.Orca;
using SolPlay.Scripts;
using SolPlay.Scripts.Services;
using SolPlay.Scripts.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup with a QrCode Scanner to scan Solana Addresses.
/// </summary>
public class TransferNftPopup : BasePopup
{
    public TMP_InputField AdressInput;
    public RawImage CameraRawImage;
    public AudioSource Audio;
    public NftItemView NftItemView;
    public Button TransferButton;

    private float RestartTime;
    private QrCodeData CurrentQrCodeData;
    private IScanner BarcodeScanner;
    private Nft currentNft;
    private Token currentToken;

    private new void Awake()
    {
        TransferButton.onClick.AddListener(OnTransferClicked);
        base.Awake();
    }

    // Disable Screen Rotation on that screen (Not sure if needed)
    void OnEnable()
    {
        // Screen.autorotateToPortrait = false;
        // Screen.autorotateToPortraitUpsideDown = false;
    }

    private void Open(Nft nft)
    {
        currentNft = nft;
        NftItemView.gameObject.SetActive(true);
        NftItemView.SetData(nft, view => { });
    }

    public override void Open(UiService.UiData uiData)
    {
        var transferPopupUiData = (uiData as TransferNftPopupUiData);

        if (transferPopupUiData == null)
        {
            Debug.LogError("Wrong ui data for transfer popup");
            return;
        }
        
        base.Open(uiData);

        if (transferPopupUiData.NftToTransfer != null)
        {
            Open(transferPopupUiData.NftToTransfer);
        }

        StartCoroutine(InitScanner());
    }

    public override void Close()
    {
        StartCoroutine(StopCamera(() => { base.Close(); }));
    }

    private async void OnTransferClicked()
    {
        if (currentNft != null)
        {
            string destinationAddress = AdressInput.text;
            PublicKey address = new PublicKey(destinationAddress);

            var result = await ServiceFactory.Resolve<TransactionService>()
                .TransferNftToPubkey(address, currentNft);
        }

        Close();
    }

    IEnumerator InitScanner()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            CameraRawImage.gameObject.SetActive(true);
            Debug.Log("webcam found");
        }
        else
        {
            CameraRawImage.gameObject.SetActive(false);
            Debug.Log("webcam not found");
        }

        // Create a basic scanner
        BarcodeScanner = new Scanner();
        BarcodeScanner.Camera.Play();

        // Display the camera texture through a RawImage
        BarcodeScanner.OnReady += (sender, arg) =>
        {
            // Set Orientation & Texture
            CameraRawImage.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
            CameraRawImage.transform.localScale = BarcodeScanner.Camera.GetScale();
            CameraRawImage.texture = BarcodeScanner.Camera.Texture;

            // Keep Image Aspect Ratio
            var rect = CameraRawImage.GetComponent<RectTransform>();
            var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);

            RestartTime = Time.realtimeSinceStartup;
        };
    }

    /// <summary>
    /// Start a scan and wait for the callback (wait 1s after a scan success to avoid scanning multiple time the same element)
    /// </summary>
    private void StartScanner()
    {
        BarcodeScanner.Scan((barCodeType, barCodeValue) =>
        {
            BarcodeScanner.Stop();
            if (AdressInput.text.Length > 250)
            {
                AdressInput.text = "";
            }

            Debug.Log("Found: " + barCodeType + " / " + barCodeValue + "\n");
            AdressInput.text = barCodeValue;

            // You can do this if you want to add extra information in the QR code. 
            // I used this one to put a sol amount in for example and then use this to request an amount, like 
            // in solana pay
            //CurrentQrCodeData = JsonUtility.FromJson<QrCodeData>(barCodeValue);
            //Debug.Log($"Current data scanne: {CurrentQrCodeData.SolAdress} {CurrentQrCodeData.SolValue} ");
            //TransferScreen.SetQrCodeData(qrCodeData);

            RestartTime += Time.realtimeSinceStartup + 1f;

            Audio.Play();

#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        });
    }

    /// <summary>
    /// The Update method from unity need to be propagated
    /// </summary>
    void Update()
    {
        if (!Root.gameObject.activeInHierarchy)
        {
            return;
        }

        if (BarcodeScanner != null)
        {
            BarcodeScanner.Update();
        }

        // Check if the Scanner need to be started or restarted
        if (RestartTime != 0 && RestartTime < Time.realtimeSinceStartup)
        {
            StartScanner();
            RestartTime = 0;
        }
    }

    /// <summary>
    /// This coroutine is used because of a bug with unity (http://forum.unity3d.com/threads/closing-scene-with-active-webcamtexture-crashes-on-android-solved.363566/)
    /// Trying to stop the camera in OnDestroy provoke random crash on Android
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator StopCamera(Action callback)
    {
        // Stop Scanning
        BarcodeScanner.Stop();
        BarcodeScanner.Camera.Stop();
        CameraRawImage.texture = null;

        // Wait a bit
        yield return new WaitForSeconds(0.1f);

        callback.Invoke();
    }
}