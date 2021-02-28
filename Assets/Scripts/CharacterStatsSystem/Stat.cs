using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Struct representing a character / building stat. It handles base values and additive / multiplicative modifiers.
/// </summary>
public readonly struct Stat : System.IEquatable<Stat>
{
    /// TODO: Work on the equations of modifiers. Multiplicative, additive modifiers, post additive, post multiplicative etc. For now only additive.

    public readonly int baseValue;
    public readonly int additiveModifier;

 
    public Stat(int baseVal, int addMods)
    {
        this.baseValue = baseVal;
        this.additiveModifier = addMods;
    }

    public bool Equals(Stat other)
    {
        if(this.baseValue == other.baseValue && this.additiveModifier == other.additiveModifier)
        {
            return true;
        }
        return false;
    }

    public int GetFinalValue()
    {
        return baseValue + additiveModifier;
    }



}
