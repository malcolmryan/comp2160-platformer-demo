/**
 * Handles enemy health & death
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 2022.3
 */

using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

#region Events
    public void Hit()
    {
        Destroy(gameObject);
    }
#endregion

}
