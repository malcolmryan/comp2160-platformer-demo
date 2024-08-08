using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class PlayerMove : MonoBehaviour
{
#region Parameters
    [SerializeField] private Vector2 maxSpeed = new Vector2(5f,5f);
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float gravity = 33f;
    [SerializeField] private float raycastDist = 0.5f;
    [SerializeField] private LayerMask floorLayer;

    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float jumpRiseTime = 1f;

#endregion 

#region Constants
    // a normal with y-part greater than 0.5f is a 60° incline or flatter
    private const float MIN_GROUND_NORMAL_Y = 0.5f;
#endregion

#region Actions
    // Actions
    private Actions actions;
    private InputAction moveAction;
    private InputAction jumpAction;
#endregion

#region Components
    // Components
    new private Rigidbody2D rigidbody;
    new private BoxCollider2D collider;
#endregion

#region State
    // State

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

    private Vector2 movementDir;
    private List<Contact> contacts;
    private List<ContactPoint2D> contactPoints;
    private bool onGround = false;
    private bool wasOnGround = false;

    private float lastJumpTime = float.NegativeInfinity;
    private float lastJumpPressedTime = float.NegativeInfinity;
    private float lastOnGroundTime = float.NegativeInfinity;
    private enum JumpState { 
        OnGround, RisingNoGravity, RisingGravity, FallingGravity, FallingNoGravity 
    }
    private JumpState jumpState;
#endregion

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
        if (jumpAction.WasPerformedThisFrame()) {
            lastJumpPressedTime = Time.time;
        }
    }

#endregion Update

#region FixedUpdate

    void FixedUpdate()
    {        
        onGround = GroundContacts();

        if (jumpState == JumpState.OnGround && wasOnGround && !onGround) 
        {
            // teleport to the ground it close enough
            onGround = TeleportToGround();
        }

        if (onGround) 
        {
            lastOnGroundTime = Time.fixedTime;
        }

        Jump();
        Move();

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

    private void Move() 
    {
        if (jumpState == JumpState.OnGround) 
        {
            MoveOnGround();
        }
        else 
        {
            MoveInAir();
        }
    }

    private void MoveOnGround() 
    {
        float targetSpeed = maxSpeed.x * movementDir.x;
        Contact? furthestContact = GetFurthestContact(targetSpeed);

        Vector2 dir = Vector2.right;
        if (furthestContact != null)
        {
            // a vector perpendicular to the normal with dir.x = 1
            Vector2 n = furthestContact.Value.normal;
            dir.y = -n.x / n.y;
        }

        rigidbody.velocity = targetSpeed * dir;
    }

    private void MoveInAir()
    {
        float targetSpeed = maxSpeed.x * movementDir.x;

        // set horizontal velocity directly
        Vector2 v = rigidbody.velocity;
        v.x = targetSpeed;
        rigidbody.velocity = v;        
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

    private void Jump()
    {
        Vector2 v = rigidbody.velocity;

        switch (jumpState) {
        
        case JumpState.OnGround:
            if (CheckJump())
            {
                DoJump();
            }
            else if (!onGround)
            {
                jumpState = JumpState.FallingGravity;
            }
            break;

        case JumpState.RisingNoGravity:
            if (Time.fixedTime - lastJumpTime > jumpRiseTime
                || !jumpAction.IsPressed()) 
            {
                jumpState = JumpState.RisingGravity;
                ApplyGravity();
            }
            break;

        case JumpState.RisingGravity:
            ApplyGravity();

            if (v.y <= 0) 
            {
                jumpState = JumpState.FallingGravity;
            }
            break;

        case JumpState.FallingGravity:
            // can possibly still jump due to coyote time
            if (CheckJump())
            {
                DoJump();
            }
            else if (v.y > maxSpeed.y)
            {
                // hit maximum speed, no gravity.
                v.y = maxSpeed.y;
                rigidbody.velocity = v;
                jumpState = JumpState.FallingNoGravity;
            }
            else if (onGround)
            {
                jumpState = JumpState.OnGround;
            }
            else 
            {
                ApplyGravity();
            }

            break;

        case JumpState.FallingNoGravity:
            // assume no coyote time
            if (onGround)
            {
                jumpState = JumpState.OnGround;
            }
            else 
            {
                // maintain maximum speed
                v.y = maxSpeed.y;
                rigidbody.velocity = v;
            }
            break;

        }

    }

    private bool CheckJump()
    {
        return (Time.fixedTime - lastJumpPressedTime < jumpBufferTime
                && Time.fixedTime - lastOnGroundTime < coyoteTime);
    }

    private void DoJump()
    {
        lastJumpTime = Time.fixedTime;
        lastOnGroundTime =  float.NegativeInfinity;
        lastJumpPressedTime = float.NegativeInfinity;
        rigidbody.AddForce(jumpSpeed * rigidbody.mass * Vector2.up, ForceMode2D.Impulse);

        if (jumpAction.WasReleasedThisFrame()) {
            jumpState = JumpState.RisingGravity;
        }
        else
        {
            jumpState = JumpState.RisingNoGravity;
        }
    }

    private void ApplyGravity()
    {
        rigidbody.AddForce(gravity * rigidbody.mass * Vector2.down);
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
