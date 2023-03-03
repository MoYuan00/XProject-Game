using System;
using System.Collections;
using CharControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[ExecuteAlways]
[RequireComponent(typeof(PlayerStateManager))]
public class ShootControl : MonoBehaviour
{
    [Tooltip("子弹预制体")] public Bullet bulletObj;
    [Tooltip("发射子弹的位置")] public Transform shootTrans;
    public LayerMask shootLayerMask; // 可以设计的目标层级

    public float maxShootDistance = 200;

    private PlayerStateManager _playerStateManager;

    //子弹的父节点
    public Transform m_UIParent;

    private Camera _camera;

    public Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

    private void Start()
    {
        _camera = Camera.main;
        _playerStateManager = GetComponent<PlayerStateManager>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(shootTrans.position, shootTrans.position + GetShootDirection() * maxShootDistance);
    }


    IEnumerator Shoot(Vector3 position, Quaternion dir)
    {
        Bullet bullet;
        //去对象池找
        UnityEngine.Object uiObj = Manager.Pool.Spawn("Shot", "子弹");

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
            bullet = Instantiate<Bullet>(bulletObj, position, dir);
            bullet.transform.SetParent(m_UIParent, false);
        }

        bullet.direction = GetShootDirection();

        GameObject go = bullet.gameObject;

        // 等待3秒
        yield return new WaitForSeconds(3);
        Manager.Pool.UnSpawn("Shot", "子弹", go); //子弹应该超出一定距离放入对象池
    }

    private Vector3 GetShootDirection()
    {
        Ray ray = _camera.ScreenPointToRay(screenCenter); //射线
        if (Physics.Raycast(ray, out var hit, maxShootDistance, shootLayerMask)) //发射射线(射线，射线碰撞信息，射线长度，射线会检测的层级)
        {
            return hit.point - _camera.transform.position;
        }
        return shootTrans.forward;
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Performed) return;
        Debug.Log("OnFire");
        StartCoroutine(Shoot(shootTrans.position, shootTrans.rotation));
    }
}