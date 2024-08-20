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
    [SerializeField] private Rect bounds;
#endregion 

#region Properties
    public Rect Bounds {
        get { return bounds.Transform(transform); }
    }
#endregion

#region Init & Destroy
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

#region Public methods
    public bool IsInside(Vector3 worldPos)
    {
        return bounds.Contains(transform.InverseTransformPoint(worldPos));
    }
#endregion

#region Update
    void Update()
    {
    }
#endregion Update

#region Gizmos
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        bounds.DrawGizmo(transform);
    }
#endregion Gizmos
}
