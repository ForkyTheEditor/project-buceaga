﻿using System.Linq.Expressions;
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
public class PlayerController : NetworkBehaviour
{
    //An reference to the main camera 
    Camera cam;
    //A reference to the NavMeshAgent
    NavMeshAgent navAgent;

    //A reference to the character's stats
    private CharacterStats charStats;
    //A reference to the component that handles all the interaction with interactables (buildings etc.)
    private PlayerInteractionMotor interactionMotor;
    private PlayerAttackingMotor attackingMotor;
    private PlayerAnimationMotor animationMotor;

    //The target towards which the player is walking
    private Transform currentNavTarget;

    //Is the player walking (actually moving, if movement is impossible, this will still be false) towards something? 
    private bool _isRunning = false;
    public bool isRunning { get { return _isRunning; } }

    //Initialize components in awake so that they're ready if other gameobjects might need them (not the case, just good practice)
    void Awake()
    { 
        cam = Camera.main;
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        charStats = gameObject.GetComponent<CharacterStats>();
        interactionMotor = gameObject.GetComponent<PlayerInteractionMotor>();
        attackingMotor = gameObject.GetComponent<PlayerAttackingMotor>();
        animationMotor = gameObject.GetComponent<PlayerAnimationMotor>();
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
            //You do not have authority. Get the hell out of here
            return;
        }
        
        //Check if the player pressed the right mouse button 
        //The right mouse button is the "Default Interaction" button
        CheckRightMouseClick();
        //Update the player's current state
        UpdatePlayerState();
    }

    private void LateUpdate()
    {
        FollowNavTarget();
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

        //Check if the player is ACTUALLY moving
        if(navAgent.velocity != Vector3.zero)
        {
            _isRunning = true;
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
        if(currentNavTarget != null)
        {
            
            navAgent.SetDestination(currentNavTarget.position);
          
        }
    }


}
