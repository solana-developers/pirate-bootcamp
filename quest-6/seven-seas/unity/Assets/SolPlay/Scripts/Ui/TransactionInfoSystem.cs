using System;
using Frictionless;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK;
using UnityEngine;

namespace SolPlay.Scripts.Ui
{
    public class TransactionInfoSystem : MonoBehaviour
    {
        public TransactionInfoListItemView TransactionInfoListItemViewPrefab;
        public GameObject Root;

        void Start()
        {
            MessageRouter.AddHandler<ShowTransactionInfoMessage>(OnShowTransactionInfoMessage);
        }

        private void OnShowTransactionInfoMessage(ShowTransactionInfoMessage message)
        {
            SpawnTransactionInfoItemView(message.TransactionInfoObject);
        }

        private void OnDestroy()
        {
            MessageRouter.RemoveHandler<ShowTransactionInfoMessage>(OnShowTransactionInfoMessage);
        }

        private void SpawnTransactionInfoItemView(TransactionInfoObject transactionInfoObject)
        {
            var instance = Instantiate(TransactionInfoListItemViewPrefab, Root.transform);
            instance.SetData(transactionInfoObject);
        }

        public class TransactionInfoObject
        {
            public string TransactionName;
            public string Signature;
            public Action<string> OnSignatureReady;
            public Action<string> OnError;
            public Commitment Commitment;
            public WalletBase Wallet;

            public TransactionInfoObject(WalletBase wallet, Commitment commitment, string transactionName)
            {
                Wallet = wallet;
                Commitment = commitment;
                TransactionName = transactionName;
            }
        }

        public class ShowTransactionInfoMessage
        {
            public TransactionInfoObject TransactionInfoObject;

            public ShowTransactionInfoMessage(TransactionInfoObject transactionInfoObject)
            {
                TransactionInfoObject = transactionInfoObject;
            }
        }
    }
}