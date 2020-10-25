using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ability scriptable object, holding all the information of an ability. When creating a new ability create a new object of this type.
/// </summary>
public abstract class AbilityBase : ScriptableObject
{
    public new string name;
    [TextArea]
    public string description;
    public GameObject abilityGO;
    public AbilityBehaviour abilityBehaviour;
    
    

    
    
}
