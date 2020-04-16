using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The interface of all interactable objects in the game
public interface IInteractable  
{
    //A reference to the GameObject this IInteractable is sitting on
    GameObject GO { get; }

    //The default interaction of the object
    void DefaultInteract();
    
    //TODO: Add other kinds of interacts: for instance DestructiveInteract for when players Attack click something etc.

}
