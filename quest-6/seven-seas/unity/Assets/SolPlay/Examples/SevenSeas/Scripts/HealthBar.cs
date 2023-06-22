using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public TextMeshProUGUI HealthText;
    public Image GreenHealth;

    public void SetHealth(ulong health, ulong maxHealth)
    {
        GreenHealth.transform.localScale = new Vector3((float) health / maxHealth, 1, 1);
        HealthText.text = health.ToString();
    } 
}
