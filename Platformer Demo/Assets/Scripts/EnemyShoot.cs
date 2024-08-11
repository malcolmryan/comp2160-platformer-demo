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
using WordsOnPlay.Utils;

public class EnemyShoot : MonoBehaviour
{

#region Parameters
    [SerializeField] private Transform firePosition;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float rotateSpeed = 360; // deg / s
    [SerializeField] private float cooldown = 1; // s
    [SerializeField] private LayerMask hitLayer;
#endregion 

#region State
    private float cooldownTimer = 0;
    private bool isFiring = false;

    private RaycastHit2D hit;
#endregion

#region State
    public bool IsFiring
    {
        get { return isFiring; }
    }
#endregion

#region Init & Destroy
    void Awake()
    {
    }
#endregion Init

#region Update
    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        isFiring = CanSeePlayer();
        if (isFiring) 
        {
            Shoot();
        }
        else {
            Aim();
        }
    }

    private bool CanSeePlayer()
    {
        if (Player.Instance == null) 
        {
            return false;   // no player in the scene
        }

        Vector2 origin = firePosition.position;
        Vector2 dir = transform.right;
        hit = Physics2D.Raycast(origin, dir, float.PositiveInfinity, hitLayer);

        if (hit.collider == null)
        {
            return false;
        }
        else 
        {
            // check if we hit the player
            return (hit.collider.gameObject.CompareTag(Tags.PLAYER));
        }
    }

    private void Aim()
    {
        if (Player.Instance == null) 
        {
            return; // no player in the scene
        }

        Vector2 dir = Player.Instance.transform.position - transform.position;
        float angle = Vector2.SignedAngle(transform.right, dir); // degrees - not signed
    
        float rotate = rotateSpeed * Time.deltaTime;
        rotate = Mathf.Min(rotate, Mathf.Abs(angle));
        rotate *= Mathf.Sign(angle);

        transform.Rotate(0,0,rotate);
    }

    private void Shoot() 
    {
        if (cooldownTimer > 0)
        {
            return;
        }

        cooldownTimer = cooldown;
        Vector3 position = firePosition.position;
        Quaternion rotation = firePosition.rotation;
        Bullet bullet = Instantiate(bulletPrefab, position, rotation);        
    }

#endregion Update

#region Gizmos
    void OnDrawGizmos()
    {
        if (hit.collider == null)
        {
            return;
        }
        
        Gizmos.color = (isFiring ? Color.green : Color.red);
        Gizmos.DrawLine(transform.position, hit.point);
    }
#endregion Gizmos
}
