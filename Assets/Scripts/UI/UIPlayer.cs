using System.Collections;
using TMPro;
using UnityEngine;

public class UIPlayer : MonoBehaviour
{
    //The stats of the player to use (not necessarily the local player, for instance
    //in the future we might want the player to be able to click on other units and see their inventory
    // like in DOTA for instance)
    private CharacterStats playerStats;
    //The team of the player
    //This is relevant because perhaps you do not want to display the enemy's inventories, even though the player clicks on them
    private Teams playerTeam;
    //Reference to the player controller 
    //(nice comment)
    private PlayerController playerController;

    //NOTE: In the future when (not if) we will create our own NetworkManager class and implement our own callbacks of OnClientStart and OnClientStop
    //which we will use to initialize / deinitialize / reinitialize the UI. Until then just use this
    private bool gameStarted = false;

     

    private ResourceInventory playerResourceInv;
    [SerializeField] private TextMeshProUGUI playerEnergyResourceText = null;
    private ResourceInventory teamResourceInv;
    [SerializeField] private TextMeshProUGUI teamEnergyResourceText = null;
    //Reference to the UI for buildings and constructions
    [SerializeField] private GameObject playerBuildingUI = null;


    // Start is called before the first frame update
    void Awake()
    {   
        //Initialize the UI components
        StartCoroutine(InitializeComponents());
    }

    IEnumerator InitializeComponents()
    {

        //Wait until the local player instance is set
        yield return new WaitUntil(() => GameManager.localPlayerInstance != null);
        //Get instance of the local player for starters!
        playerStats = GameManager.localPlayerInstance.GetComponent<CharacterStats>();
        playerController = GameManager.localPlayerInstance.GetComponent<PlayerController>();
        if (playerStats == null || playerController == null)
        {
            Debug.LogError("Local player instance couldn't be found!", this);
        }
        playerResourceInv = playerStats.GetComponent<ResourceInventory>();
        //Get the team of the player
        playerTeam = playerStats.team;
        //Check if the team is neutral (no resource manager for the neutral team) 
        if (playerTeam != Teams.Neutral)
        {
            //Get the relevant resource manager's inventory (the team's resource inventory)
            teamResourceInv = GameManager.GetResourceManager(playerTeam).GetComponent<ResourceInventory>();

            if (teamResourceInv == null)
            {
                Debug.LogError("Team Resource Inventory not found!");
            }
        }
        //Subscribe to a player event so that when the player pushes the hotkey for the Building UI this UI toggles (possibly based on proximity to base)
        playerController.BuildingUIToggle += ToggleBuildingUI;

        gameStarted = true;
       
    } 

    void LateUpdate()
    {
        //Check if the game is started, otherwise the components aren't initialized yet
        if(gameStarted == false)
        {
            return;
        }

        //Check for errors
        //If this is the case you might have to reinitialize components 
        if (playerEnergyResourceText == null || playerResourceInv == null || teamResourceInv == null)
        {

            gameStarted = false;
            StartCoroutine(InitializeComponents());
            return;
        }

        //Load the energy resource
        playerEnergyResourceText.text = playerResourceInv.GetResource(ResourceTypes.Energy).ToString();
        teamEnergyResourceText.text = teamResourceInv.GetResource(ResourceTypes.Energy).ToString();

    }

    //Toggles the building UI active / inactive
    private void ToggleBuildingUI(GameObject source)
    {
        //Toggle the UI
        playerBuildingUI.SetActive(!playerBuildingUI.activeSelf);
   
    }

}
