using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceeneWidget : MonoBehaviour
{
    public Button LoadSceneButton;
    
    void Start()
    {
        LoadSceneButton.onClick.AddListener(OnLoadSceneClicked);
    }

    private void OnLoadSceneClicked()
    {
        SceneManager.LoadScene("SevenSeas");
    }

}
