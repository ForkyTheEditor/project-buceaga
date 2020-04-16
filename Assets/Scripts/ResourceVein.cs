using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceVein : MonoBehaviour, IInteractable
{
    public enum ResourceTypes { Energy }

    [SerializeField]
    private ResourceTypes _resourceType;

    public ResourceTypes resourceType { get { return _resourceType; } }

    public GameObject GO { get { return this.gameObject; } }

    public void DefaultInteract()
    {
        //Mine the resource
        Debug.Log("Woohoo you are mining!");

    }

}
