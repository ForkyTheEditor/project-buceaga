﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractionMotor : NetworkBehaviour
{
    private bool isInteracting = false;

    private PlayerController controller;

    //The object the Player is currently focusing (the object the Player last clicked on)
    [SerializeField]
    private Interactable currentInteractFocus;
    //The last object focused; Mainly an auxiliary reference to the last object focused
    private Interactable previousInteractFocus;

    [SerializeField]
    //The range the player needs to be in to be able to mine the resource
    private float miningRange = 1f;
    [SerializeField]
    private float defaultRange = 1f;


    //A LayerMask that ignores everything besides the current interaction focus
    private LayerMask currentInteractionMask;

    //The range that the player actually needs to have in order to do the current action (e.g. if the current action is mining
    //relevantRange will be equal to miningRange)
    private float relevantRange = 0;

    void Awake()
    {
        controller = gameObject.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        ActOnInteractingFocus();
    }

    /// <summary>
    ///Performs the relevant action depending on the focus
    /// </summary>
    void ActOnInteractingFocus()
    {
        //Check if there is a current action (otherwise the player is either just walking or idling)
        if (currentInteractFocus != null)
        {
            Ray ray = new Ray(transform.position, currentInteractFocus.gameObject.transform.position - transform.position);
            RaycastHit hitInfo;
            currentInteractionMask = LayerMask.GetMask(LayerMask.LayerToName(currentInteractFocus.gameObject.layer));

            //Check which is the current relevant range
            switch (currentInteractFocus.gameObject.tag)
            {
                case "Resource":
                    relevantRange = miningRange;
                    break;
                default:
                    relevantRange = defaultRange;
                    break;

            }


            //Check if the player is within relevant range
            if (Physics.Raycast(ray, out hitInfo, relevantRange, currentInteractionMask.value))
            {
                //TODO: Separate the kinds of interaction based on the player input

                //Check if it is the same gameobject that the player clicked on
                if (GameObject.ReferenceEquals(hitInfo.collider.gameObject, currentInteractFocus.gameObject))
                {
                    if (!isInteracting)
                    {
                        //The player is in interaction range with the current focus
                        //Stop the player from moving towards the target (it's already within interaction range)
                        controller.StopPlayerMovement();
                        
                        isInteracting = true;


                        //Start interacting with it
                        CmdStartDefaultInteract(currentInteractFocus.gameObject);

                    }
                }
            }
        }
        else
        {
            //There is no current focus
            //The relevant range must be cleared
            relevantRange = 0;
            //The current layer mask should be cleared
            currentInteractionMask = 0;

        }
    }
    /// <summary>
    /// Sets the interaction focus and stops the interaction with the previous focus
    /// </summary>
    /// <param name="newFocus"></param>
    public void SetInteractingFocus(Interactable newFocus)
    {
        previousInteractFocus = currentInteractFocus;
        currentInteractFocus = newFocus;

        if (previousInteractFocus != null)
        {
            //Stop the interaction with the previous focus. There are multiple conditions for this, hence the else if.
            if (currentInteractFocus == null)
            {
                CmdStopDefaultInteract(previousInteractFocus.gameObject);
            }
            else if (!GameObject.ReferenceEquals(previousInteractFocus, currentInteractFocus))
            {
                CmdStopDefaultInteract(previousInteractFocus.gameObject);
            }

            isInteracting = false;
        }
    }

    /// <summary>
    /// Starts the default type of interaction with the object (i.e. tick, single etc.).
    /// All interactions should happen on the server, so that the server can keep count of which / how many entities are interacting with the object => Command.
    /// No matter the type of interaction, the interaction should always be stopped after it was started (because you can only interact with one object at once).
    /// </summary>
    /// <param name="netId"></param>
    
    void CmdStartDefaultInteract(GameObject go)
    {
        go.GetComponent<Interactable>().StartDefaultInteract(this.gameObject);

    }

    /// <summary>
    /// Stops the default type of interaction with the interaction focus
    /// </summary>
    /// <param name="netId"></param>
    
    void CmdStopDefaultInteract(GameObject go)
    {
        go.GetComponent<Interactable>().StopDefaultInteract(this.gameObject);
    }

    
    void CmdStartTickInteract(GameObject go)
    {
        go.GetComponent<Interactable>().CmdStartTickInteract(this.gameObject);

    }
  
  
    void CmdStopTickInteract(GameObject go)
    {
        go.GetComponent<Interactable>().CmdStopTickInteract(this.gameObject);

    }

 
    void CmdStartSingleInteract(GameObject go)
    {
        go.GetComponent<Interactable>().CmdStartSingleInteract(this.gameObject);
    }

  
    void CmdStopSingleInteract(GameObject go)
    {
        go.GetComponent<Interactable>().CmdStopSingleInteract(this.gameObject);
    }
}
