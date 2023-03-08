using System;
using PlayerFramework.model;
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
        public Transform wallFaceHead;
        public Transform wallFaceFoot;
        [Tooltip("超过这个位置，上面如果有足够的空间，那么就结束攀爬")]
        public Transform wallUpPos;
        [Tooltip("检查上方是否有落脚点是，检查的前方距离。可以理解为超过多少距离，角色就可以落脚。")]
        public float wallUpCheckLength = 0.3f;
        public float wallUpCheckInterval = 0.32f;
        public int wallUpCheckCount = 5;
        private bool _isHitWall; // 是否碰到了墙壁
        private Collision _collision;
        private CharacterController characterController;

        public ClimbingWall climbingWall;
        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            climbingWall = new ClimbingWall();
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
        public bool CheckFaceWall()
        {
            if (Physics.Raycast(wallFaceCenter.position, wallFaceCenter.forward, out var hit, faceWallCheckLength))
            {
                // 如果头部位置，也有墙体，那么就表示前面真的有一堵墙
                if (Physics.Raycast(wallFaceHead.position, wallFaceHead.forward, out var _, faceWallCheckLength))
                {
                    climbingWall.wall = hit.transform.gameObject;
                    climbingWall.hitPosition = hit.point;
                    // 脚底和碰撞点的高度差：
                    climbingWall.hitReltivePosition = hit.point - (wallFaceCenter.position - wallFaceFoot.position);
                    climbingWall.hisPositionNormal = hit.normal;
                    return true;
                }
            }
            return false;
        }
        
        // 判断当前角色是否可以直接越上建筑，结束攀爬
        public bool CheckTopHasSpace()
        {
            var checkPos = wallUpPos.position;
            for (int i = 0; i < wallUpCheckCount; i++)
            {
                if (Physics.Raycast(checkPos, wallUpPos.forward, out var _, wallUpCheckLength))
                {
                    return false;
                }
                // 如果有位置，继续向上检查，保证有一个人能站立的高度
                checkPos += Vector3.up * wallUpCheckInterval;
            }
            return true;
        }

        private bool _CheckTopHasSpace = false;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallFaceCenter.position, wallFaceCenter.position + wallFaceCenter.forward * faceWallCheckLength);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallFaceHead.position, wallFaceHead.position + wallFaceHead.forward * faceWallCheckLength);


            {
                var checkPos = wallUpPos.position;
                for (int i = 0; i < wallUpCheckCount; i++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(checkPos, checkPos + wallUpPos.forward * wallUpCheckLength);
                    checkPos += Vector3.up * wallUpCheckInterval;
                }

                var t = CheckTopHasSpace();
                if (_CheckTopHasSpace != t)
                {
                    _CheckTopHasSpace = t;
                    Debug.Log($"{t}");
                }
            }
        }
    }
}