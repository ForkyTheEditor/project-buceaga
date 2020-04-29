using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
//Forces the GameObject to attach a NavMeshAgent component
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(PlayerInteractionMotor))]
public class PlayerController : NetworkBehaviour
{
    //An reference to the main camera 
    Camera cam;
    //A reference to the NavMeshAgent
    NavMeshAgent navAgent;
    
    //A reference to the character's stats
    private CharacterStats stats;
    //A reference to the component that handles all the interaction with interactables (buildings etc.)
    private PlayerInteractionMotor interactionMotor;

    private Attackable attackingFocus;

    private bool isInteracting = false;

    [SerializeField]
    private float attackRange = 0.3f;
    

    // Start is called before the first frame update
    void Start()
    {
        
        cam = Camera.main;
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        stats = gameObject.GetComponent<CharacterStats>();
        interactionMotor = gameObject.GetComponent<PlayerInteractionMotor>();
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
      
        //Check if the player pressed the right mouse button 
        //The right mouse button is the "Default Interaction" button
        CheckRightMouseClick();
    }

    void SetAttackingFocus(Attackable newFocus)
    {
        attackingFocus = newFocus;
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
                    interactionMotor.SetInteractingFocus(hitInfo.collider.GetComponent<Interactable>());
                   
                }

               

            }
        }

    }

    /// <summary>
    /// Stops the player from moving
    /// </summary>
    public void StopPlayerMovement()
    {

        navAgent.ResetPath();

    }


}
