using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerNetworkObject : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

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

        GameObject go = Instantiate(playerPrefab);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    
    }
   
}
