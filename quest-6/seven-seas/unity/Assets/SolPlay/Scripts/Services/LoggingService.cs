using Frictionless;
using SolPlay.Scripts.Ui;
using UnityEngine;

namespace SolPlay.Scripts.Services
{
    public class LoggingService
    {
        public static void Log(string message, bool showBlimpOnScreen)
        {
            Debug.Log(message);
            if (showBlimpOnScreen)
            {
                MessageRouter.RaiseMessage(new BlimpSystem.ShowLogMessage(message));
            }
        }

        public static void LogWarning(string message, bool showBlimpOnScreen)
        {
            Debug.LogWarning(message);
            if (showBlimpOnScreen)
            {
                MessageRouter.RaiseMessage(new BlimpSystem.ShowLogMessage(message));
            }
        }
    }
}