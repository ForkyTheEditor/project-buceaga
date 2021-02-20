using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A data container for the character stats.
/// </summary>
[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "CharacterStats")]
public class CharacterStats : ScriptableObject
{
    /// <summary>
    /// Minor class meant for editor interaction with the readonly Stat structs.
    /// </summary>
    [System.Serializable]
    private class StatClass
    {
        public int baseValue = 0;
        public int additiveModifier = 0;
    }

    [SerializeField] private StatClass _currentHealth = new StatClass();
    [SerializeField] private StatClass _maxHealth = new StatClass();
    [SerializeField] private StatClass _attackTime = new StatClass();
    [SerializeField] private StatClass _attackDamage = new StatClass();

    public Stat currentHealth = new Stat(0,0);
    public Stat maxHealth = new Stat(0, 0);
    public Stat attackTime = new Stat(0, 0);
    public Stat attackDamage = new Stat(0, 0);

    //Function called when a value changed inside the editor
    private void OnValidate()
    {
        UpdateStatValues();

    }

    /// <summary>
    /// Updates the values of the structs with the values from the inspector. Whenever adding a new character stat, make sure to add it here
    /// for inspector interaction.
    /// </summary>
    private void UpdateStatValues()
    {
        currentHealth = new Stat(_currentHealth.baseValue, _currentHealth.additiveModifier);
        maxHealth = new Stat(_maxHealth.baseValue, _maxHealth.additiveModifier);
        attackTime = new Stat(_attackTime.baseValue, _attackTime.additiveModifier);
        attackDamage = new Stat(_attackDamage.baseValue, _attackDamage.additiveModifier);
    }

}
