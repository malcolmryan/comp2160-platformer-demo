using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float speedWindow = 0.6f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float gravity = 33f;
    [SerializeField] private Transform leftRaycast;
    [SerializeField] private Transform rightRaycast;
    [SerializeField] private float raycastDist = 0.5f;
    [SerializeField] private LayerMask floorLayer;

    // Actions
    private Actions actions;
    private InputAction moveAction;
    private InputAction jumpAction;

    // Components
    new private Rigidbody2D rigidbody;
    new private BoxCollider2D collider;

    // State
    private Vector2 movementDir;
    private List<ContactPoint2D> contacts;
    private bool wasOnGround = false;

#region Init
    void Awake()
    {
        actions = new Actions();
        moveAction = actions.movement.move;
        jumpAction = actions.movement.jump;

        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        contacts = new List<ContactPoint2D>();
    }

    void OnEnable()
    {
        actions.movement.Enable();
    }

    void OnDisable()
    {
        actions.movement.Disable();
    }
#endregion Init

#region Update

    void Update()
    {
        movementDir = moveAction.ReadValue<Vector2>();
    }

#endregion Update

#region FixedUpdate

    private bool OnGround()
    {
        for (int i = 0; i < contacts.Count; i++)
        {
            if (contacts[i].normal.y > 0)
            {
                return true;
            }
        }
        return false;
    }

    void FixedUpdate()
    {        
        rigidbody.GetContacts(contacts);
        bool onGround = OnGround();

        if (wasOnGround && !onGround) 
        {
            onGround = TeleportToGround();
        }

        float currentSpeed = rigidbody.velocity.x;
        float targetSpeed = maxSpeed * movementDir.x;

        float dv = targetSpeed - currentSpeed;
        float sign = Mathf.Sign(dv);
        dv = Mathf.Abs(dv);
        float a = Mathf.Min(acceleration, dv/Time.fixedDeltaTime);

        ContactPoint2D? furthestContact = null;
        float max = Mathf.NegativeInfinity;
        for (int i = 0; i < contacts.Count; i++)
        {
            if (contacts[i].normal.y > 0)
            {
                Vector2 v =  contacts[i].point - rigidbody.position;
                if (v.x * sign > max)
                {
                    furthestContact = contacts[i];
                    max = v.x * sign;
                }
            }
        }

        Vector2 dir = Vector2.right;
        if (furthestContact != null)
        {
            // a vector perpendicular to the normal with dir.x = 1
            Vector2 n = furthestContact.Value.normal;
            dir.y = -n.x / n.y;
        }

        rigidbody.AddForce(a * sign * rigidbody.mass * dir);

        if (!onGround) 
        {
            rigidbody.AddForce(gravity * rigidbody.mass * Vector2.down);
        }

        wasOnGround = onGround;
    }

    private bool TeleportToGround()
    {
        Vector2 centre = rigidbody.position + collider.offset;
        Vector2 size = collider.size;

        RaycastHit2D hit = Physics2D.BoxCast(centre, size, 0,  Vector2.down, raycastDist, floorLayer);

        if (hit.collider != null)
        {            
            // teleport to the ground
            rigidbody.position = rigidbody.position + hit.distance * Vector2.down;
            
            // zero the vertical velocity
            Vector2 v = rigidbody.velocity;
            v.y = Mathf.Min(0, v.y);;
            rigidbody.velocity = v;

            return true;
        }
        else 
        {
            return false;
        }

    }

#endregion FixedUpdate

#region Gizmos
    void OnDrawGizmos()
    {
        if (contacts == null)
        {
            return;
        }

        for (int i = 0; i < contacts.Count; i++)
        {
            ContactPoint2D c = contacts[i];
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(c.point, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(c.point, c.point + c.normal);
        }
    }
#endregion Gizmos


}
