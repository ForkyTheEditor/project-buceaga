using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
//The component of all interactable objects in the game
public class Interactable : NetworkBehaviour
{
    public NetworkIdentity networkId;

    public delegate void InteractionEventHandler(GameObject source, EventArgs args);

    //The event function that notifies that the player is interacting
    public event InteractionEventHandler Interacted;
    //The event function that notifies that the player stopped interacting
    public event InteractionEventHandler StopInteracted;

    //Is the object available to interact with?
    //The actual implementantion of when the object is available to use should be left to the object class itself (as different objects may have different
    //behaviours regarding availability)
    public bool isAvailable = true;

    //A reference to the interacting objects
    public List<GameObject> interactingObjects;

    public int maxInteractingObjects = 1;

    private void Awake()
    {
        networkId = gameObject.GetComponent<NetworkIdentity>();
    }

    private void Update()
    {
        //Only run the interaction on the server
        //Then notify the players of the consequences
        if (!isServer)
        {
            return;
        }
       

        if (isAvailable)
        {
            foreach(GameObject go in interactingObjects)
            {   
                if(Interacted != null)
                {
                    Interacted(go, EventArgs.Empty);
                }
            }

        }
    }

    /// <summary>
    ///The default interaction of the object; 
    ///Parameters: source - the GameObject that is interacting;
    /// </summary>
    public virtual void StartDefaultInteract(GameObject source)
    {
       
        if (isAvailable)
        {
            Predicate<GameObject> compareInstanceID = (go) => go.GetInstanceID() == source.GetInstanceID();
    
            if(interactingObjects.Count < maxInteractingObjects)
            {
                //There's still place for people to interact

                //Check if the source of the interaction is in the interacting list
                if (!interactingObjects.Exists(compareInstanceID)) //did i mention i goddamn love c#
                {
                    //The player isn't registered in the interactingObjects list
                    //Register it
                    interactingObjects.Add(source);

                }
            }
            else
            {
                //The maximum limit has been reached
                //Check if the source is not registered
                if (!interactingObjects.Exists(compareInstanceID))
                {
                    return;
                }
            }
            
        }
        else
        {
            Debug.Log(this.gameObject.name + " is not available for interaction!");
        }     
    }

    ///<summary>
    ///Must be called when the Player stops interacting with the object;
    ///Parameters: source - the GameObject that stopped interacting;
    /// </summary>
    public virtual void StopInteract(GameObject source)
    {
        Predicate<GameObject> compareInstanceID = (go) => go.GetInstanceID() == source.GetInstanceID();

        //Remove the object that stopped interacting
        int index = interactingObjects.FindIndex(compareInstanceID);
        if(index >= 0)
        {
            interactingObjects.RemoveAt(index);

            if(StopInteracted != null)
            {
                StopInteracted(source, EventArgs.Empty);
            }
        }
    }

    //TODO: Add other kinds of interacts: for instance DestructiveInteract for when players Attack click something etc.

}

