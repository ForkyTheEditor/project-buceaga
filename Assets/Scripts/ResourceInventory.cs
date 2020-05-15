using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


//A struct representing the inventory's dictionary 
//We need this because dictionaries cannot be sent over the network, while structs containg PODs can
//Consider it as a tool for serialization
public struct InventoryStruct
{
    //Vector2 representing dictionary entry (key, value)
    //We need one of these for each type of resource
    public Vector2 energy { get { return energy; } set { energy = value; } }
}

public class ResourceInventory : MonoBehaviour
{

    //Dictionary that holds each resource amount
    //ALWAYS INITIALIZE THESE WITH THE VALUES NEEDED
    //CHANGE THE SCRIPT TO WORK WITH THE STRUCT FORGET THE DICTIONARIES
    //PROS: NO MORE INITIALIZING; CAN BE SERIALIZED AND SENT OVER NETWORK;
    private Dictionary<ResourceTypes, int> resourceAmounts = new Dictionary<ResourceTypes, int>();
    
    [SyncVar]
    private InventoryStruct inventoryStruct;

    private void Awake()
    {
        //Initialize the dictionery
        ResourcesEnum.InitializeResourceDict(resourceAmounts);
        resourceAmounts[ResourceTypes.Energy] = 500;
    }

    private void Update()
    {
        print(this.name + " " + resourceAmounts[ResourceTypes.Energy]);
    }

    /// <summary>
    /// Returns a copy of the resource dictionary
    /// </summary>
    /// <returns>Copy of the resources dictionary</returns>
    public Dictionary<ResourceTypes, int> GetDictionary()
    {
        Dictionary<ResourceTypes, int> returnDict = new Dictionary<ResourceTypes, int>(resourceAmounts);
        return returnDict;
    }

    public void SetDictionary(Dictionary<ResourceTypes, int> newDict)
    {
        //Just update each field individually
        foreach (ResourceTypes rt in Enum.GetValues(typeof(ResourceTypes)))
        {
            resourceAmounts[rt] = newDict[rt];
        }
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
            resourceAmounts[rt] += quantity;

            //Don't let it overflow!
            resourceAmounts[rt] = Mathf.Clamp(resourceAmounts[rt], 0, int.MaxValue);
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
            if(resourceAmounts[rt] < quantity)
            {
                //Return the entire amount
                returnValue = resourceAmounts[rt];
            }
            else
            {
                //Only return the asked for quantity
                returnValue = quantity;
            }

            //Subtract it but only let it go as far as 0
            resourceAmounts[rt] -= quantity;
            resourceAmounts[rt] = Mathf.Clamp(resourceAmounts[rt], 0, int.MaxValue);

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
    /// <param name="rt"></param>
    /// <returns></returns>
    public int TakeEntireResource(ResourceTypes rt)
    {
        int returnValue = resourceAmounts[rt];
        //Reset the specified resource
        resourceAmounts[rt] = 0;

        return returnValue;
    }

    /// <summary>
    /// Getter for the resource values
    /// </summary>
    /// <param name="rt">The type of resource</param>
    /// <returns></returns>
    public int GetResource(ResourceTypes rt)
    {
        return resourceAmounts[rt];
    }

    private void UpdateInventoryStruct()
    {



    }

}
