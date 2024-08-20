/**
 *
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 2022.3
 */

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour
{

#region Parameters
    [SerializeField] private Room toRoom;
#endregion 

#region Components
    private BoxCollider2D collider;
#endregion

#region Init & Destroy
    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }
#endregion Init

#region Update
    void Update()
    {
    }
#endregion Update

#region FixedUpdate
    void OnTriggerEnter2D(Collider2D collider)
    {
        CheckEnter(collider.gameObject);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        CheckEnter(collider.gameObject);
    }

    private void CheckEnter(GameObject obj)
    {
        // Only enter if the pivot point is inside
        if (collider.OverlapPoint(obj.transform.position))
        {
            EnterDoor();
        }
    }

    private void EnterDoor() 
    {
        LevelManager.Instance.EnterRoom(toRoom);
    }
#endregion FixedUpdate

#region Gizmos
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // Don't run in the editor
            return;
        }
    }
#endregion Gizmos
}
