using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public static Dictionary<NetworkConnection, NetworkIdentity> playerObjects = new Dictionary<NetworkConnection, NetworkIdentity>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        playerObjects[conn] = player.GetComponent<NetworkIdentity>();
        NetworkServer.AddPlayerForConnection(conn, player);
       

    }
}
