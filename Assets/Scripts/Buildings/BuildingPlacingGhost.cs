using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameObject that tracks mouse pointer and checks for correct placement of a building. Gets destroyed once the player actually places the building
/// </summary>
public class BuildingPlacingGhost : MonoBehaviour
{

    private Camera mainCamera;
    private MeshRenderer renderer;

    [SerializeField] private Material correctLocationMat;   //Material for when the ghost is in an correct place
    [SerializeField] private Material incorrectLocationMat; //Material for when the ghost is in a incorrect place
    [SerializeField] private bool canBePlaced = true; //Can this object be placed in its current location?

    void Awake()
    {
        renderer = gameObject.GetComponent<MeshRenderer>();
        mainCamera = Camera.main;

        renderer.material = correctLocationMat;
    }

    void FixedUpdate()
    {
        //Track the position of the mouse pointer, but don't go out of bounds and always stay above the ground
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();

        ///TODO
        ////Hit only the ground (can only be placed on the ground! maybe for exceptions will revisit the code) (we don't want building hats...or do we?)
        LayerMask groundLayer = LayerMask.GetMask("Ground");

        if (Physics.Raycast(ray, out hitInfo, 100f, groundLayer))
        {
            //Set the ghost at the specified position plus a small vertical offset so it doesn't clip
            transform.position = hitInfo.point + Vector3.up * 0.5f;
        }

    }

    //If a collider enters the ghost, then it can't be placed
    private void OnTriggerEnter(Collider other)
    {
        //Change material to the incorrect place
        renderer.material = incorrectLocationMat;

        canBePlaced = false;

    }

    //If the ghost doesn't touch anything, then it's ready to place
    private void OnTriggerExit(Collider other)
    {
        //Change material to the correct place
        renderer.material = correctLocationMat;

        canBePlaced = true;
    }

}
