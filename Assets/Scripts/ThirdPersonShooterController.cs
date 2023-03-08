using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = System.Random;

namespace CharControl
{
    public class ThirdPersonShooterController : MonoBehaviour
    {
        private Animator _animator;
        private CharacterController _characterController;

        public float gravity = -9.8f;
        private float _currentVerticalVelocity = 0f;

        public enum PlayerPosture
        {
            Stand = 1,
            Midair = 2 // 滞空
        }

        public float playerPostureStandThreshold = 1;
        public float playerPostureMidairThreshold = 2.1f; // 阈值为2，设置比阈值大一点，可以防止阈值出现抖动。

        public PlayerPosture playerPosture = PlayerPosture.Stand;

        public enum LocomotionState
        {
            Idle,
            Walk,
            Run
        }

        public LocomotionState locomotionState = LocomotionState.Idle;

        public enum ArmState
        {
            Normal,
            Pistol
        }

        public ArmState armState = ArmState.Normal;

        public float crouchSpeed = 1.5f;
        public float walkSpeed = 2.5f;
        public float runSpeed = 5.5f;

        public float rotateSpeed = 180f;

        public float jumpMaxHeight = 1.2f;
        public float fallGravityMultiplier = 1.5f;

        private int postureHash = Animator.StringToHash("玩家姿态");
        private int moveSpeedHash = Animator.StringToHash("移动速度");
        private int trunSpeedHash = Animator.StringToHash("转向");
        private int verticalSpeedHash = Animator.StringToHash("垂直速度");
        private Vector3 playerMovement;

        public int CACHE_SPEED_SIZE = 5;
        private Vector3[] _cacheSpeed;
        private int _cacheIndex = 0;
        private Vector3 _avgVec3;


        private Vector3 AverageVal(Vector3 newVel)
        {
            _cacheSpeed[_cacheIndex] = newVel;
            _cacheIndex++;
            _cacheIndex %= CACHE_SPEED_SIZE;

            Vector3 avg = Vector3.zero;
            foreach (var vec3 in _cacheSpeed) avg += vec3;
            return avg / CACHE_SPEED_SIZE;
        }

        private void Start()
        {
            _cacheSpeed = new Vector3[CACHE_SPEED_SIZE];
            _animator = GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            CaculateGround();
            SwitchPlayerStates();
            CaculateGravity();
            Jump();
            CaculateInputDirection();
            SetupAnimator();
        }

        #region 输入

        private Vector2 _inputMoveVec2;
        private bool _isInputRunning;
        private bool _isInputCrouch;
        private bool _isInputAiming;
        private bool _isInputJumping;
        private static readonly int FeetTween = Animator.StringToHash("左右脚");

        public void GetMoveInput(InputAction.CallbackContext ctx)
        {
            _inputMoveVec2 = ctx.ReadValue<Vector2>();
        }

        public void GetRunInput(InputAction.CallbackContext ctx)
        {
            _isInputRunning = ctx.ReadValueAsButton();
        }

        public void GetCrouchInput(InputAction.CallbackContext ctx)
        {
            _isInputCrouch = ctx.ReadValueAsButton();
        }

        public void GetAimInput(InputAction.CallbackContext ctx)
        {
            _isInputAiming = ctx.ReadValueAsButton();
        }

        public void GetJumpInput(InputAction.CallbackContext ctx)
        {
            _isInputJumping = ctx.ReadValueAsButton();
        }

        #endregion

        void SwitchPlayerStates()
        {
            if (!_isGround)
            {
                playerPosture = PlayerPosture.Midair;
            }
            else
            {
                playerPosture = PlayerPosture.Stand;
            }

            if (_inputMoveVec2.magnitude == 0)
            {
                locomotionState = LocomotionState.Idle;
            }
            else if (_isInputRunning)
            {
                locomotionState = LocomotionState.Run;
            }
            else
            {
                locomotionState = LocomotionState.Walk;
            }

            if (_isInputAiming)
            {
                armState = ArmState.Pistol;
            }
            else
            {
                armState = ArmState.Normal;
            }
        }

        void CaculateInputDirection()
        {
            // 计算角色相对于相机方向的移动

            var mainTransform = Camera.main.transform;
            var forward = mainTransform.forward;
            Vector3 camForwardProjection = new Vector3(forward.x, 0, forward.z).normalized;
            playerMovement = camForwardProjection * _inputMoveVec2.y + mainTransform.right * _inputMoveVec2.x;
            playerMovement = transform.InverseTransformVector(playerMovement); // 相机世界坐标，转换到角色本地坐标方向
            Debug.Log(playerMovement);
        }

        void SetupAnimator()
        {
            Debug.Log($"{playerPosture}");
            if (playerPosture == PlayerPosture.Stand)
            {
                _animator.SetFloat(postureHash, playerPostureStandThreshold, 0.1f, Time.deltaTime);
                switch (locomotionState)
                {
                    case LocomotionState.Idle:
                        _animator.SetFloat(moveSpeedHash, 0 * playerMovement.magnitude, 0.1f, Time.deltaTime);
                        break;
                    case LocomotionState.Walk:

                        _animator.SetFloat(moveSpeedHash, walkSpeed * playerMovement.magnitude, 0.1f, Time.deltaTime);
                        break;
                    case LocomotionState.Run:
                        _animator.SetFloat(moveSpeedHash, runSpeed * playerMovement.magnitude, 0.1f, Time.deltaTime);
                        break;
                }
            }
            else if (playerPosture == PlayerPosture.Midair)
            {
                // _animator.SetFloat(postureHash, playerPostureMidairThreshold, 0.1f, Time.deltaTime);
                _animator.SetFloat(postureHash, playerPostureMidairThreshold);
                // _animator.SetFloat(verticalSpeedHash, _currentVerticalVelocity, 0.1f, Time.deltaTime);
                _animator.SetFloat(verticalSpeedHash, _currentVerticalVelocity);
                
            }

            // 武器状态
            if (armState == ArmState.Normal)
            {
                float rad = Mathf.Atan2(playerMovement.x, playerMovement.z); // 转向方向 - 弧度
                // 使用OnAnimatorMove后转向也会失效，可以使用_characterController控制
                _animator.SetFloat(trunSpeedHash, rad, 0.1f, Time.deltaTime);
                transform.Rotate(0, rad * rotateSpeed * Time.deltaTime, 0f);
            }
        }

        private void OnAnimatorMove()
        {
            if (playerPosture == PlayerPosture.Midair)
            {
                // 使用地面时的速度，比如使用最后一帧的速度。更好的方法是使用离地前几帧的速度。
                // 使用动画的速度，那么不能使用deltaPosition，
                // 而需要使用velocity，因为deltaPosition是收到帧率影响的。我们并不能让空中的长时间动画被不确定的帧率影响。
                _avgVec3.y = _currentVerticalVelocity;
                Vector3 deltaMovement = _avgVec3 * Time.deltaTime;
                // Vector3 deltaMovement = _animator.deltaPosition;
                // deltaMovement.y = _currentVerticalVelocity * Time.deltaTime;
                _characterController.Move(deltaMovement);
            }
            else
            {
                // 使用动画里面的速度
                Vector3 deltaMovement = _animator.deltaPosition;
                deltaMovement.y = _currentVerticalVelocity * Time.deltaTime;
                _characterController.Move(deltaMovement);
                _avgVec3 = AverageVal(_animator.velocity);
            }
        }

        public void Jump()
        {
            if (_isGround && _isInputJumping)
            {
                // _currentVerticalVelocity = jumpSpeed;
                _currentVerticalVelocity = Mathf.Sqrt(-2 * gravity * jumpMaxHeight);
                
                // 随机左右脚 
                // 当前动画播放进度
                // var movementAni = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1);
                
                var feetTweenRandom = UnityEngine.Random.Range(-1f, 1f);
                _animator.SetFloat(FeetTween, feetTweenRandom);
            }
        }


        private float grounOffset = 0.5f;
        private bool _isGround = true;
        void CaculateGround()
        {
            if (Physics.SphereCast(transform.position + (Vector3.up * grounOffset), _characterController.radius,
                    Vector3.down,
                    out var hit, grounOffset - _characterController.radius + 2 * _characterController.skinWidth))
            {
                // 落到地面上
                _isGround = true;
            }
            else
            {
                _isGround = false;
            }
        }
        
        private void CaculateGravity()
        {
            if (_isGround)
            {
                _currentVerticalVelocity = gravity * Time.deltaTime;
            }
            else
            {
                if (_currentVerticalVelocity < 0) // 跌落状态，加速
                {
                    
                    _currentVerticalVelocity += gravity * fallGravityMultiplier * Time.deltaTime;
                }
                else
                {
                    _currentVerticalVelocity += gravity * Time.deltaTime;
                }
            }
        }
    }
}