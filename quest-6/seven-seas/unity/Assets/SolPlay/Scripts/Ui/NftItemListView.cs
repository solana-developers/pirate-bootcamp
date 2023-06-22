using System;
using System.Collections.Generic;
using Frictionless;
using Solana.Unity.SDK.Nft;
using SolPlay.Scripts.Services;
using UnityEngine;

namespace SolPlay.Scripts.Ui
{
    public class NftItemListView : MonoBehaviour
    {
        public GameObject ItemRoot;
        public NftItemView itemPrefab;
        public string FilterSymbol;
        public string BlackList;

        private List<NftItemView> allNftItemViews = new List<NftItemView>();
        private Action<Nft> onNftSelected;

        public void OnEnable()
        {
            UpdateContent();
        }

        public void Start()
        {
            MessageRouter.AddHandler<NftSelectedMessage>(OnNFtSelectedMessage);
            MessageRouter.AddHandler<NewHighScoreLoadedMessage>(OnHighscoreLoadedMessage);
        }

        public void SetData(Action<Nft> onNftSelected)
        {
            this.onNftSelected = onNftSelected;
        }

        private void OnHighscoreLoadedMessage(NewHighScoreLoadedMessage message)
        {
            foreach (var itemView in allNftItemViews)
            {
                if (itemView.CurrentSolPlayNft.metaplexData.data.mint == message.HighscoreEntry.Seed)
                {
                    itemView.PowerLevel.text = $"Score: {message.HighscoreEntry.Highscore}";
                }
            }
        }

        private void OnNFtSelectedMessage(NftSelectedMessage message)
        {
            UpdateContent();
        }

        public void UpdateContent()
        {
            var nftService = ServiceFactory.Resolve<NftService>();
            if (nftService == null)
            {
                return;
            }

            foreach (Nft nft in nftService.MetaPlexNFts)
            {
                AddNFt(nft);
            }

            List<NftItemView> notExistingNfts = new List<NftItemView>();
            foreach (NftItemView nftItemView in allNftItemViews)
            {
                bool existsInWallet = false;
                foreach (Nft walletNft in nftService.MetaPlexNFts)
                {
                    if (nftItemView.CurrentSolPlayNft.metaplexData.data.mint == walletNft.metaplexData.data.mint)
                    {
                        existsInWallet = true;
                        break;
                    }
                }

                if (!existsInWallet)
                {
                    notExistingNfts.Add(nftItemView);
                }
            }

            for (var index = notExistingNfts.Count - 1; index >= 0; index--)
            {
                var nftView = notExistingNfts[index];
                allNftItemViews.Remove(nftView);
                Destroy(nftView.gameObject);
            }
        }

        public void AddNFt(Nft newSolPlayNft)
        {
            foreach (var nft in allNftItemViews)
            {
                if (nft.CurrentSolPlayNft.metaplexData.data.mint == newSolPlayNft.metaplexData.data.mint)
                {
                    // already exists
                    return;
                }
            }

            InstantiateListNftItem(newSolPlayNft);
        }

        private void InstantiateListNftItem(Nft solPlayNft)
        {
            if (string.IsNullOrEmpty(solPlayNft.metaplexData.data.mint))
            {
                return;
            }

            if (!string.IsNullOrEmpty(FilterSymbol) && solPlayNft.metaplexData.data.offchainData.symbol != FilterSymbol)
            {
                return;
            }

            if (!string.IsNullOrEmpty(BlackList) && solPlayNft.metaplexData.data.offchainData.symbol == BlackList)
            {
                return;
            }

            NftItemView nftItemView = Instantiate(itemPrefab, ItemRoot.transform);
            nftItemView.SetData(solPlayNft, OnItemClicked);
            allNftItemViews.Add(nftItemView);
        }

        private void OnItemClicked(NftItemView itemView)
        {
            //Debug.Log("Item Clicked: " + itemView.CurrentSolPlayNft.metaplexData.data.offchainData.name);
            ServiceFactory.Resolve<NftContextMenu>().Open(itemView, onNftSelected);
        }
    }
}