using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// GameObject that tracks mouse pointer and checks for correct placement of a building. Gets placed on the ground once player starts building the building.
/// At the end of the build cycle the proper building gets placed
/// </summary>
[RequireComponent(typeof(BuildingGridSnapper))]
public class BuildingPlacingGhost : NetworkBehaviour
{

    private Camera mainCamera;
    private MeshRenderer renderComponent;
    private GameObject playerObject;
    private ResourceInventory playerInventory; //The inventory of the player building this building

    [SerializeField] private Material correctLocationMat = null;   //Material for when the ghost is in an correct place
    [SerializeField] private Material incorrectLocationMat = null; //Material for when the ghost is in a incorrect place
    [SerializeField] private bool canBePlaced = true;       //Can this object be placed in its current location?
    private bool placed = false; //Is the ghost actually placed?
    [SerializeField] private GameObject actualBuildingObject = null; //The actual building this object represents
    
    [SerializeField] private float buildingTime; //The time necessary for this building to be done
    [SerializeField] private ResourceAmount[] amounts; //The resources needed for building to be built

    private Coroutine buildingCoroutine; //The coroutine responsible for the animation and building of the actual building; cache this to be able to cancel it
    
    void Awake()
    {
        renderComponent = gameObject.GetComponent<MeshRenderer>();
        mainCamera = Camera.main;

        renderComponent.material = correctLocationMat;

        //Get the local player instance and their inventory
        playerObject = GameManager.localPlayerInstance;
        playerInventory = playerObject.GetComponent<ResourceInventory>();
    }

    private void Update()
    {
        //If the ghost is already placed, count down the timer
        if (placed)
        {
            return;
        }

        //If the user left clicks and the building can be placed, it will spawn the actual building and destroy this ghost
        if (canBePlaced && Mouse.current.leftButton.wasPressedThisFrame && CheckEnoughResources())
        {
            placed = true;

            //Start building the actual building
            buildingCoroutine = StartCoroutine(StartBuilding());
        }
    }

    void FixedUpdate()
    {
        //Check if the ghost has already been placed on the ground
        if (placed)
        {
            return;
        }


        //Track the position of the mouse pointer, but don't go out of bounds and always stay above the ground
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
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

    private bool CheckEnoughResources()
    {
        //Check if the player has the required amounts for every resource
        foreach(ResourceAmount amount in amounts)
        {
            if(playerInventory.GetResource(amount.type) < amount.amount)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Start building the building (you can cancel it OR be interrupted, the proper building only gets spawned at the end)
    /// </summary>
    private IEnumerator StartBuilding()
    {
        //Start playing the building animation
        //Use the animation to figure out when to spawn the actual building?

        //For now just start the timer until the building gets spawned
        //The entire resource amount gets pulled when the building gets built
        yield return new WaitForSeconds(buildingTime);

        foreach(ResourceAmount amount in amounts)
        {
            playerInventory.TakeResource(amount.type, amount.amount);
        }

        SpawnBuilding();
    }
    
    /// <summary>
    /// Cancels the building. Destroys the building ghost.
    /// </summary>
    private void CancelBuilding()
    {
        StopCoroutine(buildingCoroutine);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Spawns the actual building and destroys the ghost.
    /// </summary>
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

        Destroy(this.gameObject);
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