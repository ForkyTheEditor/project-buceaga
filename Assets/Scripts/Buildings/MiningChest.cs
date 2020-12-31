using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(Interactable))]
public class MiningChest : NetworkBehaviour
{
   
    //The resources in the chest
    private ResourceInventory chestInventory;
    //The component required for interacting
    private Interactable interactComponent;

    [SerializeField] private GameObject textUIGameobject;

    void Awake()
    {
        //Load the components
        interactComponent = gameObject.GetComponent<Interactable>();
        
        if(textUIGameobject == null)
        {
            Debug.LogError("Couldn't find the text UI child on " + gameObject.name + "!");

        }

        //Check for errors
        if (interactComponent == null)
        {
            Debug.LogError("Couldn't find the interactable component on " + gameObject.name + "!");
        }

        //Subscribe to the interacting events
        interactComponent.Interacted += EnableChestUI;
        interactComponent.StopInteracted += DisableChestUI;

    }

    void EnableChestUI(GameObject source, EventArgs args)
    {
        //Enable the UI
        if (textUIGameobject != null)
        {
            textUIGameobject.SetActive(true);
        }
    }

    void DisableChestUI(GameObject source, EventArgs args)
    {
        //Disable the UI
        if(textUIGameobject != null)
        {
            textUIGameobject.SetActive(false);

        }

    }


}
