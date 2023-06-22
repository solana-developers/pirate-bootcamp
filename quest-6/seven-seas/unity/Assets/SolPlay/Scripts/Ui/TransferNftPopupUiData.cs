using Solana.Unity.SDK.Nft;
using SolPlay.Orca;
using SolPlay.Scripts.Services;

namespace SolPlay.Scripts.Ui
{
    public class TransferNftPopupUiData : UiService.UiData
    {
        public Nft NftToTransfer;
        
        public TransferNftPopupUiData(Nft solPlayNft)
        {
            NftToTransfer = solPlayNft;
        }
    }
}