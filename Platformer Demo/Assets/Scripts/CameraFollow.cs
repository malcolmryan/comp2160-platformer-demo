/**
 * Script to allow the camera to follow a target.
 * This script is usually applied to an empty parent object and the Camera is attached as a child
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 2022.3
 */

using UnityEngine;
using WordsOnPlay.Utils;

public class CameraFollow : MonoBehaviour
{

#region Parameters
    [SerializeField] private Transform player;
    [SerializeField] private Transform reticle;
    [SerializeField] private float percent = 0.5f;
    [SerializeField] private AnimationCurve curve;
#endregion 

#region Init & Destroy
    void Start()
    {
        Follow();
    }
#endregion Init

#region Update
    void LateUpdate()
    {
        Follow();
    }

    private void Follow()
    {
        if (player != null)
        {
            // aim for a point in between the player and reticle
            Vector3 p = Vector3.Lerp(player.position, reticle.position, percent);

            // move towards p with speed determined by the animation curve
            Vector3 delta = p - transform.position;
            float speed = curve.Evaluate(delta.magnitude);
            transform.position = transform.position + speed * delta * Time.deltaTime;

            KeepInBounds();
        }
    }

    private void KeepInBounds()
    {
        // check the camera does not see outside the level boundaries
        Rect bounds = LevelManager.Instance.CurrentRoom.Bounds;

        Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);
        if (!bounds.Contains(bottomLeft)) 
        {
            Vector2 q = bounds.Clamp(bottomLeft);
            Vector3 delta = q - bottomLeft;
            transform.position = transform.position + delta;
        }

        Vector2 topRight = Camera.main.ViewportToWorldPoint(Vector2.one);
        if (!bounds.Contains(topRight)) 
        {
            Vector2 q = bounds.Clamp(topRight);
            Vector3 delta = q - topRight;
            transform.position = transform.position + delta;
        }
    }
#endregion Update

#region Gizmos
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(player.transform.position, reticle.transform.position);
        Vector3 p = Vector3.Lerp(player.position, reticle.position, percent);
        Gizmos.DrawSphere(p, 0.1f);
    }

#endregion
}
