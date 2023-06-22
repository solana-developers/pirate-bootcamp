using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using SolPlay.Scripts.Ui;
using UnityEngine;

namespace SolPlay.Scripts.Services
{
    public class UiService : MonoBehaviour, IMultiSceneSingleton
    {
        [Serializable]
        public class UiRegistration
        {
            public BasePopup PopupPrefab;
            public ScreenType ScreenType;
        }
        
        public enum ScreenType
        {
            OrcaSwapPopup,
            TransferNftPopup,
            TransferTokenPopup,
            InGameWalletPopup,
            NftListPopup
        }

        public class UiData
        {
            
        }
        
        public List<UiRegistration> UiRegistrations = new List<UiRegistration>();
        
        private readonly Dictionary<ScreenType, BasePopup> openPopups = new Dictionary<ScreenType, BasePopup>();

        public void Awake()
        {
            ServiceFactory.RegisterSingleton(this);
        }

        public void OpenPopup(ScreenType screenType, UiData uiData)
        {
            if (openPopups.TryGetValue(screenType, out BasePopup basePopup))
            {
                basePopup.Open(uiData);
                return;
            }
            
            foreach (var uiRegistration in UiRegistrations)
            {
                if (uiRegistration.ScreenType == screenType)
                {
                    BasePopup newPopup = Instantiate(uiRegistration.PopupPrefab);
                    openPopups.Add(screenType, newPopup);
                    newPopup.Open(uiData);
                }
            }
        }

        public IEnumerator HandleNewSceneLoaded()
        {
            openPopups.Clear();
            yield return null;
        }
    }
}