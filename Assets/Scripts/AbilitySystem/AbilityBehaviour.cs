using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBehaviour : MonoBehaviour
{
    public abstract AbilityBase abilityData { get; set; }

    public abstract void CastAbility(GameObject source, GameObject[] targets);


}
