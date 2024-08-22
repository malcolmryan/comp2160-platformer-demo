using UnityEngine;
using UnityEngine.InputSystem;
using WordsOnPlay.Utils;

public class Reticle : MonoBehaviour
{
#region Parameters
    [SerializeField] float sensitivity = 10;
#endregion

#region Actions
    // Actions
    private Actions actions;
    private InputAction aimAction;
#endregion

#region State
    private Vector3 screenPos;
#endregion

#region Init
    void Awake()
    {
        actions = new Actions();
        aimAction = actions.shooting.aim;
        screenPos = Camera.main.WorldToScreenPoint(transform.position);

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
        Vector3 delta = aimAction.ReadValue<Vector2>();
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        screenPos += delta * sensitivity;
        screenPos = Camera.main.pixelRect.Clamp(screenPos);
        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
    }
#endregion
}
