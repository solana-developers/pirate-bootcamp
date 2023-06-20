using System.Collections;
using DG.Tweening;
using Frictionless;
using Solana.Unity.Rpc.Models;
using SolPlay.FlappyGame.Runtime.Scripts.Audio;
using SolPlay.FlappyGame.Runtime.Scripts.Core;
using SolPlay.FlappyGame.Runtime.Scripts.UI.Medal;
using SolPlay.Scripts.Services;
using SolPlay.Scripts.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolPlay.FlappyGame.Runtime.Scripts.UI.Screens
{
    public class GameOverScreen : MonoBehaviour
    {
        [Header("Elements")] [SerializeField] GameMode _gameMode;
        [SerializeField] GameSaver _gameSaver;
        [SerializeField] MedalHud _medalHud;
        [SerializeField] TextMeshProUGUI _scoreText;
        [SerializeField] TextMeshProUGUI _highScoreText;
        [SerializeField] TextMeshProUGUI _costText;
        [SerializeField] GameObject _newHud;
        [SerializeField] FadeScreen _fadeScreen;
        [SerializeField] XpWidget _xpWidget;
        [SerializeField] Button _submitHighscoreButton;
        [SerializeField] Button _mintMedalButton;
        [SerializeField] NftItemView _nftItemView;
        [SerializeField] Sprite _bronzeMedal;
        [SerializeField] Sprite _silverMedal;
        [SerializeField] Sprite _goldMedal;
        [SerializeField] Sprite _noMedal;
        [SerializeField] Image _medalImage;

        [Header("Containers")] [SerializeField]
        CanvasGroup _gameOverContainer;

        [SerializeField] CanvasGroup _statsContainer;

        [Header("GameOver Tween")] [SerializeField]
        Transform _gameOverReference;

        [SerializeField] float _gameOverAnimationTime = 1f;

        [Header("Stats Tween")] [SerializeField]
        Transform _statsReference;

        [SerializeField] float _statsAnimationDelay = 1f;
        [SerializeField] float _statsAnimationTime = 1f;

        [SerializeField] float _buttonsAnimationDelay = 1f;

        [Header("Audio")] [SerializeField] AudioFX _statsMoveAudio;
        [SerializeField] AudioFX _buttonsMoveAudio;

        private void Awake()
        {
            _submitHighscoreButton.onClick.AddListener(OnSubmitHighscoreButtonClicked);
            _mintMedalButton.onClick.AddListener(OnMintMedalButtonClicked);
        }

        private void Start()
        {
            MessageRouter.AddHandler<NftSelectedMessage>(OnNftSelectedMessage);
            MessageRouter
                .AddHandler<NewHighScoreLoadedMessage>(OnHighscoreLoadedMessage);
        }

        private void OnHighscoreLoadedMessage(NewHighScoreLoadedMessage message)
        {
            var nftService = ServiceFactory.Resolve<NftService>();
            if (nftService.SelectedNft != null)
            {
                _nftItemView.SetData(nftService.SelectedNft, view => { });
            }
        }

        private void OnNftSelectedMessage(NftSelectedMessage message)
        {
            _gameMode.RestartGame();
        }

        private void OnEnable()
        {
            _mintMedalButton.interactable = true;
            UpdateHud();

            if (_fadeScreen != null)
                StartCoroutine(_fadeScreen.Flash());

            StartCoroutine(ShowUICoroutine());
        }

        public void Quit() => _gameMode.QuitGame();
        public void Restart() => _gameMode.RestartGame();

        private IEnumerator ShowUICoroutine()
        {
            _gameOverContainer.alpha = 0;
            _gameOverContainer.blocksRaycasts = false;

            _statsContainer.alpha = 0;
            _statsContainer.blocksRaycasts = false;

            yield return StartCoroutine(
                AnimateCanvasGroup(
                    _gameOverContainer,
                    _gameOverReference.position,
                    _gameOverContainer.transform.position,
                    _gameOverAnimationTime
                ));

            _statsMoveAudio.PlayAudio();

            StartCoroutine(
                AnimateCanvasGroup(
                    _statsContainer,
                    _statsReference.position,
                    _statsContainer.transform.position,
                    _statsAnimationTime
                ));

            yield return new WaitForSeconds(_buttonsAnimationDelay);

            _buttonsMoveAudio.PlayAudio();
        }

        private IEnumerator AnimateCanvasGroup(CanvasGroup group, Vector3 from, Vector3 to, float time)
        {
            group.alpha = 0;
            group.blocksRaycasts = false;

            Tween fade = group.DOFade(1, time);

            group.transform.localScale = Vector3.zero;
            group.transform.DOScale(Vector3.one, 0.5f);

            yield return fade.WaitForKill();

            group.blocksRaycasts = true;
        }

        private async void OnMintMedalButtonClicked()
        {
            var nftMintingService = ServiceFactory.Resolve<NftMintingService>();

            if (_gameMode.Score >= 150)
            {
                LoggingService.Log("Start minting a Gold Medal", true);

                _mintMedalButton.interactable = false;
                await nftMintingService.MintNftWithMetaData(
                    "https://shdw-drive.genesysgo.net/8QHFphU4iT6rFAW93vvzj8f79Txe5auRAgto3SMxxQ8x/manifest.json",
                    $"Gold Medal {_gameMode.Score}", "SolPlay");
            }
            else if (_gameMode.Score >= 100)
            {
                LoggingService.Log("Start minting a Silver Medal", true);

                _mintMedalButton.interactable = false;
                await nftMintingService.MintNftWithMetaData(
                    "https://shdw-drive.genesysgo.net/9JjNpESm1sGJGJRuaGEiju6hG4Z54XRtCEc7jzWDJWdV/manifest.json",
                    $"Silver Medal {_gameMode.Score}", "SolPlay");
            }
            else if (_gameMode.Score >= 50)
            {
                LoggingService.Log("Start minting a Bronze Medal", true);

                _mintMedalButton.interactable = false;
                await nftMintingService.MintNftWithMetaData(
                    "https://shdw-drive.genesysgo.net/9hrRBH5U3Lc5eKDKkcazk5wycizd11jon8M5HpFTsHjG/manifest.json",
                    $"Bronze Medal {_gameMode.Score}", "SolPlay");
            }
            else
            {
                LoggingService.Log("Reach at least 50 Points!", true);
            }
        }

        private async void OnSubmitHighscoreButtonClicked()
        {
            _submitHighscoreButton.interactable = false;

            var smartContractService = ServiceFactory.Resolve<HighscoreService>();
            var nftService = ServiceFactory.Resolve<NftService>();
            var customSmartContractService = smartContractService;
            await customSmartContractService.SafeHighScore(nftService.SelectedNft, (uint) _gameMode.Score);
            AccountInfo account = await smartContractService.GetHighscoreAccountData(nftService.SelectedNft);
        }

        private void UpdateHud()
        {
            var highscoreService = ServiceFactory.Resolve<HighscoreService>();
            var walletHolderService = ServiceFactory.Resolve<WalletHolderService>();
            if (walletHolderService == null || walletHolderService.BaseWallet == null)
            {
                return;
            }
            var nftService = ServiceFactory.Resolve<NftService>();

            int score = _gameMode.Score;
            int curHighScore = 0;
            bool hasHighscoreSaved = highscoreService.TryGetCurrentHighscore(out HighscoreEntry savedHighscore);
  
            if (hasHighscoreSaved)
            {
                curHighScore = (int) savedHighscore.Highscore;
            }

            if (_gameMode.Score >= 150)
            {
                _medalImage.sprite = _goldMedal;
            }
            else if (_gameMode.Score >= 100)
            {
                _medalImage.sprite = _silverMedal;
            }
            else if (_gameMode.Score >= 50)
            {
                _medalImage.sprite = _bronzeMedal;
            }
            else
            {
                _medalImage.sprite = _noMedal;
            }

            if (nftService.SelectedNft != null)
            {
                _nftItemView.gameObject.SetActive(true);
                _nftItemView.SetData(nftService.SelectedNft, view => { });
            }
            else
            {
                _nftItemView.gameObject.SetActive(false);
            }

            var newHighscoreReached = score > curHighScore;
            int newHighScore = newHighscoreReached ? score : curHighScore;

            if (curHighScore == 0)
            {
                _costText.text = "0.00191 sol";
            }
            else
            {
                _costText.text = "0.001 sol";
            }

            _submitHighscoreButton.interactable = newHighscoreReached && hasHighscoreSaved && savedHighscore.AccountLoaded;
#if UNITY_EDITOR
            _submitHighscoreButton.interactable = true;
#endif
            _medalHud.HandleScore(score);
            _scoreText.SetText(score.ToString());
            _highScoreText.SetText(newHighScore == null ? "0" : newHighScore.ToString() );

            _newHud.SetActive(newHighscoreReached);
        }
    }
}