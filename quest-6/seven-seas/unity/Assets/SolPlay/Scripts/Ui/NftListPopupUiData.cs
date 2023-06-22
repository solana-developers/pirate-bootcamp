using Solana.Unity.SDK;
using SolPlay.Scripts.Services;

namespace SolPlay.Scripts.Ui
{
    public class NftListPopupUiData : UiService.UiData
    {
        public bool RequestNfts;
        public WalletBase Wallet;
        
        public NftListPopupUiData(bool requestNfts, WalletBase wallet)
        {
            RequestNfts = requestNfts;
            Wallet = wallet;
        }
    }
}