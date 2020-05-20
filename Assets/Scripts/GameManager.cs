using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //This is a singleton object, check for other instances and DESTROY them
    private static GameManager instance;

    private static GameObject _localPlayerInstance = null;
    public static GameObject localPlayerInstance { get { return _localPlayerInstance; } }

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


    public static void SetLocalPlayer(GameObject newPlayer)
    {
        _localPlayerInstance = newPlayer;
    }

}
