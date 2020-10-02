using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using System;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerNetworkObject : NetworkBehaviour
{

    [SerializeField] private GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //Check if this client has authority over this newly spawned PlayerNetworkObject
        if (!isLocalPlayer) 
        {
            //It does not have authority. Get the hell outta here
            return;
        }

        //Set the local instance of the network object to this
        GameManager.SetLocalPlayerNetworkObject(this.gameObject);

        //Error check
        if(GameManager.localPlayerNetworkInstance == null)
        {
            Debug.LogError("Player Network Object couldn't be set!");
        }

        //Spawn the ACTUAL PlayerObject on the server
        CmdSpawnPlayer();
        
    }


    //This command (a function that runs on the server) spawns the actual physical player object
    [Command]
    void CmdSpawnPlayer() 
    {
        //First instantiate the prefab, do any modifications/settings to it and then SPAWN it on the server
        GameObject go = Instantiate(playerPrefab);

        //----TEMPORARY-----
        go.GetComponent<CharacterStats>().team = Teams.Modernists;
        //----/TEMPORARY-----

        NetworkServer.Spawn(go, connectionToClient);
    
    }

    /// <summary>
    /// Spawns the registered prefab at the given location and with the given rotation.
    /// </summary>
    /// <param name="registeredPrefab"></param>
    public void SpawnObjectNoAuthority(int prefabID, Vector3 newPosition, Quaternion newRotation)
    {
        CmdSpawnObjectNoAuthority(prefabID, newPosition, newRotation);
    }

    [Command]
    void CmdSpawnObjectNoAuthority(int prefabID, Vector3 newPosition, Quaternion newRotation)
    {
        GameObject go = Instantiate(GameManager.spawnIDMap.GetPrefab(prefabID));
        
        //Use the correct Transform
        go.transform.position = newPosition;
        go.transform.rotation = newRotation;
        //Spawn the object without authority
        NetworkServer.Spawn(go);

        
    }
   
}
