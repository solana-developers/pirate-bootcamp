using UnityEngine;
using UnityEngine.Networking;

namespace SolPlay.DeeplinksNftExample.Scripts
{
    public class PhantomUtils
    {
        public static void OpenUrlInWalletBrowser(string url)
        {
#if UNITY_IOS || UNITY_ANROID
            string refUrl = UnityWebRequest.EscapeURL("SolPlay");
            string escapedUrl = UnityWebRequest.EscapeURL(url);
            string inWalletUrl = $"https://phantom.app/ul/browse/{url}?ref=solplay";
#else
            string inWalletUrl = url;
#endif
            Application.OpenURL(inWalletUrl);
        }
    }
}