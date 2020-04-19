using System;
using System.Collections;
using System.Collections.Generic;
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

    /// <summary>
    ///The default interaction of the object 
    /// </summary>
    public virtual void DefaultInteract()
    {
        if (Interacted != null)
        {
            Interacted(this, EventArgs.Empty);

        }
    }

    //Stops the last interaction
    //PROBABLY WILL NOT BE USED FOR NOW
    //public virtual void StopInteract()
    //{
    //    if(Interacted != null)
    //    {
    //        Interacted(this, EventArgs.Empty);
    //    }

    //}

    //TODO: Add other kinds of interacts: for instance DestructiveInteract for when players Attack click something etc.

}

