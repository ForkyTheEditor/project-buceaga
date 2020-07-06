using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimationMotor : MonoBehaviour
{
    //Reference to the animator component on the player GameObject
    private Animator playerAnimator;
    //Reference to the player controller of this object; used to figure out what the player is doing (to know which animation to play) 
    private PlayerController playerController;

    // Start is called before the first frame update
    void Awake()
    {
        //Get the relevant references
        playerAnimator = gameObject.GetComponentInChildren<Animator>();
        //Throw animator error
        if(playerAnimator == null)
        {
            Debug.LogError("Player animator not found in children!");
        }
        playerController = gameObject.GetComponent<PlayerController>();
    }

    private void LateUpdate()
    {
        //Update the running state to the state of the controller
        playerAnimator.SetBool("isRunning", playerController.isRunning);
    }

}
