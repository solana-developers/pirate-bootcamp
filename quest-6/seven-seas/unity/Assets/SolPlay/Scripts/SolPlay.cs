using Frictionless;
using SolPlay.Scripts.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SolPlay.Scripts
{
    public class SolPlay : MonoBehaviour
    {
        public static SolPlay Instance;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            ServiceFactory.RegisterSingleton(this);
        }

        public void LoadScene(string newSceneName)
        {
            MessageRouter.Reset();
            ServiceFactory.Reset();

            SceneManager.LoadScene(newSceneName);
        }

        public void LoadSceneAsync(string newSceneName)
        {
            MessageRouter.Reset();
            ServiceFactory.Reset();

            SceneManager.LoadSceneAsync(newSceneName);
        }
    }
}