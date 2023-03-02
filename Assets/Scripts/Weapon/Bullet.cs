using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 可造成伤害的射击物体
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    private Rigidbody _rigidbody;

    private Transform _transform;

    public Vector3 direction;

    // 爆炸特效
    public GameObject explodeVFX;
    
    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = transform;
    }

    public void Start()
    {
        _rigidbody.velocity = direction * speed;
    }

    /// <summary>
    /// 爆炸并销毁
    /// </summary>
    public void Explode(Vector3 position, Quaternion dir)
    {
        if (explodeVFX != null)
        {
            var obj = Instantiate(explodeVFX, position, dir);
            var vfx = obj.GetComponent<ParticleSystem>();
            vfx.gameObject.SetActive(true);
            vfx.Play();
            Destroy(obj, vfx.main.duration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("CanHit"))
        {
            var point = collision.contacts[0];
            var pos = point.point;
            collision.collider.ClosestPoint(_transform.position);
            var rotation = Quaternion.LookRotation(point.normal);
            Explode(pos, rotation);
        }
        Destroy(gameObject);
    }
}
