using TMPro;
using UnityEngine;

namespace SolPlay.Scripts.Ui
{
    /// <summary>
    /// A little animated text on the screen, that disappears after some time. 
    /// </summary>
    public class TextBlimp : MonoBehaviour
    {
        public TextMeshProUGUI Text;

        public void SetData(string text)
        {
            Text.text = text;
        }
    }
}