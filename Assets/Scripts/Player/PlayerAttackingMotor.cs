using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerController))]
public class PlayerAttackingMotor : NetworkBehaviour
{
    private PlayerController controller;
    private PlayerStatsComponent characterStats;

    private Attackable attackingFocus;

    private bool canAttack = true;

    [SerializeField]
    private float attackRange = 0.6f;

    
    private float attackTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<PlayerController>();
        characterStats = gameObject.GetComponent<PlayerStatsComponent>();
        attackTime = characterStats.attackTime.GetFinalValue();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        //A check if the focus is null is done in the function, don't do one here as the function also handles attack speed. (attack refresh time doesn't change
        //when the focus is changed)
        ActOnAttackingFocus();
    }

    public void SetAttackingFocus(Attackable newFocus)
    {
        attackingFocus = newFocus;
    }

    void ActOnAttackingFocus()
    {
        //Attack timer
        if (!canAttack)
        {
            attackTime -= Time.deltaTime;

        }
        if(attackTime <= 0)
        {
            attackTime = characterStats.attackTime.GetFinalValue();
            canAttack = true;
        }

        //Check a target actually exists
        if (attackingFocus != null)
        {

            float distanceToFocus = (attackingFocus.transform.position - transform.position).magnitude;
            if (distanceToFocus <= attackRange)
            {
                //You are within attacking range
                if (canAttack)
                {
                    //Attack it
                    CmdAttack(attackingFocus.networkId);

                    canAttack = false;

                }

                controller.PausePlayerMovement();
            }
            else
            {
                //The enemy has exited the attack range
                //Resume the following

                controller.ResumePlayerMovement();
            }


        }


    }

    [Command]
    private void CmdAttack(NetworkIdentity netId)
    {
        netId.gameObject.GetComponent<Attackable>().DefaultAttack(this.gameObject, characterStats.attackDamage.GetFinalValue());
        
    }

}
