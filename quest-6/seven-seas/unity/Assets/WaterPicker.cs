using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPicker : MonoBehaviour
{
    public GameObject MobileWater;
    public GameObject WebGLWater;

    void Start()
    {
        if (Application.isMobilePlatform)
        {
            MobileWater.gameObject.SetActive(true);
            WebGLWater.gameObject.SetActive(false);
        }
        else
        {
            MobileWater.gameObject.SetActive(false);
            WebGLWater.gameObject.SetActive(true);
        } 
    }

}
