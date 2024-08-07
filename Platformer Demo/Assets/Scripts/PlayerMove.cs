using System;
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

    // a normal with y-part greater than 0.5f is a 60° incline or flatter
    private const float MIN_GROUND_NORMAL_Y = 0.5f;

    // Actions
    private Actions actions;
    private InputAction moveAction;
    private InputAction jumpAction;

    // Components
    new private Rigidbody2D rigidbody;
    new private BoxCollider2D collider;

    private struct Contact 
    {
        public Vector2 point;
        public Vector2 normal;

        public Contact(RaycastHit2D hit)
        {
            point = hit.point;
            normal = hit.normal;
        }

        public Contact(ContactPoint2D cp)
        {
            point = cp.point;
            normal = cp.normal;
        }
    }

    // State
    private Vector2 movementDir;
    private List<Contact> contacts;
    private List<ContactPoint2D> contactPoints;
    private bool wasOnGround = false;

#region Init
    void Awake()
    {
        actions = new Actions();
        moveAction = actions.movement.move;
        jumpAction = actions.movement.jump;

        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        contacts = new List<Contact>();
        contactPoints = new List<ContactPoint2D>();
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

    void FixedUpdate()
    {        
        bool onGround = GroundContacts();

        if (wasOnGround && !onGround) 
        {
            onGround = TeleportToGround();
        }

        float targetSpeed = maxSpeed * movementDir.x;
        Contact? furthestContact = GetFurthestContact(targetSpeed);

        Vector2 dir = Vector2.right;
        if (furthestContact != null)
        {
            // a vector perpendicular to the normal with dir.x = 1
            Vector2 n = furthestContact.Value.normal;
            dir.y = -n.x / n.y;
        }

        rigidbody.velocity = targetSpeed * dir;

        if (!onGround) 
        {
            rigidbody.AddForce(gravity * rigidbody.mass * Vector2.down);
        }

        wasOnGround = onGround;
    }

    private bool GroundContacts()
    {
        rigidbody.GetContacts(contactPoints);
        contacts.Clear();

        for (int i = 0; i < contactPoints.Count; i++)
        {
            if (contactPoints[i].normal.y > MIN_GROUND_NORMAL_Y)
            {
                contacts.Add(new Contact(contactPoints[i]));
            }
        }

        return contacts.Count > 0;
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
            v.y = 0;
            rigidbody.velocity = v;

            // Add the hit point to the contacts list
            contacts.Add(new Contact(hit));            

            return true;
        }
        else 
        {
            return false;
        }

    }

    // dir > 0 => right
    // dir < 0 => left
    private Contact? GetFurthestContact(float dir)
    {
        Contact? furthestContact = null;
        float max = Mathf.NegativeInfinity;
        for (int i = 0; i < contacts.Count; i++)
        {
            if (contacts[i].normal.y > 0)
            {
                Vector2 v =  contacts[i].point - rigidbody.position;
                if (v.x * dir > max)
                {
                    furthestContact = contacts[i];
                    max = v.x * dir;
                }
            }
        }

        return furthestContact;
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
            Contact c = contacts[i];
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(c.point, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(c.point, c.point + c.normal);
        }
    }
#endregion Gizmos


}
