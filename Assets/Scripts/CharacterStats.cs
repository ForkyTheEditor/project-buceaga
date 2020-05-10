using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

[RequireComponent(typeof(Attackable))]
public class CharacterStats : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    private float _currentHealth = 0f;
    [SyncVar]
    [SerializeField]
    private int _maxHealth = 0;
    [SyncVar]
    [SerializeField]
    private Teams _team;

    [SerializeField]
    private float _attackTime = 1f;
    [SerializeField]
    private float _attackDamage = 0f;

    private Attackable attackableComponent;

    public Teams team { 
        get { 
            return _team; 
        } 
        set { 
            _team = value;
            //------TEMPORARY------
            if (_team == Teams.Modernists){
                gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.green;

            }
            else
            {
                gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.cyan;
            }
            //------/TEMPORARY------

        }
    }
    public float currentHealth { get { return _currentHealth; } }
    public int maxHealth { get { return _maxHealth; } }
    public float attackTime { get { return _attackTime; } }
    public float attackDamage { get { return _attackDamage; } }

    private void Start()
    {
        attackableComponent = GetComponent<Attackable>();
        attackableComponent.Attacked += OnAttacked;
    }

    private void LateUpdate()
    {
        if (!isServer)
        {
            return;
        }

        _currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if(currentHealth <= 0)
        {
            KillCharacter();
        }

    }

    private void OnAttacked(GameObject source , float amount, EventArgs args)
    {
        _currentHealth -= amount;
        
    }

    private void KillCharacter()
    {
        print("I died!");
        Destroy(this.gameObject);

    }



}
