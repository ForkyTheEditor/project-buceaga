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
    //The team of the player
    //This is relevant because perhaps you do not want to display the enemy's inventories, even though the player clicks on them
    private Teams playerTeam;

    private ResourceInventory playerResourceInv;
    [SerializeField] private TextMeshProUGUI playerEnergyResourceText;
    private ResourceInventory teamResourceInv;
    [SerializeField] private TextMeshProUGUI teamEnergyResourceText;



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
        playerResourceInv = playerStats.GetComponent<ResourceInventory>();
        //Get the team of the player
        playerTeam = playerStats.team;
        //Check if the team is neutral (no resource manager for the neutral team) 
        if( playerTeam != Teams.Neutral)
        {
            //Get the relevant resource manager's inventory (the team's resource inventory)
            teamResourceInv = GameManager.GetResourceManager(playerTeam).GetComponent<ResourceInventory>();

        }
    } 

    // Update is called once per frame
    void LateUpdate()
    {
        //Check for errors
        if (playerEnergyResourceText == null || playerResourceInv == null || teamResourceInv == null)
        {
            return;
        }

        //Load the energy resource
        playerEnergyResourceText.text = playerResourceInv.GetResource(ResourceTypes.Energy).ToString();
        teamEnergyResourceText.text = teamResourceInv.GetResource(ResourceTypes.Energy).ToString();

    }
}
