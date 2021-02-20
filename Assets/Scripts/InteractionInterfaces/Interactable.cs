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
    /// <summary>
    /// Describes the interaction type of the object. Each interaction type has two corresponding events: StartInteraction and StopInteraction
    /// Tick: Interacts each tick (multiple times per second)
    /// Single: Interacts once
    /// UI: Locally (as opposed to over the network, like the other ones) interacts a single time; this is usually used to bring up the UI of the object (hence, UI)
    /// </summary>
    public enum InteractionType { Tick, Single, UI }

    public InteractionType interactionType = InteractionType.Tick;

    public delegate void InteractionEventHandler(GameObject source, EventArgs args);

    //The event function that notifies that the player started interacting each tick / frame.
    public event InteractionEventHandler TickInteracted;
    //The event function that notifies that the player stopped interacting each tick / frame.
    public event InteractionEventHandler StopTickInteracted;

    //The event function that notifies that the player interacted once with the object.
    public event InteractionEventHandler SingleInteracted;
    //The event function that notifies that the player stopped his one time interaction with the object.
    public event InteractionEventHandler StopSingleInteracted;

    //The event function that notifies that the player interacted with the UI of the object.
    public event InteractionEventHandler UIInteracted;
    //The event function that notifies that the player stopped his interaction with the UI of the object.
    public event InteractionEventHandler StopUIInteracted;

    //Is the object available to interact with?
    //The actual implementantion of when the object is available to use should be left to the object class itself (as different objects may have different
    //behaviours regarding availability)
    public bool isAvailable = true;

    //A reference to the interacting objects
    public List<GameObject> interactingObjects;

    public int maxInteractingObjects = 1;

    private void Update()
    {
        //Only run the interaction on the server
        //Then notify the players of the consequences
        if (!isServer)
        {
            return;
        }

        //If the object is available, interact each tick with the objects
        if (isAvailable)
        {
            foreach (GameObject go in interactingObjects)
            {
                if (TickInteracted != null)
                {
                    TickInteracted(go, EventArgs.Empty);
                }
            }

        }
    }

    /// <summary>
    ///The default interaction of the object; 
    ///Parameters: source - the GameObject that is interacting;
    /// </summary>
    [Command (ignoreAuthority = true)]
    public virtual void CmdStartTickInteract(GameObject source)
    {

        if (isAvailable)
        {
            Predicate<GameObject> compareInstanceID = (go) => go.GetInstanceID() == source.GetInstanceID();

            if (interactingObjects.Count < maxInteractingObjects)
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
    [Command(ignoreAuthority = true)]
    public virtual void CmdStopTickInteract(GameObject source)
    {
        Predicate<GameObject> compareInstanceID = (go) => go.GetInstanceID() == source.GetInstanceID();

        //Remove the object that stopped interacting
        int index = interactingObjects.FindIndex(compareInstanceID);
        if (index >= 0)
        {
            interactingObjects.RemoveAt(index);

            if (StopTickInteracted != null)
            {
                StopTickInteracted(source, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Interacts with the object a single time.
    /// </summary>
    /// <param name="source"></param>
    [Command(ignoreAuthority = true)]
    public virtual void CmdStartSingleInteract(GameObject source)
    {
        if(interactingObjects.Count < maxInteractingObjects)
        {
            SingleInteracted(source, EventArgs.Empty);
        }
    }
    /// <summary>
    /// Stops the single interaction with an object.
    /// </summary>
    /// <param name="source"></param>
    [Command(ignoreAuthority = true)]
    public virtual void CmdStopSingleInteract(GameObject source)
    {
        StopSingleInteracted(source, EventArgs.Empty);
    }

    /// <summary>
    /// Interacts with the object a single time, for the purpose of UI enabling.
    /// </summary>
    /// <param name="source"></param>
    public virtual void StartUIInteract(GameObject source)
    {
        if (interactingObjects.Count < maxInteractingObjects)
        {
            UIInteracted(source, EventArgs.Empty);
        }
    }
    /// <summary>
    /// Stops the single interaction with the object's UI.
    /// </summary>
    /// <param name="source"></param>
    public virtual void StopUIInteract(GameObject source)
    {
        StopUIInteracted(source, EventArgs.Empty);
    }

    /// <summary>
    ///Calls the default interaction type for the object.
    ///For instance if the object is usually used in a tick based interaction, it calls that specific function
    /// </summary>
    public virtual void StartDefaultInteract(GameObject source)
    {
        //Check interaction type of this object
        switch (interactionType)
        {
            case InteractionType.Tick:
                CmdStartTickInteract(source);
                break;
            case InteractionType.Single:
                CmdStartSingleInteract(source);
                break;
            case InteractionType.UI:
                StartUIInteract(source);
                break;

        }
    }

    /// <summary>
    /// Stops the default type of interaction with this object
    /// </summary>
    /// <param name="source"></param>
    public virtual void StopDefaultInteract(GameObject source)
    {
        switch (interactionType)
        {
            case InteractionType.Tick:
                CmdStopTickInteract(source);
                break;
            case InteractionType.Single:
                CmdStopSingleInteract(source);
                break;
            case InteractionType.UI:
                StopUIInteract(source);
                break;
        }
    }

    //TODO: Add other kinds of interacts: for instance DestructiveInteract for when players Attack click something etc.

}