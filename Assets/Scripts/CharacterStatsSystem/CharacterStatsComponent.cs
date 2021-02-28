using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

//TODO: Refactor this code so it sits on the PlayerNetworkObject. Why? Because the PlayerObject gets destroyed when the character dies => you lose everything, including references to it
//sitting on the UI scripts, camera scripts etc.
/// <summary>
/// MonoBehaviour handling common character stats. Everything in here is protected because it's meant to be inherited and overwritten by more complex characters.
/// Works just fine for minor NPCs.
/// </summary>
[RequireComponent(typeof(Attackable))]
public class CharacterStatsComponent : NetworkBehaviour
{
    private bool isAlive = true;

    [SerializeField]
    //Data container for this character's stats. They get loaded from the scriptable object onto this script
    protected CharacterStatsDefaultValues _characterStats;

    [SyncVar(hook = nameof(OnCurrentHealthChanged))]
    protected Stat _currentHealth = new Stat(1, 0);
    [SyncVar]
    protected Stat _maxHealth = new Stat(1, 0);
    [SyncVar]
    protected Teams _team;

    [SyncVar]
    protected Stat _attackTime = new Stat(1, 0);
    [SyncVar]
    protected Stat _attackDamage = new Stat(1, 0);

    protected Attackable attackableComponent;

    //Because SyncVar can't handle properties and because of accessibility issues, these have to stay here. While inside the class, work with the underscore fields.
    public CharacterStatsDefaultValues characterStats
    {
        get { return _characterStats; }
        set
        {
            _characterStats = value;
            LoadStatsFromScriptableObject();
        }
    }
    public Stat currentHealth { get { return _currentHealth; } set { _currentHealth = value; } }
    public Stat maxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
    public Stat attackTime { get { return _attackTime; } }
    public Stat attackDamage { get { return _attackDamage; } }
    public Teams team { get { return _team; } set { _team = value; } }

    private void Awake()
    {
        OnAwake();
    }

    private void LateUpdate()
    {
        OnLateUpdate();
    }

    protected virtual void OnCurrentHealthChanged(Stat oldHealth, Stat newHealth)
    {
        if(newHealth.GetFinalValue() <= 0)
        {
            LocalCharacterDied();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        //Load the stats into the local fields.
        LoadStatsFromScriptableObject();

        //Set the current health as the max health when starting
        _currentHealth = new Stat(_maxHealth.GetFinalValue(), 0);
    }

    /// <summary>
    /// Method to be called in the Awake function, containing default behaviour. Initializes values.
    /// </summary>
    protected void OnAwake()
    {
        attackableComponent = GetComponent<Attackable>();
        attackableComponent.Attacked += OnAttacked;
    }

    /// <summary>
    /// Method to be called in the LateUpdate function, containing default behaviour.
    /// </summary>
    protected void OnLateUpdate()
    {
        HandleCurrentHealthUpdate();
    }

    protected void HandleCurrentHealthUpdate()
    {
        //Let only the server handle character deaths (it's important and it prevents cheating + handles updating of clients)
        if (!isServer)
        {
            return;
        }

        //The base health value of a character can't go passed the max value's final value
        //This means however that the final value of the current health CAN go up, but only because of modifiers when already at full HP
        _currentHealth = new Stat(Mathf.Clamp(_currentHealth.baseValue, int.MinValue, _maxHealth.GetFinalValue()), _currentHealth.additiveModifier);

        if (currentHealth.GetFinalValue() <= 0 && isAlive)
        {
            ServerKillCharacter();
            isAlive = false;
        }
    }

    //Called when this character is attacked
    protected void OnAttacked(GameObject source, int amount, EventArgs args)
    {
        _currentHealth = new Stat(_currentHealth.baseValue - amount, _currentHealth.additiveModifier);

    }
    
    protected virtual void LocalCharacterDied()
    {
        
    }

    /// <summary>
    /// Method for destruction of this character. Can be overwritten for more specific functionality (such as for the player).
    /// </summary>
    protected virtual void ServerKillCharacter()
    {
        if (!isServer)
        {
            return;
        }
        //Probably will include death animation call as that is something every character (presumably) has.

        NetworkServer.Destroy(this.gameObject);

    }

    /// <summary>
    /// Loads the stats with the values from the CharacterStats data object. When adding a new stat, load it here.
    /// </summary>
    protected virtual void LoadStatsFromScriptableObject()
    {
        if (_characterStats != null)
        {
            
            _currentHealth = _characterStats.currentHealth;
            _maxHealth = _characterStats.maxHealth;
            _attackDamage = _characterStats.attackDamage;
            _attackTime = _characterStats.attackTime;
        }
    }

}
