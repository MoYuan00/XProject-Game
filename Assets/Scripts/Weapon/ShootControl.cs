using System;
using System.Collections;
using CharControl;
using UnityEngine;
using UnityEngine.Events;
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

    public AudioSource audioSource;

    public UnityEvent onShoot;

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


    void Shoot(Vector3 position, Quaternion dir)
    {
       
        Bullet bullet = Instantiate<Bullet>(bulletObj, position, dir);
        bullet.transform.SetParent(m_UIParent, false);
        bullet.direction = GetShootDirection();
        bullet.gameObject.transform.forward = bullet.direction;

        
        PlayerShootAudio();
        onShoot?.Invoke();
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
        Shoot(shootTrans.position, shootTrans.rotation);
    }

    private void PlayerShootAudio()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
    
    
}