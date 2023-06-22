using SolPlay.Orca;
using SolPlay.Scripts.Services;

namespace SolPlay.Scripts.Ui
{
    public class TransferTokenPopupUiData : UiService.UiData
    {
        public Token TokenToTransfer;

        public TransferTokenPopupUiData(Token token)
        {
            TokenToTransfer = token;
        }
    }
}