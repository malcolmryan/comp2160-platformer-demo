using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WordsOnPlay.Utils;

public class Bullet : MonoBehaviour
{
#region Parameters
    [SerializeField] private float speed = 10;
    [SerializeField] private float radius = 0.1f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private LayerMask bounceLayerMask;
    [SerializeField] private LayerMask hitLayerMask;
#endregion

#region Constants
    private float HIT_DISTANCE_BACKOFF = 0.05f;
#endregion

#region State
    private float fireTime;
#endregion


#region Init
    void Awake()
    {
        fireTime = Time.time;
    }
#endregion

#region Update
    void Update()
    {
        if (Time.time > fireTime + lifetime)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 pos = (Vector2)transform.position;
        Vector2 dir = (Vector2)transform.right;
        float dist = speed * Time.deltaTime;
        LayerMask layerMask = bounceLayerMask | hitLayerMask;

        RaycastHit2D hit = Physics2D.CircleCast(pos, radius, dir, dist, layerMask);

        if (hit.collider != null) 
        {
            if (bounceLayerMask.Contains(hit.collider.gameObject)) 
            {
                Bounce(hit);
            }
            else
            {
                Hit(hit);
            }
        }
        else
        {
            transform.Translate(dist * Vector3.right, Space.Self);
        }
    }

    private void Bounce(RaycastHit2D hit) 
    {
        // move to the point of collision (not quite    )
        float distance = Mathf.Max(0, hit.distance - HIT_DISTANCE_BACKOFF);
        transform.Translate(distance * Vector3.right, Space.Self);
        
        // rotate to face in the relected direction
        Vector2 v = transform.right;
        v = v.Project(hit.normal);
        Vector2 reflected = (Vector2)transform.right - 2 * v;
        transform.localRotation = Quaternion.FromToRotation(Vector3.right, reflected);
    }

    private void Hit(RaycastHit2D hit) 
    {
        GameObject o = hit.collider.gameObject;

        if (o.CompareTag(Tags.ENEMY))
        {
            EnemyHealth enemy = o.GetComponent<EnemyHealth>();
            enemy.Hit();
        }

        Destroy(gameObject);
    }
#endregion

}

