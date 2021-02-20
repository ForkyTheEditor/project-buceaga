using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(CharacterStatsComponent))]
public class Attackable : NetworkBehaviour
{
    public NetworkIdentity networkId { get; private set; }

    public delegate void AttackEventHandler(GameObject source, int dmgAmount, EventArgs args);

    public event AttackEventHandler Attacked;

    private void Awake()
    {
        networkId = GetComponent<NetworkIdentity>();
    }

    public virtual void DefaultAttack(GameObject source, int dmgAmount)
    {
        if(Attacked != null)
        {
            Attacked(source, dmgAmount, EventArgs.Empty);

        }
    }

}
