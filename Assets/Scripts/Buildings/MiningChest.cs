using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(ResourceInventory))]
public class MiningChest : NetworkBehaviour
{
   
    //The resources in the chest
    private ResourceInventory chestInventory;
    //The component required for interacting
    private Interactable interactComponent;

    //The inventory of the player interacting with the chest
    private ResourceInventory interactingInventory = null;

    [SerializeField] private GameObject textUIGameobject = null;

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

        //Set the interaction type to UI
        interactComponent.interactionType = Interactable.InteractionType.UI;
        //Subscribe to the interacting events
        interactComponent.UIInteracted += EnableChest;
        interactComponent.StopUIInteracted += DisableChest;

        //Cache the resource inventory 
        chestInventory = gameObject.GetComponent<ResourceInventory>();

        if(chestInventory == null)
        {
            Debug.LogError("Couldn't find the mining chest's resource inventory!");
        }

    }

    void EnableChest(GameObject source, EventArgs args)
    {
        //Enable the UI
        if (textUIGameobject != null)
        {
            textUIGameobject.SetActive(true);
        }

        interactingInventory = source.GetComponent<ResourceInventory>();

    }

    void DisableChest(GameObject source, EventArgs args)
    {
        //Disable the UI
        if(textUIGameobject != null)
        {
            textUIGameobject.SetActive(false);

        }

        interactingInventory = null;
    }

    /// <summary>
    /// Takes half the resources (of the specified type) in the player's inventory. Puts them in the chest.
    /// </summary>
    /// <param name="type"></param>
    public void PlaceHalfResources(ResourceTypes type)
    {
        chestInventory.AddResource(type, interactingInventory.TakeResource(type, interactingInventory.GetResource(type) / 2));

    }

    /// <summary>
    /// Takes all the resources (of the specified type) in the player's inventory. Puts them in the chest.
    /// </summary>
    /// <param name="type"></param>
    public void PlaceAllResources(ResourceTypes type)
    {
        chestInventory.AddResource(type, interactingInventory.TakeResource(type, interactingInventory.GetResource(type)));
    }

    /// <summary>
    /// Takes half the resources (of the specified type) in the chest. Puts them in the interacting player's inventory.
    /// </summary>
    /// <param name="type"></param>
    public void TakeHalfResources(ResourceTypes type)
    {    
        //Take the resource from the chest and put them in the interacting inventory
        interactingInventory.AddResource(type, chestInventory.TakeResource(type, chestInventory.GetResource(type) / 2));

    }

    /// <summary>
    /// Takes all the resources (of the specified type) in the chest. Puts them in the interacting player's inventory.
    /// </summary>
    /// <param name="type"></param>
    public void TakeAllResources(ResourceTypes type)
    {
        //Take the resource from the chest and put them in the interacting inventory
        interactingInventory.AddResource(type, chestInventory.TakeResource(type, chestInventory.GetResource(type)));
        
    }
    
    /// <summary>
    /// This function is here because Unity UI button functions can't have Enum parameters. This does nothing but convert the int to ResourceTypes enum.
    /// </summary>
    /// <param name="type"></param>
    public void TakeHalfResourcesInt(int type)
    {
        TakeHalfResources((ResourceTypes)type);
    }

    /// <summary>
    /// This function is here because Unity UI button functions can't have Enum parameters. This does nothing but convert the int to ResourceTypes enum.
    /// </summary>
    /// <param name="type"></param>
    public void TakeAllResourcesInt(int type)
    {
        TakeAllResources((ResourceTypes)type);
    }

    /// <summary>
    /// This function is here because Unity UI button functions can't have Enum parameters. This does nothing but convert the int to ResourceTypes enum.
    /// </summary>
    /// <param name="type"></param>
    public void PlaceHalfResourcesInt(int type)
    {
        PlaceHalfResources((ResourceTypes)type);
    }

    /// <summary>
    /// This function is here because Unity UI button functions can't have Enum parameters. This does nothing but convert the int to ResourceTypes enum.
    /// </summary>
    /// <param name="type"></param>
    public void PlaceAllResourcesInt(int type)
    {
        PlaceAllResources((ResourceTypes)type);
    }
}
