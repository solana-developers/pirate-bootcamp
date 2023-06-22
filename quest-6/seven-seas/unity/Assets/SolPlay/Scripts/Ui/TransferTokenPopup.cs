using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using AllArt.Solana.Example;
using Frictionless;
using Solana.Unity.Wallet;
using SolPlay.DeeplinksNftExample.Scripts;
using SolPlay.DeeplinksNftExample.Utils;
using SolPlay.Orca;
using SolPlay.Scripts;
using SolPlay.Scripts.Services;
using SolPlay.Scripts.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup with a QrCode Scanner to scan Solana Addresses and to send a SPL Token.
/// </summary>
public class TransferTokenPopup : BasePopup
{
    public TMP_InputField AdressInput;
    public TMP_InputField AmountInput;
    public RawImage CameraRawImage;
    public AudioSource Audio;
    public TokenItemView TokenItemView;
    public Button TransferButton;

    private float RestartTime;
    private QrCodeData CurrentQrCodeData;
    private IScanner BarcodeScanner;
    private SolPlayNft currentNft;
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

    private void Open(Token token)
    {
        currentToken = token;
        TokenItemView.gameObject.SetActive(true);
        TokenItemView.SetData(token, view => { });
    }

    public override void Open(UiService.UiData uiData)
    {
        var transferPopupUiData = (uiData as TransferTokenPopupUiData);

        if (transferPopupUiData == null)
        {
            Debug.LogError("Wrong ui data for transfer popup");
            return;
        }

        base.Open(uiData);

        if (transferPopupUiData.TokenToTransfer != null)
        {
            Open(transferPopupUiData.TokenToTransfer);
        }

        //StartCoroutine(InitScanner());
    }

    public override void Close()
    {
        StartCoroutine(StopCamera(() => { base.Close(); }));
    }

    private async void OnTransferClicked()
    {
        if (!float.TryParse(AmountInput.text, out float amount))
        {
            return;
        }

        if (currentToken != null)
        {
            var transactionService = ServiceFactory.Resolve<TransactionService>();
            Debug.Log($"amount: {amount}");
            LoggingService
                .Log($"Start transfer of {amount} {currentToken.symbol} to {AdressInput.text}", true);

            if (currentToken.mint == OrcaWhirlpoolService.NativeMint)
            {
                var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
                var result = await transactionService.TransferSolanaToPubkey(walletHolderService.BaseWallet,
                    new PublicKey(AdressInput.text),
                     (long) (amount * SolanaUtils.SolToLamports));
            }
            else
            {
                // Transfer one token to to another wallet. USDC has 6 decimals to we need to send 1000000 for 1 USDC. Depends on the token
                var pow = Mathf.Pow(10, currentToken.decimals);
                var convertedToTokenAmount = (ulong) (amount * pow);
                var result = await transactionService.TransferTokenToPubkey(
                    new PublicKey(AdressInput.text),
                    new PublicKey(currentToken.mint), convertedToTokenAmount);
            }
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
        BarcodeScanner.OnReady += (sender, arg) => { StartCoroutine(InitCameraImageNextFrame()); };
    }

    private IEnumerator InitCameraImageNextFrame()
    {
        yield return null;
        // Set Orientation & Texture
        CameraRawImage.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
        CameraRawImage.transform.localScale = BarcodeScanner.Camera.GetScale();
        CameraRawImage.texture = BarcodeScanner.Camera.Texture;

        // Keep Image Aspect Ratio
        var rect = CameraRawImage.GetComponent<RectTransform>();
        var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);

        RestartTime = Time.realtimeSinceStartup;
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

        if (BarcodeScanner == null)
        {
            return;
        }

        BarcodeScanner.Update();

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
        if (BarcodeScanner != null)
        {
            BarcodeScanner.Stop();
            BarcodeScanner.Camera.Stop();
        }
        CameraRawImage.texture = null;

        // Wait a bit
        yield return new WaitForSeconds(0.1f);

        callback.Invoke();
    }
}