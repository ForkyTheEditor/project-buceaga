using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


/// <summary>
/// Inventory for each type of resources. Does its own syncing and networking, so you can locally interact with it and it will sync itself with the host.
/// </summary>
[RequireComponent(typeof(NetworkIdentity))]
public class ResourceInventory : NetworkBehaviour
{
    //The implementation of the resource inventory
    //Holds the amounts of each resource at the respective index of ResourceTypes
    //ALWAYS INITIALIZE THESE WITH THE VALUES NEEDED
    private SyncList<int> resourceAmounts = new SyncList<int>();
    private List<int> localResourceAmounts = new List<int>();

    private void Awake()
    {
        //Initialize the local list in Awake so it gets initialized on each client.
        ResourcesEnum.InitializeResourceList(localResourceAmounts);
    }

    public override void OnStartServer()
    {
        //This should only be managed by the server
        if (!isServer)
        {
            return;
        }
        
        //Initialize the resource list on the server => it then gets updated to each client
        ResourcesEnum.InitializeResourceList(resourceAmounts);
        

    }

    private void Update()
    {
        if(UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame){
            Debug.LogError("cacat");
        }
    }

    /// <summary>
    /// Add a resource to the inventory
    /// </summary>
    /// <param name="rt">The type of resource</param>
    /// <param name="quantity">The quantity added</param>
    public void AddResource(ResourceTypes rt, int quantity)
    {
        UpdateLocalList();

        //Only take in positive values so no weirdness happens
        if (quantity >= 0)
        {
            localResourceAmounts[(int)rt] += quantity;

            //Don't let it overflow!
            localResourceAmounts[(int)rt] = Mathf.Clamp(localResourceAmounts[(int)rt], 0, int.MaxValue);
        }
        else
        {
            print(this.name + ": Quantity added was negative!");
        }

        CmdUpdateResourceList(localResourceAmounts.ToArray());


    }


    /// <summary>
    /// Takes an amount of the specified resource and returns it
    /// </summary>
    /// <param name="rt">The type of resource</param>
    /// <param name="quantity">The quantity added</param>
    /// <returns></returns>
    public int TakeResource(ResourceTypes rt, int quantity)
    {
        UpdateLocalList();

        int returnValue = 0;

        //Only take in positive values so no weirdness happens
        if (quantity >= 0)
        {
            //Check how much is left in the inventory, only return the maximum possible amount
            if(localResourceAmounts[(int)rt] < quantity)
            {
                //Return the entire amount
                returnValue = localResourceAmounts[(int)rt];
            }
            else
            {
                //Only return the asked for quantity
                returnValue = quantity;
            }

            //Subtract it but only let it go as far as 0
            localResourceAmounts[(int)rt] -= quantity;
            localResourceAmounts[(int)rt] = Mathf.Clamp(localResourceAmounts[(int)rt], 0, int.MaxValue);

        }
        else
        {
            print(this.name + ": Quantity added was negative!");
        }


        CmdUpdateResourceList(localResourceAmounts.ToArray());

        return returnValue;
    }

    /// <summary>
    /// Takes the entirety of the specified resource and returns it
    /// </summary>
    /// <param name="rt">Specified resource</param>
    /// <returns></returns>
    public int TakeEntireResource(ResourceTypes rt)
    {
        UpdateLocalList();

        int returnValue = localResourceAmounts[(int)rt];
        //Reset the specified resource
        localResourceAmounts[(int)rt] = 0;

        CmdUpdateResourceList(localResourceAmounts.ToArray());


        return returnValue;
    }

    

    /// <summary>
    /// Getter for the resource values
    /// </summary>
    /// <param name="rt">The type of resource</param>
    /// <returns></returns>
    public int GetResource(ResourceTypes rt)
    {
        UpdateLocalList();

        return localResourceAmounts[(int)rt];
    }

    [Command(ignoreAuthority = true)]
    private void CmdUpdateResourceList(int[] resources)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            resourceAmounts[i] = resources[i];
        }
    }

    private void UpdateLocalList()
    {
        for (int i = 0; i < resourceAmounts.Count; i++)
        {
            localResourceAmounts[i] = resourceAmounts[i];
        }
    }


}
