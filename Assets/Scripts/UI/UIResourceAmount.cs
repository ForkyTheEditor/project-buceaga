using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIResourceAmount : MonoBehaviour
{
    //TODO: Find how to get the player reference in order to display their inventory
    [SerializeField] private ResourceInventory resourceInv;
    private TextMeshProUGUI resourceText;

    // Start is called before the first frame update
    void Start()
    {
        resourceText = gameObject.GetComponent<TextMeshProUGUI>();
        resourceInv = gameObject.GetComponentInParent<ResourceInventory>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Check for errors
        if (resourceText == null || resourceInv == null)
        {
            return;
        }

        resourceText.text = "";

        //Show all resource types
        foreach(ResourceTypes rt in Enum.GetValues(typeof(ResourceTypes)))
        {
            string resourceName = rt.ToString();
            int playerResourceAmount = resourceInv.GetResource(rt);


            resourceText.text += resourceName + ":" + playerResourceAmount.ToString();

        }


    }
}
