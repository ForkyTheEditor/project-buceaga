using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

//The component of all interactable objects in the game
public class Interactable : NetworkBehaviour
{
    
    public delegate void InteractionEventHandler(object source, EventArgs args);

    //The event function that handles interaction
    public event InteractionEventHandler Interacted;

    //Is the object available to interact with?
    //The actual implementantion of when the object is available to use should be left to the object class itself (as different objects may have different
    //behaviours regarding availability)
    public bool isAvailable = true;


    //A reference to the interacting objects
    public List<GameObject> interactingObjects;


    public int maxInteractingObjects = 1;



    /// <summary>
    ///The default interaction of the object; 
    ///Parameters: source - the GameObject that is interacting;
    /// </summary>
    public virtual void DefaultInteract(GameObject source)
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
            //Check if there are any subscribers to the event
            if (Interacted != null)
            {
                Interacted(source, EventArgs.Empty);
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
            Debug.Log("Stopped interacting");
        }
    }

    //TODO: Add other kinds of interacts: for instance DestructiveInteract for when players Attack click something etc.

}

