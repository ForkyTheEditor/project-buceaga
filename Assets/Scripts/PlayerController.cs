using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
//Forces the GameObject to attach a NavMeshAgent component
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class PlayerController : NetworkBehaviour
{
    //An reference to the main camera 
    Camera cam;
    //A reference to the NavMeshAgent
    NavMeshAgent navAgent;
    
    //The current action taken by the player
    private CharacterActions currentAction;
    //A reference to the character's stats
    private CharacterStats stats;


    private Attackable attackingFocus;

    //The object the Player is currently focusing (the object the Player last clicked on)
    [SerializeField]
    private Interactable currentInteractFocus;
    //The last object focused; Mainly an auxiliary reference to the last object focused
    private Interactable previousInteractFocus;

    private bool isInteracting = false;

    [SerializeField]
    private float attackRange = 0.3f;
    [SerializeField]
    //The range the player needs to be in to be able to mine the resource
    private float miningRange = 0.5f;

    //A LayerMask that ignores everything besides the current interaction focus
    private LayerMask currentInteractionMask;

    
    //The range that the player actually needs to have in order to do the current action (e.g. if the current action is mining
    //relevantRange will be equal to miningRange)
    private float relevantRange = 0;

    // Start is called before the first frame update
    void Start()
    {
        
        cam = Camera.main;
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        stats = gameObject.GetComponent<CharacterStats>();
        currentAction = CharacterActions.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if the client running this code has authority over this gameobject
        if (!hasAuthority)
        {
            //You do not have authority. Get the hell out of here
            return;
        }
      
        //Perform the relevant action according to the focus
        ActOnInteractingFocus();

        //Check if the player pressed the right mouse button 
        //The right mouse button is the "Default Interaction" button
        CheckRightMouseClick();
    }



    //Sets the current focus of the player to the given Interactable object
    void SetInteractingFocus(Interactable newFocus)
    {
        previousInteractFocus = currentInteractFocus;
        currentInteractFocus = newFocus;

        if (previousInteractFocus != null)
        {
            //Stop the interaction with the previous focus
            if (currentInteractFocus == null)
            {
                CmdStopInteract(previousInteractFocus.netId);
            }
            else if(!GameObject.ReferenceEquals(previousInteractFocus, currentInteractFocus))
            {
                CmdStopInteract(previousInteractFocus.netId);
            }

            
            isInteracting = false;

        }


    }

    void SetAttackingFocus(Attackable newFocus)
    {
        attackingFocus = newFocus;
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
                        navAgent.ResetPath();

                        //Interact with it
                        CmdStartDefaultInteract(currentInteractFocus.netId);

                        currentAction = CharacterActions.Interact;
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

    void ActOnAttackingFocus()
    {
        if(attackingFocus != null)
        {
            float distanceToFocus = (attackingFocus.transform.position - transform.position).magnitude;
            if(distanceToFocus <= attackRange)
            {
                //You are within attacking range

            }


        }


    }


    void CheckRightMouseClick()
    {
        //If the player right clicks somewhere on the map, start moving towards that point
        if (Input.GetMouseButtonDown(1))
        {
            //Shoot a ray through the screen at the position of the cursor
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                //Move towards the click either way
                navAgent.SetDestination(hitInfo.point);

                if (hitInfo.transform.tag == "Player")
                {
                    SetAttackingFocus(hitInfo.collider.GetComponent<Attackable>());
                    

                }
                else
                {
                    //If the object isn't interactable, then the focus will be null
                    SetInteractingFocus(hitInfo.collider.GetComponent<Interactable>());
                }

               

            }
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
