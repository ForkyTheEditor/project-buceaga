using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHealthBar : MonoBehaviour
{

    private CharacterStats characterStats;
    private TextMeshProUGUI hpText;

    // Start is called before the first frame update
    void Start()
    {
        hpText = gameObject.GetComponent<TextMeshProUGUI>();
        characterStats = gameObject.GetComponentInParent<CharacterStats>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        int truncatedCurrentHealth = Mathf.FloorToInt(characterStats.currentHealth);
        int truncatedMaxHealth = Mathf.FloorToInt(characterStats.maxHealth);
        hpText.text = truncatedCurrentHealth.ToString() + "/" + truncatedMaxHealth.ToString();
    }
}
