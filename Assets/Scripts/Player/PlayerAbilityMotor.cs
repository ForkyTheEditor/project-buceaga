using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerController))]
public class PlayerAbilityMotor : MonoBehaviour
{

    public AbilityBase[] abilities;

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //ONLY FOR TESTING PURPOSES 
        //INPUT WILL BE MOVED TO PLAYERCONTROLLER FOR LATER UPDATES

        if (Input.GetKeyDown(KeyCode.E))
        {
           // abilities[0].CastAbility(this.gameObject ,null);
        }


    }
}
