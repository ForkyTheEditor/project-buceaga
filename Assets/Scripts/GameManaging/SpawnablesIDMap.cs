using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for storing a Look-Up Table of all the spawnable entities in the game.
/// In order to edit the look-up table you need to edit the spawnables array inside this class' instance on the GameManager.
/// </summary>
public class SpawnablesIDMap : MonoBehaviour
{

    private TwoWayMappingDictionary<int, GameObject> spawnablesIDMapper = new TwoWayMappingDictionary<int, GameObject>();
    [SerializeField] private GameObject[] spawnables;

    private void Awake()
    {
        //Populate the dictionary with the gameobjects
        for(int i =0; i< spawnables.Length; i++)
        {
            spawnablesIDMapper.Add(i, spawnables[i]);
        }
    }

    /// <summary>
    /// Returns the ID of the given prefab.
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public int GetID(GameObject go)
    {
        return spawnablesIDMapper[go]; 
    }

    /// <summary>
    /// Returns the prefab with the given ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GameObject GetPrefab(int id)
    {
        return spawnablesIDMapper[id];
    }

}
