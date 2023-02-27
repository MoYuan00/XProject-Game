using System;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class ShootControl : MonoBehaviour
{

    public Bullet shootObj;
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
