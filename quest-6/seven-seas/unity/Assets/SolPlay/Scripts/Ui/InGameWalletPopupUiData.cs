using SolPlay.Scripts.Services;

namespace SolPlay.Scripts.Ui
{
    public class InGameWalletPopupUiData : UiService.UiData
    {
        public long RequiredLamports;
        
        public InGameWalletPopupUiData(long requiredLamports)
        {
            RequiredLamports = requiredLamports;
        }
    }
}