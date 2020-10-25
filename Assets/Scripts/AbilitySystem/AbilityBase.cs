using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SYSTEM IN DEVELOPMENT. SUBJECT TO CHANGE.
/// </summary>
[CreateAssetMenu(menuName ="Abilities/Base Ability", fileName = "New Base Ability")]
public class AbilityBase : ScriptableObject
{
    public new string name;
    [TextArea]
    public string description;
    public Sprite icon;
    public float baseCooldown;
    public float baseCost;

    public GameObject abiltyGameObject;
    public AbilityBehaviour abilityBehaviour;

    private void Awake()
    {
        if(abilityBehaviour != null)
        {
            //Set the ability data instance to this object
            abilityBehaviour.abilityData = this;
        }
    }

}