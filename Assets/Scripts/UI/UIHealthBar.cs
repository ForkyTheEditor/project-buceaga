using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHealthBar : MonoBehaviour
{

    private CharacterStatsComponent characterStats;
    private TextMeshProUGUI hpText;

    // Start is called before the first frame update
    void Start()
    {
        hpText = gameObject.GetComponent<TextMeshProUGUI>();
        characterStats = gameObject.GetComponentInParent<CharacterStatsComponent>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Check for errors
        if(hpText == null || characterStats == null)
        {
            return;
        }

        int truncatedCurrentHealth = characterStats.currentHealth.GetFinalValue();
        int truncatedMaxHealth = characterStats.maxHealth.GetFinalValue();
        hpText.text = truncatedCurrentHealth.ToString() + "/" + truncatedMaxHealth.ToString();
    }
}
