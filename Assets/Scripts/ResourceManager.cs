using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField]
    //The team this object belongs to 
    private Teams team;

    //Dictionary containing the resource amount for each resource
    //Key: ResourceTypes: The type of resource
    //Value: int: The amount of the specific resource
    private Dictionary<ResourceTypes, int> resourceAmounts = new Dictionary<ResourceTypes, int>();

    private void Start()
    {
        //Initialize the dictionary with 0 for each resource type
        foreach(ResourceTypes rt in Enum.GetValues(typeof(ResourceTypes)))
        {
            resourceAmounts[rt] = 0;
        }
    }

}
