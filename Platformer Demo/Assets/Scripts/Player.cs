/**
 * A Singleton class providing access to the player gameobject
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 2022.3
 */

using UnityEngine;

public class Player : MonoBehaviour
{
#region Singleton
    private static Player instance;

    public static Player Instance {
        get { return instance; }
    }
#endregion

#region Init & Destroy
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is already a Player object in the scene.");
        }
        else 
        {
            instance = this;
        }
    }
#endregion 
}
