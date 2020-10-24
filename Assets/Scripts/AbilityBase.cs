using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract superclass of all abilities in game. Inherit from this when creating a new ability.
/// </summary>
public abstract class AbilityBase : ScriptableObject
{
    public new string name;
    [TextArea]
    public string description;
    public GameObject abilityGO;
    
    

    
    
}
