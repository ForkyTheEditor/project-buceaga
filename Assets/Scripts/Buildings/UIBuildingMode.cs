using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuildingMode : MonoBehaviour
{

    private GameObject spawnedGhost = null;

    /// <summary>
    /// Simply instantiates (not spawns) the given building and disables the building canvas. This script is supposed to sit on the canvas with the building UI.
    /// </summary>
    /// <param name="prefab"></param>
    public void InstantiateBuilding(GameObject prefab)
    {
        if(prefab != null)
        {
            //If the player has opened the UI twice, destroy the old object so you don't have two of them
            if(spawnedGhost != null)
            {
                Destroy(spawnedGhost);
            }

            spawnedGhost = Instantiate(prefab, Input.mousePosition, prefab.transform.rotation);

            //Deactivate this canvas
            this.gameObject.SetActive(false);
        }
    }

}
