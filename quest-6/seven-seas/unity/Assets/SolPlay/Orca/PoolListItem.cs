using System;
using SolPlay.Orca.OrcaWhirlPool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PoolListItem : MonoBehaviour
{
    public PoolData PoolData;
    public Button SwapButton;
    public Image IconA;
    public Image IconB;
    public TextMeshProUGUI TokenSymbolA; 
    public TextMeshProUGUI TokenSymbolB;

    private Action<PoolListItem> onSwapAction;
    
    private void Awake()
    {
        SwapButton.onClick.AddListener(OnSwapButtonClicked);
    }

    public void SetData(PoolData poolData, Action<PoolListItem> onSwapClick)
    {
       PoolData = poolData;
       TokenSymbolA.text = poolData.SymbolA;
       TokenSymbolB.text = poolData.SymbolB;
       IconA.sprite = poolData.SpriteA;
       IconB.sprite = poolData.SpriteB;
       onSwapAction = onSwapClick;
    }

    private void OnSwapButtonClicked()
    {
        onSwapAction?.Invoke(this);
    }
}
