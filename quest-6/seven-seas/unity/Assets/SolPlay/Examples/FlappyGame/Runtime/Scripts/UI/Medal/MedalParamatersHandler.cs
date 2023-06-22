using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.UI.Medal
{
    public class MedalParamatersHandler : MonoBehaviour
    {
        [SerializeField] MedalParameters _medalHandler;

        public Medal GetMedalWithScore(int score) 
        {
            // Note: List is sorted from the greater to the lower
            for(int i = 0; i < _medalHandler.Medals.Count; i++)
            {
                if(score >= _medalHandler.Medals[i].MinPointsRequired)
                {
                    return _medalHandler.Medals[i];
                }
            }

            return null;
        }
    }
}
