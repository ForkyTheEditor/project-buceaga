﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlayer : MonoBehaviour
{
    //The stats of the player to use (not necessarily the local player, for instance
    //in the future we might want the player to be able to click on other units and see their inventory
    // like in DOTA for instance)
    private CharacterStats playerStats;

    private ResourceInventory resourceInv;
    [SerializeField] private TextMeshProUGUI resourceText;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize the UI components
        StartCoroutine(InitializeComponents());
    }

    IEnumerator InitializeComponents()
    {
        
        //Wait until the local player instance is set
        yield return new WaitUntil(() => GameManager.localPlayerInstance != null);
        //Get instance of the local player for starters!
        playerStats = GameManager.localPlayerInstance.GetComponent<CharacterStats>();
        if (playerStats == null)
        {
            Debug.LogError("Local player instance couldn't be found!", this);
        }
        resourceInv = playerStats.GetComponent<ResourceInventory>();
        
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
        foreach (ResourceTypes rt in Enum.GetValues(typeof(ResourceTypes)))
        {
            string resourceName = rt.ToString();
            int playerResourceAmount = resourceInv.GetResource(rt);

            resourceText.text += resourceName + ":" + playerResourceAmount.ToString();

        }
    }
}
