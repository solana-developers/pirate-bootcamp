using System;
using System.Numerics;
using Frictionless;
using SolPlay.DeeplinksNftExample.Scripts;
using SolPlay.Orca.OrcaWhirlPool;
using SolPlay.Scripts.Services;
using SolPlay.Scripts.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrcaSwapPopup : BasePopup
{
    public TMP_InputField TokenInputA;
    public TMP_InputField TokenInputB;
    public TMP_InputField AmountInput;
    public TextMeshProUGUI TokenSymbolA;
    public TextMeshProUGUI TokenSymbolB;
    public Image IconA;
    public Image IconB;
    public Button SwapButton;
    public Button SwapAAndBButton;
    public float DefaultSwapValue = 0.1f;

    private bool AToB = true;
    private PoolData currentPoolData;

    private void Awake()
    {
        ServiceFactory.RegisterSingleton(this);
        SwapButton.onClick.AddListener(OnSwapButtonClicked);
        SwapAAndBButton.onClick.AddListener(OnSwapAAndBButtonClicked);
        base.Awake();
        TokenInputA.onValueChanged.AddListener(OnInputValueChanged);
        TokenInputA.text = TokenInputB.text = DefaultSwapValue.ToString();
    }

    private void OnInputValueChanged(string newValue)
    {
        if (!float.TryParse(AmountInput.text, out float value))
        {
            LoggingService
                .LogWarning($"Wrong input value {value} {currentPoolData.SymbolA} to {currentPoolData.SymbolB}", true);
            return;
        }

        UpdateContent();
    }

    public void Open(PoolData poolData)
    {
        Open(new UiService.UiData());
        currentPoolData = poolData;
        AToB = true;
        TokenInputA.text = DefaultSwapValue.ToString();
        UpdateContent();
    }

    private void UpdateContent()
    {
        if (currentPoolData == null)
        {
            return;
        }

        if (!float.TryParse(AmountInput.text, out float value))
        {
            LoggingService
                .LogWarning($"Wrong input value {value} {currentPoolData.SymbolA} to {currentPoolData.SymbolB}", true);
            return;
        }

        BigInteger bigInt = currentPoolData.Pool.SqrtPrice;
        var d = Double.Parse(bigInt.ToString());
        var fromX64 = Math.Pow(d * Math.Pow(2, -64), 2);
        var aRatioA = Math.Pow(10,
            currentPoolData.TokenA.decimals -
            currentPoolData.TokenB.decimals);
        var priceBtoA = fromX64 * aRatioA;

        TokenSymbolA.text = AToB ? currentPoolData.SymbolA : currentPoolData.SymbolB;
        TokenSymbolB.text = AToB ? currentPoolData.SymbolB : currentPoolData.SymbolA;
        IconA.sprite = AToB ? currentPoolData.SpriteA : currentPoolData.SpriteB;
        IconB.sprite = AToB ? currentPoolData.SpriteB : currentPoolData.SpriteA;

        var aToB = (value * priceBtoA).ToString("F6");
        var bToA = (value / priceBtoA).ToString("F6");
        Debug.Log($"Price a to b {aToB} b to a: {bToA}");
        TokenInputB.text = AToB ? aToB : bToA;
    }

    private async void OnSwapButtonClicked()
    {
        if (!float.TryParse(AmountInput.text, out float value))
        {
            LoggingService
                .LogWarning($"Wrong input value {value} {currentPoolData.SymbolA} to {currentPoolData.SymbolB}", true);
            return;
        }

        var pow = (ulong) Math.Pow(10,
            AToB
                ? currentPoolData.TokenA.decimals
                : currentPoolData.TokenB.decimals);
        var floorToInt = value * pow;
        ulong valueLong = (ulong) floorToInt;

        string fromToMessage =
            AToB
                ? $"Swapping {value} {currentPoolData.SymbolA} to {currentPoolData.SymbolB}"
                : $"Swapping {value} {currentPoolData.SymbolB} to {currentPoolData.SymbolA}";

        LoggingService.Log(fromToMessage, true);
        var wallet = ServiceFactory.Resolve<WalletHolderService>().BaseWallet;
        var signature = await ServiceFactory.Resolve<OrcaWhirlpoolService>()
            .Swap(wallet, currentPoolData.Pool, valueLong, AToB);
        ServiceFactory.Resolve<TransactionService>().CheckSignatureStatus(signature,
            success => { MessageRouter.RaiseMessage(new TokenValueChangedMessage()); });
    }

    private void OnSwapAAndBButtonClicked()
    {
        AToB = !AToB;
        TokenInputA.text = TokenInputB.text;
        UpdateContent();
    }
}