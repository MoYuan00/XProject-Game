using System;
using FrameworkFSM;
using FrameworkAnimation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CharControlCamera3rd
{
    [Obsolete]
    public class Camera3rdPlayerMoveControl : MonoBehaviour
    {
        private FSMArray<CharMoveState> _fsm = new FSMArray<CharMoveState>();
        private Transform _transform;

        public Animator animator;
        public Transform bodyCenter;

        private static readonly int PistolAim = Animator.StringToHash("PistolAim");
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");

        public float runningSpeed = 3.442973f;
        public float runningSpeedScale = 1f;
        private float _targetSpeed = 0f;
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");

        public float jumpUpForce = 3f;
        public float jumpDownForce = -1f;

        public Rigidbody rigidbody;

        private void Awake()
        {
            _transform = transform;
            _fsm.Register(CharMoveState.Idle)
                .OnEnter(() =>
                {
                    rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
                });
            _fsm.ChangeState(CharMoveState.Idle);

            RegisterPistolAim();
            RegisterRunning();
            RegisterJump();
        }

        #region RegisterFSM - 注册状态机事件

        private void RegisterPistolAim()
        {
            _fsm.Register(CharMoveState.PistolAim, CharMoveState.Running, CharMoveState.JumpingUp)
                .OnEnter(() =>
                {
                    animator.SetBool(PistolAim, true);
                    CameraManager.ChangeCameraState(CameraMode.WalkingAim);
                })
                .OnExit(() =>
                {
                    animator.SetBool(PistolAim, false);
                    CameraManager.ChangeCameraState(CameraMode.Free);
                });
        }

        private void RegisterRunning()
        {
            _fsm.Register(CharMoveState.Running, CharMoveState.PistolAim)
                .OnEnter(() => { animator.SetBool(IsRunning, true); })
                .OnFixUpdate(() => { MovePlayer(); })
                .OnExit(() =>
                {
                    _targetSpeed = 0f;
                    rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
                    animator.SetBool(IsRunning, false);
                });
        }

        private void RegisterJump()
        {
            _fsm.Register(CharMoveState.JumpingUp)
                .OnEnter(() =>
                {
                    animator.SetBool(IsJumping, true);
                    rigidbody.AddForce(transform.up * jumpUpForce, ForceMode.Impulse);
                }).OnExit(() => { animator.SetBool(IsJumping, false); });

            // AnimationEventManager.OnStateExit(AnimationStateName.JumpingUp,
            //     () =>
            //     {
            //         _fsm.ChangeState(CharMoveState.JumpingDown);
            //     });
            //
            //
            // _fsm.Register(CharMoveState.JumpingDown)
            //     .OnFixUpdate(() =>
            //     {
            //         rigidbody.AddForce(transform.up * jumpDownForce, ForceMode.Force);
            //     });
            // AnimationEventManager.OnStateExit(AnimationStateName.JumpingDown,
            //     () => _fsm.ExitState(CharMoveState.JumpingDown));
        }

        #endregion

        private void Update()
        {
            _fsm.Update();
        }
        private void FixedUpdate()
        {
            _fsm.FixUpdate();
        }

        private void OnDestroy()
        {
            _fsm.Clear();
        }

        private void MovePlayer()
        {
            var speed = _transform.forward * _targetSpeed;
            rigidbody.velocity = new Vector3(speed.x, rigidbody.velocity.y, speed.z);
            _targetSpeed = Mathf.Lerp(_targetSpeed, runningSpeed * runningSpeedScale, 0.5f); // 插值
        }


        #region InputEvent

        public void OnMoveInput(InputAction.CallbackContext ctx)
        {
            switch (ctx.phase)
            {
                case InputActionPhase.Canceled:
                    if (_fsm.IsRunning(CharMoveState.Running))
                        _fsm.ExitState(CharMoveState.Running);
                    return;
                case InputActionPhase.Performed:
                    if (_fsm.IsRunningAny(CharMoveState.JumpingUp, CharMoveState.JumpingDown)) return;
                    _fsm.ChangeState(CharMoveState.Running);
                    return;
            }
        }

        public void OnPistolAim(InputAction.CallbackContext ctx)
        {
            if (ctx.phase != InputActionPhase.Performed) return;
            if (_fsm.IsRunning(CharMoveState.PistolAim))
            {
                _fsm.ExitState(CharMoveState.PistolAim);
                return;
            }

            _fsm.ChangeState(CharMoveState.PistolAim);
        }

        public void OnJumping(InputAction.CallbackContext ctx)
        {
            if (ctx.phase != InputActionPhase.Performed) return;
            _fsm.ChangeState(CharMoveState.JumpingUp);
        }

        #endregion
    }

    public enum CharMoveState
    {
        Idle,
        Moving, // 移动
        PistolAim, // 瞄准
        Running, // 奔跑中
        JumpingDown,
        JumpingUp,
    }
}