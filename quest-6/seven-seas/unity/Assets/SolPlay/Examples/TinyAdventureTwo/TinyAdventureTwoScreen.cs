using Frictionless;
using TinyAdventureTwo.Accounts;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TinyAdventure
{
    public class TinyAdventureTwoScreen : MonoBehaviour
    {
        public Button GetDataButton;
        public Button MoveRightButton;
        public Button ResetLevelAndSpawnChestButton;
        public TMP_InputField PasswordInput;
        public Button InitializeButton;

        public TextMeshProUGUI GameText;
        public TextMeshProUGUI WrongPasswordText;

        private void Awake()
        {
            GetDataButton.onClick.AddListener(OnGetDataButtonClicked);
            MoveRightButton.onClick.AddListener(OnMoveRightButtonClicked);
            ResetLevelAndSpawnChestButton.onClick.AddListener(OnResetLevelAndSpawnChestButtonClicked);
            InitializeButton.onClick.AddListener(OnInitializeButtonClicked);
        }

        private void Start()
        {
            MessageRouter.AddHandler<TinyAdventureTwoService.GameDataChangedMessage>(OnGameDataChangedMessage);
        }

        private void OnGameDataChangedMessage(TinyAdventureTwoService.GameDataChangedMessage message)
        {
            UpdateGameDataView(message.GameDataAccount);
        }

        private void OnInitializeButtonClicked()
        {
            ServiceFactory.Resolve<TinyAdventureTwoService>().Initialize();
        }

        private void OnMoveRightButtonClicked()
        {
            WrongPasswordText.gameObject.SetActive(false);

            ServiceFactory.Resolve<TinyAdventureTwoService>().MoveRight(PasswordInput.text, () =>
            {
                WrongPasswordText.gameObject.SetActive(true);
            });
        }

        private void OnResetLevelAndSpawnChestButtonClicked()
        {
            ServiceFactory.Resolve<TinyAdventureTwoService>().ResetLevelAndSpawnChest();
        }
        
        private async void OnGetDataButtonClicked()
        {
            GameDataAccount gameData = await ServiceFactory.Resolve<TinyAdventureTwoService>().GetGameData();
            UpdateGameDataView(gameData);
        }

        private void UpdateGameDataView(GameDataAccount gameData)
        {
            WrongPasswordText.gameObject.SetActive(false);

            switch ((int) gameData.PlayerPosition)
            {
                case 0:
                    GameText.text = "o.........💎";
                    break;
                case 1:
                    GameText.text = "..o.......💎";
                    break;
                case 2:
                    GameText.text = ".....o....💎";
                    break;
                case 3:
                    GameText.text = "........\\o/💎";
                    break;
            }
        }
    }
}
