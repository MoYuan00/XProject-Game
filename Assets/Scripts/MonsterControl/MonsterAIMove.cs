using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物跟随主角
/// </summary>
public class MonsterAIMove : MonoBehaviour
{
    public float speed = 1f;
    public float rotateSpeed = 300f;

    private Transform _transform;

    public Transform target;

    private void OnEnable()
    {
        _transform = gameObject.transform;
    }

    void Update()
    {
        if (target != null)
        {
            RotateUpdate();
            MoveUpdate();
        }
    }

    void MoveUpdate()
    {
        var dir = target.position - _transform.position;
        dir.y = 0;
        
        if(dir.magnitude < 0.01f) return;

        var moveDir = Vector3.Lerp(_transform.forward, dir.normalized, 0.5f);
        _transform.position += moveDir * (speed * Time.deltaTime);
    }

    void RotateUpdate()
    {
        var dir = target.position - _transform.position;
        dir.y = 0;
        
        Debug.DrawLine(_transform.position, target.position, Color.red);
        
        if(dir.magnitude < 0.01f) return;
        
        Quaternion targetRotate = Quaternion.LookRotation(dir.normalized, Vector3.up);
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, 
            targetRotate, rotateSpeed * Time.deltaTime);
        
    }

}
