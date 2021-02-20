using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// MonoBehaviour representing the PlayerCharacter stats component. Handles death, health keeping, stats keeping etc.
/// </summary>
public class PlayerStatsComponent : CharacterStatsComponent
{
    private void Awake()
    {
        base.OnAwake();
    }

    private void LateUpdate()
    {
        base.OnLateUpdate();

    }

    protected override void KillCharacter()
    {
        base.KillCharacter();
    }
}
