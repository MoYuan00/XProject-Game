using System;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class ShootControl : MonoBehaviour
{
    [Tooltip("子弹预制体")]
    public Bullet shootObj;
    [Tooltip("发射子弹的位置")]
    public Transform shootTrans;

    public void Shoot(Vector3 position, Quaternion dir)
    {
       var bullet = Instantiate<Bullet>(shootObj, position, dir);
       bullet.direction = shootTrans.forward;
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
            Shoot(shootTrans.position, shootTrans.rotation);
    }
}
