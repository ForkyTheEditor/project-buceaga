using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for the ability behaviours. Inherit from this when creating a new ability script.
/// </summary>
public interface AbilityBehaviour
{

    void Cast(GameObject source, GameObject[] targets);


}
