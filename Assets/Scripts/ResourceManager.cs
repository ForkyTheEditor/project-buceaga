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

    private void Start()
    {
        resourceInventory = gameObject.GetComponent<ResourceInventory>();
   
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

            ResourceInventory enemyInventory = other.GetComponent<ResourceInventory>();
            //Check for loading error
            if(enemyInventory == null)
            {
                print(this.name + ": Enemy inventory failed to load!");
                return;
            }

            //Take all the resources from their inventory
            foreach(ResourceTypes rt in Enum.GetValues(typeof(ResourceTypes)))
            {
                this.resourceInventory.AddResource(rt, enemyInventory.TakeEntireResource(rt));
            }

        }


    }
}
