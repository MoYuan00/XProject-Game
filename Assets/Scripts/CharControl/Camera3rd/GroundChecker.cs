using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("地面检查，判断人是否在地面")] public LayerMask groundMask;
    [Tooltip("地面检查射线长度")] public Transform groundCheckOrigin;
    public float groundCheckRayLength = 1.7f;

    private void OnDrawGizmos()
    {
        var position = groundCheckOrigin.position;
        Debug.DrawLine(position, Vector3.down * groundCheckRayLength + position, Color.cyan);
    }

    public bool IsOnGround()
    {
        if (Physics.Raycast(groundCheckOrigin.position, Vector3.down, out var hit,
                groundCheckRayLength, groundMask))
        {
            return true;
        }

        return false;
    }
}