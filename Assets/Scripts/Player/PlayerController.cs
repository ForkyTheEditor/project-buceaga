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
[RequireComponent(typeof(PlayerAttackingMotor))]
[RequireComponent(typeof(ResourceInventory))]
[RequireComponent(typeof(PlayerAnimationMotor))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    //An reference to the main camera 
    Camera cam;
    //A reference to the NavMeshAgent
    NavMeshAgent navAgent;
    //Reference to the rigidbody attached to this gameobject
    private Rigidbody rb;
    //A reference to the character's stats
    private CharacterStats charStats;
    //A reference to the component that handles all the interaction with interactables (buildings etc.)
    private PlayerInteractionMotor interactionMotor;
    private PlayerAttackingMotor attackingMotor;
    private PlayerAnimationMotor animationMotor;
    
    //The target towards which the player is walking
    private Transform currentNavTarget;

    [SyncVar]
    private Vector3 syncedPosition; // Position sent to the server for smooth and correct movement between clients
    [SyncVar]
    private Quaternion syncedRotation; // Rotation sent to the server - || -  

    private Vector3 prevPos; //The previous position of this object
    private Vector3 currentPos; //The current position of the object; Together the two are used to determine if the player has moved;

    private float updateInterval;
    private float updatePeriod = 0.11f; //The period of time between each update; currently ~ 9 times / second

    //Is the player walking (actually moving, if movement is impossible, this will still be false) towards something? 
    private bool _isRunning = false;
    public bool isRunning { get { return _isRunning; } }

    //Delegate to handle hotkey presses by the player; For now it only takes in the keycode of the key pressed, but in the future we may have to add more complex behaviour
    //for instance taking in macros such as CTRL + ALT + W 
    public delegate void HotkeyHandler (GameObject source, KeyCode kc);
    //Event raised when a certain hotkey is pressed
    public event HotkeyHandler HotkeyPressed;
    

    //Initialize components in awake so that they're ready if other gameobjects might need them (not the case, just good practice)
    void Awake()
    { 
        cam = Camera.main;
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        charStats = gameObject.GetComponent<CharacterStats>();
        interactionMotor = gameObject.GetComponent<PlayerInteractionMotor>();
        attackingMotor = gameObject.GetComponent<PlayerAttackingMotor>();
        animationMotor = gameObject.GetComponent<PlayerAnimationMotor>();
        rb = gameObject.GetComponent<Rigidbody>();

        prevPos = transform.position;
        currentPos = transform.position;
    }

    public override void OnStartAuthority()
    {
        //LOAD THE CURRENT GAMEOBJECT INTO THE GAME MANAGER
        //THIS IS THE LOCAL PLAYER INSTANCE
        //ONLY CALL THIS ON THE CLIENT WHOSE OBJECT THIS IS (AKA THE CLIENT WITH AUTHORITY)
        GameManager.SetLocalPlayer(this.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        //Check if the client running this code has authority over this gameobject
        if (!hasAuthority)
        {
            //Interpolate the position and rotation for client objects with no authority for smooth movement / rotation
            transform.position = Vector3.Lerp(transform.position, syncedPosition, 0.08f);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncedRotation, 0.08f);

            //You do not have authority. Get the hell out of here
            return;
        }

        //Check if the player pressed the right mouse button 
        //The right mouse button is the "Default Interaction" button
        CheckRightMouseClick();

        //Timer for the sync update
        updateInterval += Time.deltaTime;
        if (updateInterval >= updatePeriod)
        {
            //Sync the movement and the rotation
            CmdSyncPositionRotation(transform.position, transform.rotation);

            //Reset timer
            updateInterval = 0;
        }

        //I couldn't figure out a better way to do this for now and it doesn't really matter for the prototype; So for now enjoy this ugliness
        if (Input.GetKeyDown(KeyCode.B))
        {
            HotkeyPressed(this.gameObject, KeyCode.B);
        }

    }

    private void LateUpdate()
    {  
        //Update the player's current state, before checking for authority, as you want to update the state across ALL clients (for animations, syncing etc.)
        UpdatePlayerState();

        //Check if the client running this code has authority over this gameobject
        if (!hasAuthority)
        {
            //You do not have authority. Get the hell out of here
            return;
        }

        FollowNavTarget();
    }

    /// <summary>
    /// Syncs the position and rotation of the player from client to server
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    [Command]
    void CmdSyncPositionRotation(Vector3 position, Quaternion rotation)
    {
        syncedPosition = position;
        syncedRotation = rotation;
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
                //Delete the previous follow target
                SetNavTarget(null);
                //Override any stopped movement done by the motors
                ResumePlayerMovement();
                //Clear the player's focuses so that no weird interaction happens (and so the Great Bug of May doesn't happen again)
                RemoveAllFocus();

                //Switch to determine which type of object was hit
                switch (hitInfo.transform.tag)
                {
                    case "Player":
                        {
                            //Check if we didn't click ourselves
                            if(!GameObject.ReferenceEquals(hitInfo.transform.gameObject, this.gameObject))
                            {

                                //We've clicked a player. Attack him!
                                //...but only if he's on the other team! (or maybe if we implement a deny mechanic or something)
                                if(hitInfo.transform.GetComponent<CharacterStats>().team != charStats.team)
                                {
                                    attackingMotor.SetAttackingFocus(hitInfo.collider.GetComponent<Attackable>());
                                }
                                //Follow if its a player (or other unit, but that will be later)
                                SetNavTarget(hitInfo.transform);
                            }
                            break;
                        }
                    case "Resource":
                        {
                            interactionMotor.SetInteractingFocus(hitInfo.collider.GetComponent<Interactable>());
                            break;
                        }
                    default:
                        {
                            RemoveAllFocus();
                            break;
                        }


                }
            }
        }
    }

    /// <summary>
    /// Updates the player's current state (idle, moving, attacking etc.)
    /// This can be used for animations, abilities that rely on different states etc.
    /// </summary>
    private void UpdatePlayerState()
    {

        //Update the current position
        currentPos = transform.position;

        //Check if the player is ACTUALLY moving, by checking if the position has changed
        //Also check that they are INTENTIONALLY moving
        if(prevPos != currentPos )
        {
            _isRunning = true;
            prevPos = currentPos;
            
        }
        else
        {
            _isRunning = false;
        }
        

    }
    
    /// <summary>
    /// Removes the focuses from all the control motors
    /// </summary>
    public void RemoveAllFocus()
    {
        interactionMotor.SetInteractingFocus(null);
        attackingMotor.SetAttackingFocus(null);
    }

    /// <summary>
    /// Resumes the previous path
    /// </summary>
    public void ResumePlayerMovement()
    {
        navAgent.isStopped = false;
    }

    /// <summary>
    /// Pauses the player movement, but retains the previous path
    /// </summary>
    public void PausePlayerMovement()
    {
        navAgent.isStopped = true;
    }

    /// <summary>
    /// Stops the player from moving
    /// </summary>
    public void StopPlayerMovement()
    {

        navAgent.ResetPath();

    }

    /// <summary>
    /// Sets the target for the NavAgent to follow
    /// </summary>
    /// <param name="target">The target GameObject to follow</param>
    private void SetNavTarget(Transform target)
    {
        currentNavTarget = target;
    }

    //Follows the specified target; if the target moves its position, it follows it indefinitely
    private void FollowNavTarget()
    {
        //Check for errors
        if(currentNavTarget != null)
        {
            //Check that the target isn't already within a reasonable range, so no weird movement happens
            //TODO: Make this distance a variable (possibly based on the other interaction range variables)
            if ((currentNavTarget.position - transform.position).magnitude < 1.5f )
            {
                PausePlayerMovement();
            }
            else
            {
                ResumePlayerMovement();
                //Go to target's current position
                navAgent.SetDestination(currentNavTarget.position);

            }


        }
    }


}
