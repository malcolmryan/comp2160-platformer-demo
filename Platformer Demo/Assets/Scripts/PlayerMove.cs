using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float speed = 5;

    private Actions actions;
    private InputAction moveAction;
    private InputAction jumpAction;

    private Vector2 movementDir;

    new private Rigidbody2D rigidbody;

#region Init
    void Awake()
    {
        actions = new Actions();
        moveAction = actions.movement.move;
        jumpAction = actions.movement.jump;

        rigidbody = GetComponent<Rigidbody2D>();
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
        Vector2 v = rigidbody.velocity;
        v.x = movementDir.x * speed;
        rigidbody.velocity = v;
    }

#endregion FixedUpdate

}
