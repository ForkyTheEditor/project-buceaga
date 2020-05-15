using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Interactable))]
public class ResourceVein : NetworkBehaviour
{

    [SerializeField]
    private ResourceTypes _resourceType;

    public ResourceTypes resourceType { get { return _resourceType; } }

    private BoxCollider col;

    //A reference to the Interactable component of this GameObject
    private Interactable interactComponent;

    public int maxInteractingObjects = 1;

    private ParticleSystem miningEffect;
    private bool playedMiningEffect = false;
     

    private void Awake()
    { 
        interactComponent = gameObject.GetComponent<Interactable>();
        miningEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        col = gameObject.GetComponent<BoxCollider>();

        if(interactComponent != null)
        {
            //Subscribe to the Interacted event
            interactComponent.Interacted += OnDefaultInteract;
            //Subscribe to the StopInteracted event
            interactComponent.StopInteracted += OnStopInteract;

            interactComponent.maxInteractingObjects = this.maxInteractingObjects;
        }

        
    }

    public void OnDefaultInteract(GameObject source, EventArgs args)
    {

        //Mine the resource
        Debug.Log("Woohoo you are mining!");
        //Play the mining effect
        if(miningEffect != null && !playedMiningEffect)
        {
            Vector3 playerPos = source.transform.position;
            //Shoot a ray to hit the edge of the collider
            Ray ray = new Ray(playerPos, transform.position - playerPos);
            RaycastHit hitInfo;

            col.Raycast(ray, out hitInfo, 10);
            
            playedMiningEffect = true;

            //Play the mining effect on ALL the clients
            RpcPlayMiningEffect(playerPos, hitInfo.point);
            
            
        }

    }

    public void OnStopInteract(GameObject source, EventArgs args)
    {

        playedMiningEffect = false;
        //Stop it from all the clients!
        RpcStopMiningEffect();

    }


    [ClientRpc]
    void RpcPlayMiningEffect(Vector3 playerPos, Vector3 edgePos)
    {
        miningEffect.transform.LookAt(playerPos);
        miningEffect.transform.position = new Vector3(edgePos.x, playerPos.y, edgePos.z);
        miningEffect.Play();
    }

    [ClientRpc]
    void RpcStopMiningEffect()
    {
        miningEffect.Stop();
    }

}
