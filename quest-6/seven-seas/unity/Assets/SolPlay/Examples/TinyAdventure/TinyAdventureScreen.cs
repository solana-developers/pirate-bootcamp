using System.Collections.Generic;
using Frictionless;
using TinyAdventure.Accounts;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TinyAdventure
{
    public class TinyAdventureScreen : MonoBehaviour
    {
        public Button GetDataButton;
        public Button MoveRightButton;
        public Button MoveLeftButton;
        public Button InitializeButton;

        public TextMeshProUGUI GameText;

        private void Awake()
        {
            GetDataButton.onClick.AddListener(OnGetDataButtonClicked);
            MoveRightButton.onClick.AddListener(OnMoveRightButtonClicked);
            MoveLeftButton.onClick.AddListener(OnMoveLeftButtonClicked);
            InitializeButton.onClick.AddListener(OnInitializeButtonClicked);
        }

        private void Start()
        {
            MessageRouter.AddHandler<TinyAdventureService.GameDataChangedMessage>(OnGameDataChangedMessage);
        }

        private void OnGameDataChangedMessage(TinyAdventureService.GameDataChangedMessage message)
        {
            UpdateGameDataView(message.GameDataAccount);
        }

        private void OnInitializeButtonClicked()
        {
            ServiceFactory.Resolve<TinyAdventureService>().Initialize();
        }

        private void OnMoveLeftButtonClicked()
        {
            ServiceFactory.Resolve<TinyAdventureService>().MoveLeft();
        }

        private void OnMoveRightButtonClicked()
        {
            ServiceFactory.Resolve<TinyAdventureService>().MoveRight();
        }

        private async void OnGetDataButtonClicked()
        {
            GameDataAccount gameData = await ServiceFactory.Resolve<TinyAdventureService>().GetGameData();
            UpdateGameDataView(gameData);
        }

        private void UpdateGameDataView(GameDataAccount gameData)
        {
            switch ((int) gameData.PlayerPosition)
            {
                case 0:
                    GameText.text = "o.........";
                    break;
                case 1:
                    GameText.text = "..o.......";
                    break;
                case 2:
                    GameText.text = ".....o....";
                    break;
                case 3:
                    GameText.text = "........\\o/";
                    break;
            }
        }
    }
}
