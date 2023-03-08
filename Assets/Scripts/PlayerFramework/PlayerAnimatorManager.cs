using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerFramework
{
    public class PlayerAnimatorManager : MonoBehaviour
    {

        private Animator _animator;
        
        private float playerPostureStandThreshold = 1;
        private float playerPostureMidairThreshold = 2.01f;
        
        private int postureHash = Animator.StringToHash("玩家姿态");
        private int moveSpeedHash = Animator.StringToHash("移动速度");
        private int trunSpeedHash = Animator.StringToHash("转向");
        private int verticalSpeedHash = Animator.StringToHash("垂直速度");
        private int FeetTween = Animator.StringToHash("左右脚");
        private int wallClimbSpeed = Animator.StringToHash("攀爬速度");
        private int isClimb = Animator.StringToHash("IsClimb");

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void Idle()
        {
            _animator.SetFloat(postureHash, playerPostureStandThreshold, 0.1f, Time.deltaTime);
            _animator.SetFloat(moveSpeedHash, 0, 0.1f, Time.deltaTime);
        }
        
        public void Walk(float speed)
        {
            _animator.SetFloat(postureHash, playerPostureStandThreshold, 0.1f, Time.deltaTime);
            _animator.SetFloat(moveSpeedHash, speed, 0.1f, Time.deltaTime);
        }
        
        public void Run(float speed)
        {
            _animator.SetFloat(postureHash, playerPostureStandThreshold, 0.1f, Time.deltaTime);
            _animator.SetFloat(moveSpeedHash, speed, 0.1f, Time.deltaTime);
        }

        public void Jump()
        {
            var feetTweenRandom = UnityEngine.Random.Range(-1f, 1f);
            _animator.SetFloat(FeetTween, feetTweenRandom);
        }

        public void Rotate(float rad)
        {
            if (rad == 0)
            {
                _animator.SetFloat(trunSpeedHash, rad);
                return;
            }
            _animator.SetFloat(trunSpeedHash, rad, 0.1f, Time.deltaTime);
        }
        
        public void MidAir_Posture(float verticalVelocity)
        {
            _animator.SetFloat(postureHash, playerPostureMidairThreshold);
            _animator.SetFloat(verticalSpeedHash, verticalVelocity);
        }

        public void Climb(float speed)
        {
            _animator.SetBool(isClimb, true);
            _animator.SetFloat(wallClimbSpeed, speed);
        }

        public void ClimbEnd()
        {
            _animator.SetBool(isClimb, false);
            _animator.SetFloat(wallClimbSpeed, 0f);
        }

    }
}