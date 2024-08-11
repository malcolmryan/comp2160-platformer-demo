/**
 * Movement code for an enemy that patrols back and forth along a series of control points.
 *
 * Note: movement is implemented using a kinematic rigidbody, so it unaffected by physics.
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 2022.3
 */

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{

#region Parameters
    [SerializeField] private float speed = 3f;  // m/s
    [SerializeField] private float pause = 0.5f; // s
    [SerializeField] private Transform patrol;    
    [SerializeField] private EnemyShoot gun;
#endregion 

#region Components
    new private Rigidbody2D rigidbody;
#endregion

#region State
    private float pauseTimer = 0;
    private int nextPatrol = 0;
    private int patrolDir = 1;
#endregion

#region Init & Destroy
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic;

        // detatch patrol points
        patrol.parent = null;
        patrol.gameObject.name = $"{gameObject.name} Patrol";

        if (patrol.childCount <= 1)
        {
            // disable the component with a warning
            Debug.LogWarning($"{gameObject.name} patrol path has fewer than 2 waypoints");
            this.enabled = false;
        }

        transform.position = patrol.GetChild(0).position;
        nextPatrol = 1;
    }
#endregion Init

#region Update
    void Update()
    {
        if (gun.IsFiring)
        {
            pauseTimer = pause;
        }
        else if (pauseTimer > 0)
        {
            pauseTimer -= Time.deltaTime;
        }
        else
        {
            Move();
        }
    }

    private void Move()
    {
        float distanceToMove = speed * Time.deltaTime;

        // move towards the next patrol point (without overshooting)
        Transform next = patrol.GetChild(nextPatrol);
        Vector2 delta = next.position - transform.position;

        if (delta.magnitude < distanceToMove) 
        {
            // We've hit the patrol point. Pause then continue to the next point.
            // Note that this assumes there are at least 2 patrol points.

            transform.position = next.position;
            pauseTimer = pause;

            nextPatrol += patrolDir;

            if (nextPatrol < 0) 
            {
                patrolDir = 1;
                nextPatrol = 1;
            }
            if (nextPatrol >= patrol.childCount)
            {
                patrolDir = -1;
                nextPatrol = patrol.childCount - 2;
            }
        }
        else 
        {
            transform.Translate(speed * Time.deltaTime * delta.normalized);
        }
    }
#endregion Update

#region Gizmos
    void OnDrawGizmos()
    {

    }
#endregion Gizmos
}
