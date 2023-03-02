using System;
using System.Collections;
using System.Linq;
using FrameworkFSM;
using FrameworkAnimation;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CharControl_New
{
    public class PlayerMovement : MonoBehaviour
    {
        private Transform _transform;
        [Header("References")] public Animator animator;
        public Transform bodyCenter;
        public Rigidbody rigidbody;
        public GroundChecker groundChecker;
        private bool _isOnGround;

        [Header("Movement")] public float runningSpeed = 3.442973f;
        public float runningSpeedScale = 1f;
        public float airSpeedScale = 0.4f; // 在空中的速度缩放
        private float _targetSpeed = 0f;
        public float groundDrag = 4f; // 地面阻力，帮助在停止输入时停下来

        [Header("Jump")] public float jumpUpSpeed;
        public float jumpCooldown = 0.2f;
        public float fallDownSpeed = 5f; // 跌落的速度曲线
        private bool _readyToJump = true;
        public float jumpMaxHeight = 1f;

        [Header("Pistol Aim")] public float pistolAimCooldown = 0.4f;
        private bool _readyPistolAim = true;


        // 输入
        private Vector2 _inputMoveVec2;
        private bool _inputJump;
        private bool _inputPistolAim;

        // 动画
        private static readonly int PistolAimParam = Animator.StringToHash("PistolAim");
        private static readonly int IsRunningParam = Animator.StringToHash("IsRunning");
        private static readonly int IsJumpingParam = Animator.StringToHash("IsJumping");

        public CharMoveState state;

        [Header("Aim IK")] public AimIK aimIK;

        private void Awake()
        {
            _transform = transform;
        }

        public void Start()
        {
            DoUpdateAimIK();
            DoUpdateCamera();
        }

        private void Update()
        {
            _isOnGround = groundChecker.IsOnGround();

            if (_isOnGround) RemoveState(CharMoveState.Air);
            else SetState(CharMoveState.Air);

            if (_isOnGround)
            {
                rigidbody.drag = groundDrag;
                RemoveState(CharMoveState.JumpingUp);
            }

            DoStateMove();
            
        }

        private void DoUpdateAimIK()
        {
            if (ExistState(CharMoveState.PistolAim))
            {
                Invoke(nameof(ResetAimIK), 0.4f);
            }
            else aimIK.enabled = false;
        }

        private void ResetAimIK()
        {
            aimIK.enabled = true;
        }

        private void LateUpdate()
        {
            DoUpdateCamera();
        }

        private void DoUpdateCamera()
        {
            if (ExistState(CharMoveState.PistolAim))
            {
                CameraManager.ChangeCameraState(CameraMode.WalkingAim);
            }
            else
            {
                CameraManager.ChangeCameraState(CameraMode.Free);
            }
        }

        private void DoStatePistolAim()
        {
            if (!_inputPistolAim || !_readyPistolAim) return;
            if (ExistState(CharMoveState.PistolAim))
            {
                // 取消拔枪
                animator.SetBool(PistolAimParam, false);
                RemoveState(CharMoveState.PistolAim);
                DoUpdateAimIK();
                return;
            }

            _readyPistolAim = false;
            animator.SetBool(PistolAimParam, true);
            SetState(CharMoveState.PistolAim);
            
            CameraManager.ChangeCameraState(CameraMode.WalkingAim);
            DoUpdateAimIK();
            
            StartCoroutine(nameof(ResetPistolAim), pistolAimCooldown);
        }

        private void ResetPistolAim()
        {
            _readyPistolAim = true;
        }

        private void DoStateJumping()
        {
            _isOnGround = groundChecker.IsOnGround();
            if (!_inputJump || !_readyToJump || !_isOnGround)
            {
                RemoveState(CharMoveState.JumpingUp);
                return;
            }

            SetState(CharMoveState.JumpingUp);
            _readyToJump = false;
            JumpPlayer();
            Invoke(nameof(ResetJump), jumpCooldown);

            animator.SetTrigger(IsJumpingParam);
        }

        private void JumpPlayer()
        {
            var velocity = rigidbody.velocity;
            rigidbody.velocity = new Vector3(velocity.x, jumpUpSpeed, velocity.z);
        }

        private void ResetJump()
        {
            _readyToJump = true;
        }

        private void DoStateMove()
        {
            if (_inputMoveVec2 != Vector2.zero)
            {
                animator.SetBool(IsRunningParam, true);
                MovePlayer();
                return;
            }

            rigidbody.drag = groundDrag;
            animator.SetBool(IsRunningParam, false);
        }

        private void MovePlayer()
        {
            if (_inputMoveVec2 == Vector2.zero) return;
            var moveSpeed = runningSpeed * runningSpeedScale;

            if (ExistState(CharMoveState.Air))
                moveSpeed *= airSpeedScale;

            _targetSpeed = Mathf.Lerp(_targetSpeed, moveSpeed, 0.5f);

            var speed = _transform.forward * _targetSpeed;
            rigidbody.velocity = new Vector3(speed.x, rigidbody.velocity.y, speed.z);
        }


        #region InputEvent

        public void OnMoveInput(InputAction.CallbackContext ctx)
        {
            switch (ctx.phase)
            {
                case InputActionPhase.Canceled:
                    _inputMoveVec2 = Vector2.zero;
                    return;
                case InputActionPhase.Performed:
                    _inputMoveVec2 = ctx.ReadValue<Vector2>();
                    return;
            }
        }

        public void OnPistolAim(InputAction.CallbackContext ctx)
        {
            _inputPistolAim = ctx.phase == InputActionPhase.Performed;
            Debug.Log("OnPistolAim");
            if (_inputPistolAim)
                DoStatePistolAim();
        }

        public void OnJumping(InputAction.CallbackContext ctx)
        {
            _inputJump = ctx.phase == InputActionPhase.Performed;
            Debug.Log("OnJumping");
            if (_inputJump) DoStateJumping();
        }

        #endregion

        private void SetState(CharMoveState moveState)
        {
            this.state |= moveState;
            Debug.Log($"State: add '{moveState}'  all: {state}");
        }

        private void RemoveState(CharMoveState moveState)
        {
            if (ExistState(moveState))
            {
                var t = state;
                state -= moveState;
                Debug.Log($"State: remove '{moveState}'  all: {state}");
            }
        }

        private bool ExistState(CharMoveState moveState)
        {
            return (state & moveState) == moveState;
        }
    }

    [Flags]
    public enum CharMoveState
    {
        Idle = 0,
        PistolAim = 2, // 瞄准
        JumpingUp = 4, // 跳起中

        Air = 8, // 是否在空中
    }
}