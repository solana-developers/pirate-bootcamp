using Frictionless;
using SolPlay.Scripts.Services;
using TMPro;
using UnityEngine;

namespace SolPlay.Scripts.Ui
{
    /// <summary>
    /// Whenever a new NFT arrives this widget updates the total power level of all Nfts
    /// </summary>
    public class PowerLevelWidget : MonoBehaviour
    {
        public TextMeshProUGUI TotalPowerLevelText;

        void Start()
        {
            MessageRouter.AddHandler<NftLoadedMessage>(OnNftArrived);
        }

        private void OnNftArrived(NftLoadedMessage message)
        {
            var totalPowerLevel = ServiceFactory.Resolve<NftPowerLevelService>().GetTotalPowerLevel();
            TotalPowerLevelText.text = $"{totalPowerLevel}";
        }
    }
}