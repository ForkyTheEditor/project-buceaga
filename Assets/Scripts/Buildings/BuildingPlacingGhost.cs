using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

/// <summary>
/// GameObject that tracks mouse pointer and checks for correct placement of a building. Gets destroyed once the player actually places the building
/// </summary>
public class BuildingPlacingGhost : NetworkBehaviour
{

    private Camera mainCamera;
    private MeshRenderer renderComponent;

    [SerializeField] private Material correctLocationMat;   //Material for when the ghost is in an correct place
    [SerializeField] private Material incorrectLocationMat; //Material for when the ghost is in a incorrect place
    [SerializeField] private bool canBePlaced = true;       //Can this object be placed in its current location?
    [SerializeField] private GameObject actualBuildingObject; //The actual building this object represents

    void Awake()
    {
        renderComponent = gameObject.GetComponent<MeshRenderer>();
        mainCamera = Camera.main;

        renderComponent.material = correctLocationMat;
    }

    private void Update()
    {
        //If the user left clicks and the building can be placed, it will spawn the actual building and destroy this ghost
        if (canBePlaced && Input.GetMouseButtonDown(0))
        {
            //Spawn the object from the server
            SpawnBuilding();

            Destroy(this.gameObject);

        }
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

    private void SpawnBuilding()
    {
        //Take position, rotation and scale from ghost object
        Transform buildingTransform = this.transform;

        PlayerNetworkObject networkObj = GameManager.localPlayerNetworkInstance.GetComponent<PlayerNetworkObject>();

        if (networkObj == null)
        {
            Debug.LogError("Player Network Object couldn't be found!");

        }

        //Send a command to the player network object to spawn my prefab
        networkObj.SpawnObjectNoAuthority(GameManager.spawnIDMap.GetID(actualBuildingObject), buildingTransform.position, buildingTransform.rotation);
        

    }

    //If a collider enters the ghost, then it can't be placed
    private void OnTriggerEnter(Collider other)
    {
        //Change material to the incorrect place
        renderComponent.material = incorrectLocationMat;

        canBePlaced = false;

    }

    //If the ghost doesn't touch anything, then it's ready to place
    private void OnTriggerExit(Collider other)
    {
        //Change material to the correct place
        renderComponent.material = correctLocationMat;

        canBePlaced = true;
    }

}
