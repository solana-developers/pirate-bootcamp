using Frictionless;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;


namespace SolPlay.Scripts.Ui
{
    public class TabBarComponent : MonoBehaviour
    {
        public HorizontalScrollSnap HorizontalScrollSnap;

        private void Awake()
        {
            ServiceFactory.RegisterSingleton(this);
            int counter = 0;
            foreach (var toggle in GetComponentsInChildren<Button>())
            {
                var counter1 = counter;
                toggle.onClick.AddListener(delegate { HorizontalScrollSnap.ChangePage(counter1); });
                counter++;
            }
        }
    }
}