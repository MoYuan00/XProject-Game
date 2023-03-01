using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class ShootControl : MonoBehaviour
{
    [Tooltip("子弹预制体")]
    public Bullet shootObj;
    [Tooltip("发射子弹的位置")]
    public Transform shootTrans;

    //子弹的父节点
    public Transform m_UIParent;

    IEnumerator Shoot(Vector3 position, Quaternion dir)
    {
        Bullet bullet;
        //去对象池找
        UnityEngine.Object uiObj = Manager.Pool.Spawn("Shot","子弹");

        //如果找到了执行这个
        if (uiObj != null)
        {
            GameObject game = uiObj as GameObject;
            //从对象池返回的对象需要设置一下父节点
            game.transform.SetParent(m_UIParent, false);
            bullet = game.GetComponent<Bullet>();
            yield return null;
            
        }
        else
        {
            bullet = Instantiate<Bullet>(shootObj, position, dir);
            bullet.transform.SetParent(m_UIParent, false);
        }
        bullet.direction = shootTrans.forward;

        GameObject go = bullet.gameObject;

        // 等待3秒
        yield return new WaitForSeconds(3);
        Manager.Pool.UnSpawn("Shot", "子弹", go); //子弹应该超出一定距离放入对象池
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
            StartCoroutine(Shoot(shootTrans.position, shootTrans.rotation));
        
    }
}
