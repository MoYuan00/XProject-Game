using System;
using UnityEngine;

namespace PlayerFramework
{
    public class PlayerEnvManager : MonoBehaviour
    {
        [Header("检查点离地面的距离")] public float groundOffset = 0.5f;
        public float groundCheckHeight = 0.2f;
        [Header("wall face")] public float faceWallCheckLength = 0.2f;
        public float faceWallAngle = 35.0f;
        public Transform wallFaceCenter;
        public Transform wallFaceFoot;
        private bool _isHitWall; // 是否碰到了墙壁
        private Collision _collision;
        private CharacterController characterController;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
        }

        public bool CheckIsGrounded()
        {
            if (Physics.SphereCast(transform.position + (Vector3.up * groundOffset), characterController.radius,
                    Vector3.down,
                    out var hit, groundOffset - characterController.radius + 2 * characterController.skinWidth))
            {
                return true;
            }

            if (Physics.Raycast(transform.position, Vector3.down, out var groundHit, groundCheckHeight))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否面向墙壁
        /// </summary>
        /// <returns></returns>
        public bool CheckFaceWall(out Vector3 hitPosition, out Vector3 hitDir)
        {
            if (Physics.Raycast(wallFaceCenter.position, wallFaceCenter.forward, out var hit, faceWallCheckLength))
            {
                // 脚底和碰撞点的高度差：
                var h = wallFaceCenter.position - wallFaceFoot.position;
                hitPosition = hit.point - h;
                hitDir = -hit.normal;
                return true;
            }
            hitDir = Vector3.right;
            hitPosition = Vector3.zero;
            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallFaceCenter.position, wallFaceCenter.position + wallFaceCenter.forward * faceWallCheckLength);
        }
    }
}