using Frictionless;
using SolPlay.DeeplinksNftExample.Utils;
using SolPlay.Scripts.Services;
using SolPlay.Scripts.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup which lets the player transfer sol to the in game wallet and withdraw it
/// </summary>
public class InGameWalletPopup : BasePopup
{
    public Button DepositButton;
    public Button WithdrawButton;
    public Button AirdropButton;
    public TextMeshProUGUI MissingSolText;

    private void Awake()
    {
        DepositButton.onClick.AddListener(OnDepositButtonClicked);
        WithdrawButton.onClick.AddListener(OnWithdrawButtonClicked);
        AirdropButton.onClick.AddListener(OnAirdropButtonClicked);
        base.Awake();
    }

    private async void OnAirdropButtonClicked()
    {
        AirdropButton.interactable = false;
        await ServiceFactory.Resolve<WalletHolderService>().RequestAirdrop();
        AirdropButton.interactable = true;
    }

    public override void Open(UiService.UiData uiData)
    {
        var inGameWalletPopupUiData = (uiData as InGameWalletPopupUiData);

        if (inGameWalletPopupUiData == null)
        {
            Debug.LogError("Wrong ui data for ingame wallet popup");
            return;
        }

        var isDevNetLogin = ServiceFactory.Resolve<WalletHolderService>().TwoWalletSetup;
        AirdropButton.gameObject.SetActive(true);
        DepositButton.interactable = !isDevNetLogin;
        WithdrawButton.interactable = !isDevNetLogin;
        MissingSolText.gameObject.SetActive(inGameWalletPopupUiData.RequiredLamports > 0);
        MissingSolText.text =
            $"You are missing {inGameWalletPopupUiData.RequiredLamports} lamports ({(inGameWalletPopupUiData.RequiredLamports / (float) SolanaUtils.SolToLamports).ToString("F2")}) sol in your in game wallet.";
        base.Open(uiData);
    }

    private async void OnWithdrawButtonClicked()
    {
        var transactionService = ServiceFactory.Resolve<TransactionService>();
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();

        var result = await transactionService.TransferSolanaToPubkey(walletHolderService.InGameWallet,
            walletHolderService.BaseWallet.Account.PublicKey,
            walletHolderService.InGameWalletSolBalance - 1000000);
    }

    private async void OnDepositButtonClicked()
    {
        var transactionService = ServiceFactory.Resolve<TransactionService>();
        var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();

        var result = await transactionService.TransferSolanaToPubkey(walletHolderService.BaseWallet,
            walletHolderService.InGameWallet.Account.PublicKey,
            SolanaUtils.SolToLamports / 2);
    }
}