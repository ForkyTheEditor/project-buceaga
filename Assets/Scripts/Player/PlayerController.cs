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
    private PlayerAttackingMotor attackingMotor;

    private Transform currentNavTarget;
   
 
    // Start is called before the first frame update
    void Start()
    {
        
        cam = Camera.main;
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        stats = gameObject.GetComponent<CharacterStats>();
        interactionMotor = gameObject.GetComponent<PlayerInteractionMotor>();
        attackingMotor = gameObject.GetComponent<PlayerAttackingMotor>();
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

                if (hitInfo.transform.tag == "Player" && !GameObject.ReferenceEquals(hitInfo.transform.gameObject, this.gameObject))
                {   
                    //We've clicked a player. Attack him!
                    attackingMotor.SetAttackingFocus(hitInfo.collider.GetComponent<Attackable>());
                    //Follow if its a player (or other unit, but that will be later)
                    SetNavTarget(hitInfo.transform);

                }
                else
                {
                    
                    //If the object isn't interactable, then the focus will be null
                    interactionMotor.SetInteractingFocus(hitInfo.collider.GetComponent<Interactable>());
                   
                }

               

            }
        }

    }

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
    /// <param name="target"></param>
    private void SetNavTarget(Transform target)
    {
        currentNavTarget = target;
    }

    private void FollowNavTarget()
    {
        if(currentNavTarget != null)
        {
            navAgent.SetDestination(currentNavTarget.position);
          
        }
    }


}