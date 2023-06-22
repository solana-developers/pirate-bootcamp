using System;
using SolPlay.DeeplinksNftExample.Scripts;
using SolPlay.Orca;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolPlay.Scripts.Ui
{
    /// <summary>
    /// Show the image of a Token. Tokens are currently coming from the orca API so we dont need to always get
    /// Token account info.
    /// </summary>
    public class TokenItemView : MonoBehaviour
    {
        public Token CurrentToken;
        public Image Icon;
        public TextMeshProUGUI Headline;
        public Button Button;

        private Action<TokenItemView> onButtonClickedAction;

        public async void SetData(Token token, Action<TokenItemView> onButtonClicked = null)
        {
            if (token == null)
            {
                return;
            }

            CurrentToken = token;
            Icon.gameObject.SetActive(false);

            if (gameObject.activeInHierarchy)
            {
                Icon.gameObject.SetActive(true);
                Icon.sprite = await OrcaWhirlpoolService.GetTokenIconSprite(token.mint, token.symbol);
            }

            Headline.text = CurrentToken.symbol;
            Button.onClick.AddListener(OnButtonClicked);
            onButtonClickedAction = onButtonClicked;
        }

        private void OnButtonClicked()
        {
            onButtonClickedAction?.Invoke(this);
        }
    }
}