using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerNetworkObject : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    private int temporaryTeamSelect = 0;

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

        //---------<TEMPORARY TEAM SELECT AND MATERIAL SELECT>---------------

        switch (temporaryTeamSelect % 2) {

            case 0:
                {

                    go.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                    go.GetComponent<CharacterStats>().team = Teams.Modernists;

                    temporaryTeamSelect++;
                    break;
                }
            case 1:
                {
                    go.GetComponentInChildren<MeshRenderer>().material.color = Color.cyan;
                    go.GetComponent<CharacterStats>().team = Teams.Traditionalists;

                    temporaryTeamSelect++;
                    break;
                }
        
        }


        //---------</TEMPORARY TEAM SELECT AND MATERIAL SELECT>---------------


        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    
    }
   
}
