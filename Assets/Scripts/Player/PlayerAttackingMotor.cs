using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerController))]
public class PlayerAttackingMotor : NetworkBehaviour
{
    private PlayerController controller;
    private CharacterStats characterStats;

    private Attackable attackingFocus;

    private bool canAttack = true;

    [SerializeField]
    private float attackRange = 0.3f;

    private float attackTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<PlayerController>();
        characterStats = gameObject.GetComponent<CharacterStats>();
        attackTimer = characterStats.attackTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        

        ActOnAttackingFocus();
    }

    public void SetAttackingFocus(Attackable newFocus)
    {
        attackingFocus = newFocus;
    }

    void ActOnAttackingFocus()
    {
        if (!canAttack)
        {
            attackTimer -= Time.deltaTime;

        }
        if(attackTimer <= 0)
        {
            attackTimer = characterStats.attackTime;
            canAttack = true;
        }

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
        netId.gameObject.GetComponent<Attackable>().DefaultAttack(this.gameObject, characterStats.attackDamage);
        
    }

}
