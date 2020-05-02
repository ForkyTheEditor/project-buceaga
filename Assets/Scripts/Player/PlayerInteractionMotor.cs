using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    //A LayerMask that ignores everything besides the current interaction focus
    private LayerMask currentInteractionMask;


    //The range that the player actually needs to have in order to do the current action (e.g. if the current action is mining
    //relevantRange will be equal to miningRange)
    private float relevantRange = 0;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<PlayerController>();

    }

    // Update is called once per frame
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
            if (currentInteractFocus.gameObject.tag == "Resource")
            {
                relevantRange = miningRange;
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

                        //Interact with it
                        CmdStartDefaultInteract(currentInteractFocus.networkId);


                        isInteracting = true;
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

    public void SetInteractingFocus(Interactable newFocus)
    {
        previousInteractFocus = currentInteractFocus;
        currentInteractFocus = newFocus;

        if (previousInteractFocus != null)
        {
            //Stop the interaction with the previous focus
            if (currentInteractFocus == null)
            {
                CmdStopInteract(previousInteractFocus.networkId);
            }
            else if (!GameObject.ReferenceEquals(previousInteractFocus, currentInteractFocus))
            {
                CmdStopInteract(previousInteractFocus.networkId);
            }


            isInteracting = false;

        }


    }

    [Command]
    void CmdStartDefaultInteract(NetworkIdentity netId)
    {

        netId.gameObject.GetComponent<Interactable>().StartDefaultInteract(this.gameObject);
    }

    [Command]
    void CmdStopInteract(NetworkIdentity netId)
    {
        netId.gameObject.GetComponent<Interactable>().StopInteract(this.gameObject);

    }
}
