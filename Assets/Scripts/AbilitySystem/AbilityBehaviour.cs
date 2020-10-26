using UnityEngine;

public abstract class AbilityBehaviour : MonoBehaviour
{
    public AbilityBase abilityData;

    public abstract void CastAbility(GameObject source, GameObject[] targets);


}

public class projectildata : AbilityBase
{
    public int damage;
    
    public override void InitializeAbility()
    {
        
    }
   
}

public class Fireball : AbilityBehaviour
{

    
    public override void CastAbility(GameObject source, GameObject[] targets)
    {
        
    }
}
