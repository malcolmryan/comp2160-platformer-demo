using UnityEngine;
using UnityEngine.InputSystem;

public class Reticule : MonoBehaviour
{
#region Actions
    // Actions
    private Actions actions;
    private InputAction aimAction;
#endregion

#region Init
    void Awake()
    {
        actions = new Actions();
        aimAction = actions.shooting.aim;

        // detach from player
        transform.parent = null;
    }

    void OnEnable()
    {
        actions.shooting.Enable();
    }

    void OnDisable()
    {
        actions.shooting.Disable();
    }

#endregion

#region Update
    void Update()
    {
        Vector3 pos = aimAction.ReadValue<Vector2>();
        pos = Camera.main.ScreenToWorldPoint(pos);      
        pos.z = 0;
        transform.position = pos;
    }
#endregion
}
