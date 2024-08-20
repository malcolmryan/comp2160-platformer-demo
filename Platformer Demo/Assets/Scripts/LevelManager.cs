/**
 * Singleton object containing information about the current level.
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 2022.3
 */

using UnityEngine;
using WordsOnPlay.Utils;

public class LevelManager : MonoBehaviour
{

#region Singleton
    private static LevelManager instance;
    public static LevelManager Instance {
        get { return instance; }
    }
#endregion 

#region Parameters
    [SerializeField] private Room startingRoom;
#endregion

#region State
    private Room currentRoom;
#endregion 

#region Properties
    public Room CurrentRoom {
        get { return currentRoom; }
    }
#endregion

#region Init & Destroy
    void Awake()
    {
        currentRoom = startingRoom;
    }

    void OnEnable()
    {
        if (instance != null) 
        {
            Debug.LogError("There are multiple LevelManagers in the Scene.");
        }

        instance = this;
    }

    void OnDisable()
    {
        instance = null;
    }
#endregion Init

#region Public Methods
    public void EnterRoom(Room room)
    {
        currentRoom = room;
    }
#endregion

#region Update
    void Update()
    {
    }
#endregion Update

}
