using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //This is a singleton object, check for other instances and DESTROY them
    private static GameManager instance;

    private static GameObject _localPlayerInstance = null;
    //Reference to the local player
    public static GameObject localPlayerInstance { get { return _localPlayerInstance; } }
    //The dictionary containing all the resource managers, sorted by their team
    private static Dictionary<Teams, GameObject> _resourceManagersInstances = new Dictionary<Teams, GameObject>();
    

    //INSTANTIATE THE OBJECT
    private void Awake()
    {
        //Check for other accidentally created instances and destroy them
        if(instance != null && instance != this)
        {
            //You do not matter, kill yourself. bitch.
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    /// <summary>
    /// Set the local player instance. This should only be called at the start by the instance itself
    /// </summary>
    /// <param name="newPlayer"></param>
    public static void SetLocalPlayer(GameObject newPlayer)
    {
        _localPlayerInstance = newPlayer;
    }
    /// <summary>
    /// Returns the resource manager for the specified team
    /// </summary>
    /// <param name="team">The team the manager belongs to</param>
    /// <returns></returns>
    public static GameObject GetResourceManager(Teams team)
    {
        return _resourceManagersInstances[team];
    }
    public static void SetResourceManager(Teams team, GameObject newManager)
    {
        _resourceManagersInstances[team] = newManager;
    }

}
