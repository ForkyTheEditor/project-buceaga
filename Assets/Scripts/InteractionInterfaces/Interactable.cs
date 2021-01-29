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

    //A reference to the local interacting objects (the objects which are interacting with this entity on this client)
    //There may be entities interacting with this object on other clients => don't let any client interact with this object, but let each client handle its own interactions)
    private List<GameObject> localInteractingObjects = new List<GameObject>();

    [SyncVar]
    private int networkCurrentInteractingObjects = 0; //The amount of entities interacting with this object on the network

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
            foreach(GameObject go in localInteractingObjects)
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
    
            if(networkCurrentInteractingObjects < maxInteractingObjects)
            {
                //There's still place for people to interact
                //Update the network number of objects
                CmdSetNetworkInteractingObjects(networkCurrentInteractingObjects + 1);


                //Check if the source of the interaction is in the interacting list
                if (!localInteractingObjects.Exists(compareInstanceID)) //did i mention i goddamn love c#
                {
                    //The player isn't registered in the interactingObjects list
                    //Register it
                    localInteractingObjects.Add(source);

                }
            }
            else
            {
                //The maximum limit has been reached
                //Check if the source is not registered
                if (!localInteractingObjects.Exists(compareInstanceID))
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
        int index = localInteractingObjects.FindIndex(compareInstanceID);
        if(index >= 0)
        {
            //Update the network interacting objects
            CmdSetNetworkInteractingObjects(networkCurrentInteractingObjects - 1);


            localInteractingObjects.RemoveAt(index);

            if(StopInteracted != null)
            {
                StopInteracted(source, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Set the current number of interacting objects (anywhere in the network).
    /// </summary>
    /// <param name="nr"></param>
    [Command(ignoreAuthority = true)]
    public void CmdSetNetworkInteractingObjects(int nr)
    {
        //Check if the current interacting objects are not already maxxed out.
        if(networkCurrentInteractingObjects >= maxInteractingObjects)
        {
            return;
        }

        if(nr > 0 && nr <= maxInteractingObjects)
        {
            networkCurrentInteractingObjects = nr;
        }
    }

    /// <summary>
    /// Returns the current number of interacting objects (anywhere in the network). 
    /// </summary>
    /// <returns></returns>
    public int GetNetworkInteractingObjects()
    {
        return networkCurrentInteractingObjects;
    }


    //TODO: Add other kinds of interacts: for instance DestructiveInteract for when players Attack click something etc.

}

