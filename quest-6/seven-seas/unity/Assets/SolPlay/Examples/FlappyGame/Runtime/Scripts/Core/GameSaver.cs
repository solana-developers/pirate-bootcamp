using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Core
{
    public class SaveGameData
    {
        public int HighestScore;
    }

    public class GameSaver : MonoBehaviour
    {
        private const string HighestScoreKey = "HighestScore";

        private SaveGameData _currentSave;
        public SaveGameData CurrentSave 
        { 
            get
            {
                if(_currentSave  == null)
                    LoadGame();

                return _currentSave;
            } 
        }

        public void SaveGame(SaveGameData saveData)
        {
            _currentSave = saveData;
            PlayerPrefs.SetInt(HighestScoreKey, CurrentSave.HighestScore);
            PlayerPrefs.Save();
        }

        public void LoadGame()
        {
            _currentSave = new SaveGameData
            {
                HighestScore = PlayerPrefs.GetInt(HighestScoreKey, 0),
            };
        }
    }
}