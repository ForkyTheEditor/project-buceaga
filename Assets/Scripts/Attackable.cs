using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(CharacterStats))]
public class Attackable : NetworkBehaviour
{

    public NetworkIdentity networkId;

    public delegate void AttackEventHandler(GameObject source, float dmgAmount, EventArgs args);

    public event AttackEventHandler Attacked;

    private void Start()
    {
        networkId = gameObject.GetComponent<NetworkIdentity>();
    }

    public virtual void DefaultAttack(GameObject source, float dmgAmount)
    {
        if(Attacked != null)
        {
            Attacked(source, dmgAmount, EventArgs.Empty);

        }
    }

}
