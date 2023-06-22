using TMPro;
using UnityEngine;

namespace SolPlay.Scripts.Ui
{
    /// <summary>
    /// A little animated text on the screen, that disappears after some time. 
    /// </summary>
    public class TextBlimp3D : MonoBehaviour
    {
        public TextMeshPro Text;

        public void SetData(string text)
        {
            Text.text = text;
        }
    }
}