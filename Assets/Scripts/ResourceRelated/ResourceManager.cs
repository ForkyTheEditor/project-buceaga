using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

[RequireComponent(typeof(ResourceInventory))]
[RequireComponent(typeof(NetworkIdentity))]
public class ResourceManager : NetworkBehaviour
{
    [SerializeField]
    //The team this object belongs to 
    private Teams team;
    
    ResourceInventory resourceInventory;

    private void Awake()
    {
        resourceInventory = gameObject.GetComponent<ResourceInventory>();
   
    }
    private void Start()
    {
        //Set this manager as the team's resource manager in the GameManager
        GameManager.SetResourceManager(team, this.gameObject);
    }

    //This checks if the players are within range to put the resources in the base
    private void OnTriggerEnter(Collider other)
    {
        //Check for server (only the server runs this script)
        if (!isServer)
        {
            return;
        }
        //Check if player at all & if the player is on this team
        if (other.tag == "Player" && other.GetComponent<CharacterStats>().team == this.team)
        {

            ResourceInventory playerInventory = other.GetComponent<ResourceInventory>();
            //Check for loading error
            if(playerInventory == null)
            {
                print(this.name + ": Player inventory failed to load!");
                return;
            }

            //Take all the resources from their inventory
            foreach(ResourceTypes rt in Enum.GetValues(typeof(ResourceTypes)))
            {
                this.resourceInventory.AddResource(rt, playerInventory.TakeEntireResource(rt));
            }

        }


    }
}
