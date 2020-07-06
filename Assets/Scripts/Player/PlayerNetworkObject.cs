using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerNetworkObject : NetworkBehaviour
{

    [SerializeField] private GameObject playerPrefab;


  

    private static int temporaryTeamSelect = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Check if this client has authority over this newly spawned PlayerNetworkObject
        if (!hasAuthority) 
        {
            //It does not have authority. Get the hell outta here
            return;
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

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    
    }
   
}
