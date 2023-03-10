using System;
using CharControl;
using Framework.Utils;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CharControl_New
{
    [Obsolete]
    public class PlayerMovement : MonoBehaviour
    {
        private Transform _transform;
        [Header("References")] public Animator animator;
        public Rigidbody rigidbody;
        public GroundChecker groundChecker;
        private bool _isOnGround;

        [Header("Movement References")] public Transform playerObj;

        [Header("Movement")] public float runningSpeed = 3.442973f;
        public float runningSpeedScale = 1f;
        public float airSpeedScale = 0.4f; // 在空中的速度缩放
        private float _targetSpeed = 0f;
        public float groundDrag = 4f; // 地面阻力，帮助在停止输入时停下来

        [Header("Jump")] public float jumpUpSpeed;
        public float jumpCooldown = 0.2f;
        public AnimationCurve fallDownSpeedCurve; // 跌落的速度曲线
        private AnimationCurveHelper _fallDownSpeedCurveHelper;

        private bool _readyToJump = true;
        public float jumpMaxHeight = 1f;
        public AnimationCurve jumpSpeedCurve;
        private AnimationCurveHelper _jumpSpeedCurveHelper;

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

        public PlayerStateManager playerState;

        [Header("Aim IK")] public AimIK aimIK;

        public float timeScale = 0.3f;

        private void Awake()
        {
            _transform = transform;
            _fallDownSpeedCurveHelper = new AnimationCurveHelper(fallDownSpeedCurve);
            _jumpSpeedCurveHelper = new AnimationCurveHelper(jumpSpeedCurve);
        }

        public void Start()
        {
            DoUpdateAimIK();
            DoUpdateCamera();
        }

        private void Update()
        {
            Time.timeScale = timeScale;
            _isOnGround = groundChecker.IsOnGround();

            if (_isOnGround) playerState.Remove(CharMoveState.Air);
            else playerState.Append(CharMoveState.Air);

            if (_isOnGround)
            {
                rigidbody.drag = groundDrag;
            }

            DoStateMove();


            DoFallDown();
            DoStateJumping();
        }

        public void JumpingToTopPoint(int s)
        {
            if (s != 1) return;
            playerState.Append(CharMoveState.JumpingDown);
            playerState.Remove(CharMoveState.JumpingUp);
        }

        private void DoFallDown() // 跌落
        {
            if (playerState.Exists(CharMoveState.JumpingUp) || _isOnGround)
            {
                _fallDownSpeedCurveHelper.Reset();
                playerState.Remove(CharMoveState.JumpingDown);
                return;
            }
            
            var velocity = rigidbody.velocity;
            velocity = new Vector3(velocity.x, velocity.y + _fallDownSpeedCurveHelper.GetValue(), velocity.z);
            rigidbody.velocity = velocity;
        }

        private void DoUpdateAimIK()
        {
            if (playerState.Exists(CharMoveState.PistolAim))
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
            if (playerState.Exists(CharMoveState.PistolAim))
            {
                CameraManager.ChangeCameraState(CameraMode.WalkingAim);
            }
            else
            {
                CameraManager.ChangeCameraState(CameraMode.Free);
            }
        }

        #region PistolAim

        private void DoStatePistolAim()
        {
            if (!_inputPistolAim || !_readyPistolAim) return;
            if (playerState.Exists(CharMoveState.PistolAim))
            {
                // 取消拔枪
                animator.SetBool(PistolAimParam, false);
                playerState.Remove(CharMoveState.PistolAim);
                DoUpdateAimIK();
                return;
            }

            _readyPistolAim = false;
            animator.SetBool(PistolAimParam, true);
            playerState.Append(CharMoveState.PistolAim);

            CameraManager.ChangeCameraState(CameraMode.WalkingAim);
            DoUpdateAimIK();

            StartCoroutine(nameof(ResetPistolAim), pistolAimCooldown);
        }

        private void ResetPistolAim()
        {
            _readyPistolAim = true;
        }

        #endregion

        #region Jumping

        private void DoStateJumping()
        {
            _isOnGround = groundChecker.IsOnGround();
            if (!_inputJump || !_readyToJump || !_isOnGround)
            {
                _jumpSpeedCurveHelper.Reset();
                return;
            }

            playerState.Append(CharMoveState.JumpingUp);
            _readyToJump = false;
            JumpPlayer();
            Invoke(nameof(ResetJump), jumpCooldown);

            animator.SetTrigger(IsJumpingParam);
        }

        private void JumpPlayer()
        {
            var velocity = rigidbody.velocity;
            rigidbody.velocity = new Vector3(velocity.x, GetJumpSpeedY(), velocity.z);
        }

        private float GetJumpSpeedY()
        {
            _jumpSpeedCurveHelper.IncreaseUpdateTime();
            return _jumpSpeedCurveHelper.GetValue();
        }

        private void ResetJump()
        {
            _readyToJump = true;
        }

        #endregion

        #region Move

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

            if (playerState.Exists(CharMoveState.Air))
                moveSpeed *= airSpeedScale;

            _targetSpeed = Mathf.Lerp(_targetSpeed, moveSpeed, 0.5f);

            var speed = _transform.forward * _targetSpeed;
            rigidbody.velocity = new Vector3(speed.x, rigidbody.velocity.y, speed.z);
        }

        #endregion

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
    }
}