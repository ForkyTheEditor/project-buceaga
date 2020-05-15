using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceTypes { Energy }

public class ResourcesEnum : MonoBehaviour
{
    /// <summary>
    /// Initializes the given resources dictionary.
    /// Resources dictionaries store the amount of each specific resource.
    /// Note: ALWAYS call this on a new resources dictionary
    /// </summary>
    /// <param name="dict"></param>
    public static void InitializeResourceDict(Dictionary<ResourceTypes, int> dict)
    {
        //Initialize the dictionary with 0 for each resource type
        foreach (ResourceTypes rt in Enum.GetValues(typeof(ResourceTypes)))
        {
            //Add an entry for each resource type
            dict[rt] = 0;
        }

    }
}
