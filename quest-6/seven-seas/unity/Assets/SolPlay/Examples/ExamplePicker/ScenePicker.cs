using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePicker : MonoBehaviour
{
    [Serializable]
    public class SceneListItemVO
    {
        public string Headline;
        public string Description;
        public string SceneName;
        public Sprite Preview;
    }

    public List<SceneListItemVO> SceneListItemVos;
    public ScenePickerListItem ScenePickerListItemPrefab;
    public GameObject Root;
    
    public void Awake()
    {
        foreach (SceneListItemVO sceneListItemVo in SceneListItemVos)
        {
            var pickerListItem = Instantiate(ScenePickerListItemPrefab, Root.transform);
            pickerListItem.Init(sceneListItemVo, (vo) =>
            {
                SceneManager.LoadScene(vo.SceneName);
            });
        }
    }
}
