using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// NetworkBehaviour representing the PlayerCharacter stats component. Handles death, health keeping, stats keeping etc.
/// </summary>
public class PlayerStatsComponent : CharacterStatsComponent
{
    private void Awake()
    {
        base.OnAwake();

    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        
        //TEMPORARY
        ClientScene.localPlayer.GetComponent<PlayerNetworkObject>().DeactivateRespawnUI();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    private void LateUpdate()
    {
        base.OnLateUpdate();
      
        if(UnityEngine.InputSystem.Keyboard.current.tKey.wasPressedThisFrame)
        {
            _currentHealth = new Stat(currentHealth.baseValue - 50, 0);
        }
        if (UnityEngine.InputSystem.Keyboard.current.qKey.wasPressedThisFrame)
        {
            _maxHealth = new Stat(_maxHealth.baseValue + 50, 0);
        }
        

    }
    
    protected override void LocalCharacterDied()
    {
        if (!hasAuthority)
        {
            return;
        }

        base.LocalCharacterDied();
        GameManager.localPlayerNetworkInstance.LocalPlayerDied();
    }

    protected override void ServerKillCharacter()
    {
        if (!isServer)
        {
            return;
        }

        //We don't call the base function as we want specific player functionality to happen, not just destroy the character

        CustomNetworkManager.playerObjects[connectionToClient].GetComponent<PlayerNetworkObject>().ServerPlayerDied();
    }
}
