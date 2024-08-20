/**
 *
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 2022.3
 */

using UnityEngine;
using WordsOnPlay.Utils;

public class Room : MonoBehaviour
{

#region Parameters
    [SerializeField] private Rect bounds;
#endregion 

#region Components
#endregion

#region State
#endregion

#region Properties
/// <summary>
/// Return the room boundary rectangle in world coordinates
/// </summary>
    public Rect Bounds     
    {
        get { return bounds.Transform(transform); }
    }
#endregion

#region Actions
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
    }
#endregion Init

#region Public Methods
/// <summary>
/// Test if a point (in world coordinates) is inside the room
/// </summary>
/// <param name="worldPos"></param>
/// <returns></returns>
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
