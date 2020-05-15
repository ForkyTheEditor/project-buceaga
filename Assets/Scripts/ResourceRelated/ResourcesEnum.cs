using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum ResourceTypes { Energy }

public class ResourcesEnum
{
    /// <summary>
    /// Initializes the given resources list.
    /// Resources lists store the amount of each specific resource.
    /// Note: ALWAYS call this on a new resources list
    /// </summary>
    /// <param name="invStruct">The list given to initialize</param>
    public static void InitializeResourceList(SyncListInt invList)
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
