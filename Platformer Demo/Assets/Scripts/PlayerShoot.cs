using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
#region Parameters
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePosition;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private float fireBufferTime = 0.1f;
#endregion

#region Connections
    [SerializeField] private Transform reticule;
#endregion

#region Actions
    // Actions
    private Actions actions;
    private InputAction fireAction;
#endregion

#region State
    private float cooldownTimer = 0;
    private float lastFireTime = float.NegativeInfinity;
#endregion


#region Init
    void Awake()
    {
        actions = new Actions();
        fireAction = actions.shooting.fire;
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
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        Vector3 v = reticule.position - transform.position;
        transform.localRotation = Quaternion.FromToRotation(Vector3.right, v);

        if (fireAction.WasPressedThisFrame())
        {
            lastFireTime = Time.time;
        }   

        if (cooldownTimer <= 0 && Time.time - lastFireTime < fireBufferTime)
        {
            Fire();
        }
    }

    private void Fire()
    {
        cooldownTimer = cooldown;
        lastFireTime = float.NegativeInfinity;

        Vector3 position = firePosition.position;
        Quaternion rotation = firePosition.rotation;
        Bullet bullet = Instantiate(bulletPrefab, position, rotation);        
    }
#endregion
}
