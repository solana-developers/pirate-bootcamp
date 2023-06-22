using System;
using System.Collections;
using Frictionless;
using SolPlay.DeeplinksNftExample.Utils;
using SolPlay.Scripts.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolPlay.Scripts.Ui
{
    /// <summary>
    /// Shows the sol balance of the connected wallet. Should be updated at certain points, after transactions for example.
    /// </summary>
    public class SolBalanceWidget : MonoBehaviour
    {
        public TextMeshProUGUI SolBalance;
        public TextMeshProUGUI SolChangeText;
        public TextMeshProUGUI PublicKey;
        public Button CopyAddressButton;
        public bool InGameWallet = false;

        private double lamportsChange;
        private Coroutine disableSolChangeCoroutine;

        private void Awake()
        {
            if (CopyAddressButton)
            {
                CopyAddressButton.onClick.AddListener(OnCopyClicked);
            }
        }

        private void OnCopyClicked()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();

            GUIUtility.systemCopyBuffer = InGameWallet ? walletHolderService.InGameWallet.Account.PublicKey : walletHolderService.BaseWallet.Account.PublicKey;;
            
        }

        private void OnEnable()
        {
            MessageRouter.AddHandler<SolBalanceChangedMessage>(OnSolBalanceChangedMessage);
            UpdateContent();
        }

        private void OnDisable()
        {
            MessageRouter.RemoveHandler<SolBalanceChangedMessage>(OnSolBalanceChangedMessage);
        }

        private void UpdateContent()
        {
            SolBalance.text = (GetLamports() / SolanaUtils.SolToLamports).ToString("F2") + " sol";
            if (PublicKey != null)
            {
                PublicKey.text = GetPubkey();
            }
        }

        private double GetLamports()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            if (walletHolderService == null)
            {
                return 0;
            }

            return InGameWallet ? walletHolderService.InGameWalletSolBalance : walletHolderService.BaseWalletSolBalance;
        }

        private string GetPubkey()
        {
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            if (walletHolderService == null)
            {
                return "";
            }

            return InGameWallet
                ? walletHolderService.InGameWallet.Account.PublicKey.Key.Substring(0, 5) + "..."
                : walletHolderService.BaseWallet.Account.PublicKey.Key.Substring(0, 5) + "...";
        }

        private void OnSolBalanceChangedMessage(SolBalanceChangedMessage message)
        {
            UpdateContent();
            if (message.IsInGameWallet != InGameWallet)
            {
                return;
            }

            if (message.SolBalanceChange != 0 && Math.Abs(GetLamports() - message.SolBalanceChange) > 0.00000001)
            {
                lamportsChange += message.SolBalanceChange;
                if (message.SolBalanceChange > 0)
                {
                    if (disableSolChangeCoroutine != null)
                    {
                        StopCoroutine(disableSolChangeCoroutine);
                    }

                    SolChangeText.text = "<color=green>+" + lamportsChange.ToString("F2") + "</color> ";
                    disableSolChangeCoroutine = StartCoroutine(DisableSolChangeDelayed());
                }
                else
                {
                    if (message.SolBalanceChange < -0.0001)
                    {
                        if (disableSolChangeCoroutine != null)
                        {
                            StopCoroutine(disableSolChangeCoroutine);
                        }

                        SolChangeText.text = "<color=red>" + lamportsChange.ToString("F2") + "</color> ";
                        disableSolChangeCoroutine = StartCoroutine(DisableSolChangeDelayed());
                    }
                }
            }
            else
            {
                UpdateContent();
            }
        }

        private IEnumerator DisableSolChangeDelayed()
        {
            SolChangeText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            lamportsChange = 0;
            SolChangeText.gameObject.SetActive(false);
            disableSolChangeCoroutine = null;
        }
    }
}