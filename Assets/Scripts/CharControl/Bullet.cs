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
    
    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = transform;
    }

    public void Start()
    {
        _rigidbody.velocity = direction * speed;
    }
}
