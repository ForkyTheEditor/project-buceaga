using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum ResourceTypes { Axioms }

public class ResourcesEnum
{
    /// <summary>
    /// Initializes the given resources list.
    /// Resources lists store the amount of each specific resource.
    /// Note: ALWAYS call this on a new resources list. ALWAYS call from the server.
    /// </summary>
    /// <param name="invStruct">The list given to initialize</param>
    public static void InitializeResourceList(SyncList<int> invList)
    {
        //Initialize the dictionary with 0 for each resource type
        foreach (ResourceTypes rt in Enum.GetValues(typeof(ResourceTypes)))
        {
            //Add an entry for each resource type
            //The index represents the resource type
            invList.Add(0);
        }

    }

    public static void InitializeResourceList(List<int> invList)
    {
        //Initialize the dictionary with 0 for each resource type
        foreach (ResourceTypes rt in Enum.GetValues(typeof(ResourceTypes)))
        {
            //Add an entry for each resource type
            //The index represents the resource type
            invList.Add(0);
        }

    }
}
