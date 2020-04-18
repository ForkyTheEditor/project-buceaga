using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class ResourceVein : MonoBehaviour
{
    public enum ResourceTypes { Energy }

    [SerializeField]
    private ResourceTypes _resourceType;

    public ResourceTypes resourceType { get { return _resourceType; } }


    private void Awake()
    {

        if(this.GetComponent<Interactable>() != null)
        {
            //Subscribe to the Interacted event
            this.GetComponent<Interactable>().Interacted += OnDefaultInteract;
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
