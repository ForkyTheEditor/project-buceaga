using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceTypes { Energy }

[RequireComponent(typeof(Interactable))]
public class ResourceVein : MonoBehaviour
{

    [SerializeField]
    private ResourceTypes _resourceType;

    public ResourceTypes resourceType { get { return _resourceType; } }

    //A reference to the Interactable component of this GameObject
    private Interactable interactComponent;

    
      

    private void Awake()
    {
        interactComponent = this.GetComponent<Interactable>();

        if(interactComponent != null)
        {
            //Subscribe to the Interacted event
            interactComponent.Interacted += OnDefaultInteract;
        }

    }

    public void OnDefaultInteract(object source, EventArgs args)
    {
        //Mine the resource
        Debug.Log("Woohoo you are mining!");
        
    }

    public void OnStopInteract(object source, EventArgs args)
    {

        

    }

}
