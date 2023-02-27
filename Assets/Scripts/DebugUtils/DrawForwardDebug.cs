using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DrawForwardDebug : MonoBehaviour
{
    private Transform _transform;
    public float length = 0.5f;
    public Color color = Color.green;

    private void OnEnable()
    {
        _transform = gameObject.transform;
    }


    private void OnDrawGizmos()
    {
        var position = _transform.position;
        Gizmos.color = color;
        Gizmos.DrawLine(position, position + _transform.forward * length);
    }
}
