using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class ResourceInventory : NetworkBehaviour
{
    //The implementation of the resource inventory
    //Holds the amounts of each resource at the respective index of ResourceTypes
    //ALWAYS INITIALIZE THESE WITH THE VALUES NEEDED
    private SyncListInt resourceAmounts = new SyncListInt();

    private void Start()
    {
        //This should only be managed by the server
        if (!isServer)
        {
            return;
        }

        //Initialize the resource list
        ResourcesEnum.InitializeResourceList(resourceAmounts);

    }

    /// <summary>
    /// Add a resource to the inventory
    /// </summary>
    /// <param name="rt">The type of resource</param>
    /// <param name="quantity">The quantity added</param>
    public void AddResource(ResourceTypes rt, int quantity)
    {
        //Only take in positive values so no weirdness happens
        if (quantity >= 0)
        {
            resourceAmounts[(int)rt] += quantity;

            //Don't let it overflow!
            resourceAmounts[(int)rt] = Mathf.Clamp(resourceAmounts[(int)rt], 0, int.MaxValue);
        }
        else
        {
            print(this.name + ": Quantity added was negative!");
        }
    }

    /// <summary>
    /// Takes an amount of the specified resource and returns it
    /// </summary>
    /// <param name="rt">The type of resource</param>
    /// <param name="quantity">The quantity added</param>
    /// <returns></returns>
    public int TakeResource(ResourceTypes rt, int quantity)
    {
        int returnValue = 0;

        //Only take in positive values so no weirdness happens
        if (quantity >= 0)
        {
            //Check how much is left in the inventory, only return the maximum possible amount
            if(resourceAmounts[(int)rt] < quantity)
            {
                //Return the entire amount
                returnValue = resourceAmounts[(int)rt];
            }
            else
            {
                //Only return the asked for quantity
                returnValue = quantity;
            }

            //Subtract it but only let it go as far as 0
            resourceAmounts[(int)rt] -= quantity;
            resourceAmounts[(int)rt] = Mathf.Clamp(resourceAmounts[(int)rt], 0, int.MaxValue);

        }
        else
        {
            print(this.name + ": Quantity added was negative!");
        }

        return returnValue;
    }

    /// <summary>
    /// Takes the entirety of the specified resource and returns it
    /// </summary>
    /// <param name="rt">Specified resource</param>
    /// <returns></returns>
    public int TakeEntireResource(ResourceTypes rt)
    {
        int returnValue = resourceAmounts[(int)rt];
        //Reset the specified resource
        resourceAmounts[(int)rt] = 0;

        return returnValue;
    }

    

    /// <summary>
    /// Getter for the resource values
    /// </summary>
    /// <param name="rt">The type of resource</param>
    /// <returns></returns>
    public int GetResource(ResourceTypes rt)
    {
        return resourceAmounts[(int)rt];
    }


}
