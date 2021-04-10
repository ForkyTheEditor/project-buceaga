using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using System;


public class PlayerNetworkObject : NetworkBehaviour
{
    [SyncVar]
    private bool isAlive = false; //Is the player character alive?

    private CharacterStatsDefaultValues playerCharacterStats = null;

    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private GameObject respawnUI = null;
    private GameObject playerCharacterInstance = null;

    public delegate void PlayerEventHandler(GameObject player);

    public event PlayerEventHandler OnLocalPlayerSpawn;
    public event PlayerEventHandler OnLocalPlayerDeath;

    //In this method go things that initialize the LOCAL versions of PlayerNetworkObject (so that each client has its own PlayerNetworkObject inside the GameManager for instance)
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        
        //Check if this client has authority over this newly spawned PlayerNetworkObject
        if (!isLocalPlayer)
        {
            //It does not have authority. Get the hell outta here
            return;
        }

        //Set the local instance of the network object to this
        GameManager.SetLocalPlayerNetworkObject(this);

        //Error check
        if (GameManager.localPlayerNetworkInstance == null)
        {
            Debug.LogError("Player Network Object couldn't be set!");
        }

        OnLocalPlayerSpawn += DeactivateRespawnUI;
        OnLocalPlayerDeath += ActivateRespawnUI;
        playerCharacterStats = playerPrefab.GetComponent<CharacterStatsComponent>().characterStats;
        
       
       
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            //It does not have authority. Get the hell outta here
            return;
        }

        //Spawn the ACTUAL PlayerObject on the server
        //Only spawn the player in Start() so that each object has time to subscribe to the events OnPlayerSpawned etc. AND 
        //so they actually receive the first event
        CmdSpawnPlayer();

    }

    //This command spawns the player character
    [Command]
    public void CmdSpawnPlayer()
    {
        //Only spawn character if it's dead
        if(isAlive == true)
        {
            return;
        }    

        //First instantiate the prefab, do any modifications/settings to it and then SPAWN it on the server
        playerCharacterInstance = Instantiate(playerPrefab);
        
        
        CharacterStatsComponent statsComponent = playerCharacterInstance.GetComponent<CharacterStatsComponent>();

        //----TEMPORARY-----
        statsComponent.team = Teams.Modernists;
        //----/TEMPORARY-----


        NetworkServer.Spawn(playerCharacterInstance, connectionToClient);

        //Notify the client that the player object has been spawned, as well as send a reference to it for convenience
        TargetPlayerHasSpawned(connectionToClient, playerCharacterInstance.GetComponent<NetworkIdentity>());

        isAlive = true;

    }


    /// <summary>
    ///TargetRPC that triggers the PlayerSpawned event. You can only do this here because 1. the OnStartAuthority
    ///doesn't know what the player object is or 2. when it actually spawns
    /// </summary>
    [TargetRpc]
    void TargetPlayerHasSpawned(NetworkConnection conn, NetworkIdentity player)
    {
        OnLocalPlayerSpawn(player.gameObject);
    }


    /// <summary>
    /// Function gets called LOCALLY when the player dies. For handling things like UI, local variables etc.
    /// </summary>
    public void LocalPlayerDied()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        OnLocalPlayerDeath(this.gameObject);

    }

    /// <summary>
    /// Function gets called ON THE SERVER when the player dies. For handling the actual death despawn, character stats etc.
    /// </summary>
    public void ServerPlayerDied()
    {
        if (!isServer)
        {
            return;
        }

        //Update the current stats, so you can spawn the character with the new stats next time
        SaveCharacterStats();
       
        isAlive = false;
        
        NetworkServer.Destroy(playerCharacterInstance);
    }

    private void SaveCharacterStats()
    {
        //Save all the stats in the CharacterStats ScriptableObject so that it respawns with the new stats next time
        var charStatsComponent = playerCharacterInstance.GetComponent<CharacterStatsComponent>();
        charStatsComponent.characterStats.maxHealth = charStatsComponent.maxHealth;
        charStatsComponent.characterStats.attackDamage = charStatsComponent.attackDamage;
        charStatsComponent.characterStats.attackTime = charStatsComponent.attackTime;
        charStatsComponent.characterStats.currentHealth = charStatsComponent.currentHealth;

    }

    /// <summary>
    /// Spawns the registered prefab with no authority at the given location and with the given rotation.
    /// </summary>
    /// <param name="registeredPrefab"></param>
    public void SpawnObjectWithNoAuthority(int prefabID, Vector3 newPosition, Quaternion newRotation)
    {
        CmdSpawnObjectWithNoAuthority(prefabID, newPosition, newRotation);
         
    }

    [Command]
    void CmdSpawnObjectWithNoAuthority(int prefabID, Vector3 newPosition, Quaternion newRotation)
    {
        GameObject go = Instantiate(GameManager.spawnIDMap.GetPrefab(prefabID));
        
        //Use the correct Transform
        go.transform.position = newPosition;
        go.transform.rotation = newRotation;
        //Spawn the object without authority
        NetworkServer.Spawn(go);
    }


    public void ActivateRespawnUI(GameObject player)
    {
        if (respawnUI != null)
        {
            respawnUI.SetActive(true);
        }
    }
    public void DeactivateRespawnUI(GameObject player)
    {
        if (respawnUI != null)
        {
            respawnUI.SetActive(false);
        }
    }

}
