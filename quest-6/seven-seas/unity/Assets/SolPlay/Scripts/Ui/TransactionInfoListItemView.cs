using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frictionless;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Messages;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using SolPlay.Scripts.Services;
using SolPlay.Scripts.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TransactionInfoListItemView : MonoBehaviour
{
    public TextMeshProUGUI TransactionName;
    public TextMeshProUGUI Confirmations;

    public GameObject SuccessRoot;
    public GameObject FailRoot;
    public GameObject RunnigRoot;
    public GameObject InitializingRoot;

    private ulong? confirmations;
    private TransactionInfoSystem.TransactionInfoObject transactionInfoObject;
    int retryCounter = 0;
    const int maxTries = 30;

    private enum TransactionStatus
    {
        Running,
        Failed,
        Success,
        Initializing
    }

    public void SetData(TransactionInfoSystem.TransactionInfoObject transactionInfoObject)
    {
        this.transactionInfoObject = transactionInfoObject;
        TransactionName.text = transactionInfoObject.TransactionName;
        UpdateContent(TransactionStatus.Initializing);
        transactionInfoObject.OnError = error =>
        {
            UpdateContent(TransactionStatus.Failed);
            Confirmations.text = error;
            StartCoroutine(DestroyDelayed());
        };
        if (!string.IsNullOrEmpty(transactionInfoObject.Signature))
        {
            StartCoroutine(ConfirmTransaction(transactionInfoObject));     
        }
        else
        {
            transactionInfoObject.OnSignatureReady = s =>
            {
                transactionInfoObject.Signature = s;
                StartCoroutine(ConfirmTransaction(transactionInfoObject));
            };
        }
    }

    private IEnumerator ConfirmTransaction(TransactionInfoSystem.TransactionInfoObject transactionInfoObject)
    {
        bool transactionFinalized = false;

        UpdateContent(TransactionStatus.Running);

        while (!transactionFinalized && retryCounter < maxTries)
        {
            retryCounter++;
            Task<RequestResult<ResponseValue<List<SignatureStatusInfo>>>> task =
                transactionInfoObject.Wallet.ActiveRpcClient.GetSignatureStatusesAsync(new List<string>() {transactionInfoObject.Signature},
                    true);

            yield return new WaitUntil(() => task.IsCompleted);
            
            RequestResult<ResponseValue<List<SignatureStatusInfo>>> signatureResult = task.Result;

            if (signatureResult.Result == null)
            {
                UpdateContent(TransactionStatus.Failed);
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            foreach (var signatureStatusInfo in signatureResult.Result.Value)
            {
                if (signatureStatusInfo == null)
                {
                    UpdateContent(TransactionStatus.Running);
                }
                else
                {
                    confirmations = signatureStatusInfo.Confirmations;
                    if (signatureStatusInfo.ConfirmationStatus ==
                        Enum.GetName(typeof(TransactionService.TransactionResult), transactionInfoObject.Commitment) ||
                        signatureStatusInfo.ConfirmationStatus == Enum.GetName(
                            typeof(TransactionService.TransactionResult),
                            TransactionService.TransactionResult.finalized))
                    {
                        MessageRouter.RaiseMessage(new TokenValueChangedMessage());
                        UpdateContent(TransactionStatus.Success);
                        transactionFinalized = true;
                    }
                    else
                    {
                        UpdateContent(TransactionStatus.Running);
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);

            if (retryCounter >= maxTries)
            {
                UpdateContent(TransactionStatus.Failed);
                StartCoroutine(DestroyDelayed());
            }
        }
    }

    private void UpdateContent(TransactionStatus transactionStatus)
    {
        FailRoot.gameObject.SetActive(transactionStatus == TransactionStatus.Failed);
        SuccessRoot.gameObject.SetActive(transactionStatus == TransactionStatus.Success);
        RunnigRoot.gameObject.SetActive(transactionStatus == TransactionStatus.Running);
        InitializingRoot.gameObject.SetActive(transactionStatus == TransactionStatus.Initializing);
        if (transactionInfoObject.Commitment == Commitment.Confirmed)
        {
            Confirmations.text = $"{confirmations}/1";
        }
        else
        {
            Confirmations.text = $"{confirmations}/31";
        }

        if (transactionStatus == TransactionStatus.Success)
        {
            StartCoroutine(DestroyDelayed());
        }
        else if (transactionStatus == TransactionStatus.Failed)
        {
            Confirmations.text = $"<color=red>{retryCounter}/{maxTries}</color>";
        } else if (transactionStatus == TransactionStatus.Running && retryCounter > 0)
        {
            Confirmations.text = $"<color=red>{retryCounter}/{maxTries}</color>";
        }
    }

    private IEnumerator DestroyDelayed()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}