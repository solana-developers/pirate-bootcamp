using UnityEngine;
using UnityEngine.UI;

namespace SolPlay.Scripts.Ui
{
    [RequireComponent(typeof(Button))]
    public class LinkButton : MonoBehaviour
    {
        Button button;
        public string Url;
        
        void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            Application.OpenURL(Url);
        }
    }
}