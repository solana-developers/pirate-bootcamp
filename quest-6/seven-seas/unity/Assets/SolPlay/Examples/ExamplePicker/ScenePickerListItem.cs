using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenePickerListItem : MonoBehaviour
{
    public Button OpenSceneButton;
    public TextMeshProUGUI Headline;
    public TextMeshProUGUI Description;
    public Image PreviewImage;

    private ScenePicker.SceneListItemVO _sceneListItemVo;
    private Action<ScenePicker.SceneListItemVO> onClick;
    
    private void Awake()
    {
        OpenSceneButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        onClick?.Invoke(_sceneListItemVo);
    }

    public void Init(ScenePicker.SceneListItemVO sceneListItemVo, Action<ScenePicker.SceneListItemVO> onClick)
    {
        _sceneListItemVo = sceneListItemVo;
        this.onClick = onClick;
        Headline.text = sceneListItemVo.Headline;
        Description.text = sceneListItemVo.Description;
        PreviewImage.sprite = sceneListItemVo.Preview;
    }
}
