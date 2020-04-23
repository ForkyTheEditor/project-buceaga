using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
//Forces the GameObject to attach a NavMeshAgent component
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : NetworkBehaviour
{
    //An reference to the main camera 
    Camera cam;
    //A reference to the NavMeshAgent
    NavMeshAgent navAgent;
    
    //The object the Player is currently focusing (the object the Player last clicked on)
    [SerializeField]
    private Interactable currentFocus;
    //The last object focused; Mainly an auxiliary reference to the last object focused
    private Interactable previousFocus;

    private bool isInteracting = false;
   
    [SerializeField]
    //The range the player needs to be in to be able to mine the resource
    private float miningRange = 0.5f;

    //A LayerMask that ignores everything besides the current focus
    private LayerMask currentMask;

    //The range that the player actually needs to have in order to do the current action (e.g. if the current action is mining
    //relevantRange will be equal to miningRange)
    private float relevantRange = 0;

    // Start is called before the first frame update
    void Start()
    {
        
        cam = Camera.main;
        navAgent = this.GetComponent<NavMeshAgent>();
        
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
        ActOnTheFocus();

        //Check if the player pressed the right mouse button 
        //The right mouse button is the "Default Interaction" button
        CheckRightMouseClick();
    }



    //Sets the current focus of the player to the given Interactable object
    void SetFocus(Interactable newFocus)
    {
        previousFocus = currentFocus;
        currentFocus = newFocus;

        if (previousFocus != null)
        {
            //Stop the interaction with the previous focus
            if (currentFocus == null)
            {
                CmdStopInteract(previousFocus.netId);
            }
            else if(!GameObject.ReferenceEquals(previousFocus, currentFocus))
            {
                CmdStopInteract(previousFocus.netId);
            }

            isInteracting = false;

        }


    }
    

    /// <summary>
    ///Performs the relevant action depending on the focus
    /// </summary>
    void ActOnTheFocus()
    {
        //Check if there is a current action (otherwise the player is either just walking or idling)
        if (currentFocus != null)
        {
            Ray ray = new Ray(transform.position, currentFocus.gameObject.transform.position - transform.position);
            RaycastHit hitInfo;
            currentMask = LayerMask.GetMask(LayerMask.LayerToName(currentFocus.gameObject.layer));

            //Check which is the current relevant range
            if (currentFocus.gameObject.tag == "Resource")
            {
                relevantRange = miningRange;
            }

            //Check if the player is within relevant range
            if (Physics.Raycast(ray, out hitInfo, relevantRange, currentMask.value))
            {
                
                //TODO: Separate the kinds of interaction based on the player input

                //Check if it is the same gameobject that the player clicked on
                if (GameObject.ReferenceEquals(hitInfo.collider.gameObject, currentFocus.gameObject))
                {
                    if (!isInteracting)
                    {
                        //The player is in interaction range with the current focus
                        //Stop the player from moving towards the target (it's already within interaction range)
                        navAgent.ResetPath();

                        //Interact with it
                        CmdStartDefaultInteract(currentFocus.netId);
                        
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
            currentMask = 0;
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
                //If the object isn't interactable, then the focus will be null
                navAgent.SetDestination(hitInfo.point);
                SetFocus(hitInfo.collider.GetComponent<Interactable>());

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
