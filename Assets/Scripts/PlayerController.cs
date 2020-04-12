using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Forces the GameObject to attach a NavMeshAgent component
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    //An reference to the main camera 
    Camera cam;
    //A reference to the NavMeshAgent
    NavMeshAgent navAgent;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        navAgent = this.GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        //If the player right clicks somewhere on the map, start moving towards that point
        if (Input.GetMouseButtonDown(1))
        {
            //Shoot a ray through the screen at the position of the cursor
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                //Very basic movement for now
                navAgent.SetDestination(hitInfo.point);

            }
        }
    }
}
