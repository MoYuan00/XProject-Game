using UnityEngine;

namespace PlayerFramework
{
    [RequireComponent(typeof(PlayerInputManager))]
    [RequireComponent(typeof(PlayerStateManager))]
    [RequireComponent(typeof(PlayerEnvManager))]
    [RequireComponent(typeof(PlayerAnimatorManager))]
    [RequireComponent(typeof(PlayerMovementManager))]
    public class PlayerControl : MonoBehaviour
    {
        private PlayerInputManager _input;
        private PlayerStateManager _state;
        private PlayerEnvManager _env;
        private PlayerAnimatorManager _anim;
        private PlayerMovementManager _movement;

        private bool _readyToJump = true;
        public float jumpCooldown = 0.2f;
        public float climbCooldown = 0.2f;
        public bool _readyClimb = true;
        public float airASpeedMult = 1.5f;

        private Vector3 _hitPos;
        private Vector3 _hitDir;

        private void Start()
        {
            _input = GetComponent<PlayerInputManager>();
            _state = GetComponent<PlayerStateManager>();
            _env = GetComponent<PlayerEnvManager>();
            _anim = GetComponent<PlayerAnimatorManager>();
            _movement = GetComponent<PlayerMovementManager>();

            _state.positionFSM.State(PlayerPositionState.Grounded);
            // .OnUpdate(() => { movementManager.speedY = (movementManager.gravity * Time.deltaTime); });

            _state.positionFSM.State(PlayerPositionState.MidAir)
                .OnUpdate(() =>
                {
                    _movement.AmountGravityToSpeedY(airASpeedMult);
                    _anim.MidAir_Posture(_movement.speedY);
                });

            _state.movementFSM.State(PlayerMovementState.Idle)
                .OnUpdate(() => { _anim.Idle(); });

            _state.movementFSM.State(PlayerMovementState.Walk)
                .OnUpdate(() =>
                {
                    _movement.Rotate(_input.inputMoveVec2);
                    _anim.Walk(_movement.Speed * _movement.walkSpeed);
                })
                .OnExit(() => { _movement.Rotate(Vector2.zero); });

            _state.movementFSM.State(PlayerMovementState.Run)
                .OnUpdate(() =>
                {
                    _movement.Rotate(_input.inputMoveVec2);
                    _anim.Run(_movement.Speed * _movement.runSpeed);
                }).OnExit(() => { _movement.Rotate(Vector2.zero); });

            _state.movementFSM.State(PlayerMovementState.Jump)
                .OnEnter(() =>
                {
                    _anim.Jump();
                    _movement.Jump();
                    Invoke(nameof(CoolDownJump), 0.1f);
                })
                .OnFixUpdate(() =>
                {
                    _movement.Rotate(_input.inputMoveVec2);
                    _movement.AirMovementWithInput(_input.inputMoveVec2);
                }).OnExit(() => { Invoke(nameof(ResetJump), jumpCooldown); });

            _state.positionFSM.State(PlayerPositionState.FixedAir)
                .OnEnter(() => { _movement.speedY = 0; });
            _state.movementFSM.State(PlayerMovementState.WallClamp)
                .OnEnter(() =>
                {
                    Invoke(nameof(CooldownClimb), 0.1f);
                    // 在空中的时候允许爬墙。
                    // 墙壁碰撞点，固定当前角色到碰撞点
                    // 播放动画
                    // TODO 角色需要面向墙壁
                    var pos = _hitPos;
                    transform.position = pos;
                    transform.forward = _hitDir;
                    _anim.Climb(0f);
                    _movement.isEnableRootMotionMove = false;
                })
                .OnUpdate(() =>
                {
                    _movement.ClimbWithInput(_input.inputMoveVec2);
                    _anim.Climb(_movement.speedY);
                })
                .OnExit(() =>
                {
                    Invoke(nameof(ResetClimb), climbCooldown);
                    _anim.ClimbEnd();
                    _movement.isEnableRootMotionMove = true;
                });

            _state.movementFSM.State(PlayerMovementState.Aim)
                .OnEnter(() => { _anim.Aim(0f); })
                .OnUpdate(() =>
                {
                    _movement.Rotate(_input.inputMoveVec2);
                    _anim.Aim(_movement.aimWalkSpeed * _input.inputMoveVec2.y);
                })
                .OnExit(() => { _anim.AimEnd(); });

            OnInputAim();
            OnInputJump();
        }

        private void Update()
        {
            SwitchPlayerStates();
            _state.positionFSM.Update();
            _state.movementFSM.Update();
        }

        private void FixedUpdate()
        {
            _state.positionFSM.FixUpdate();
            _state.movementFSM.FixUpdate();
        }

        void OnInputAim()
        {
            _input.onInputAim = () =>
            {
                if (_state.movementFSM.IsRunning(PlayerMovementState.Aim))
                    _state.movementFSM.Exit(PlayerMovementState.Aim);
                else
                    _state.movementFSM.ChangeState(PlayerMovementState.Aim);
            };
        }

        void OnInputJump()
        {
            _input.onInputJump = () =>
            {
                if (_state.movementFSM.IsRunning(PlayerMovementState.WallClamp))
                {
                    // 按下跳跃键，脱离 爬墙状态
                    // TODO Bug - 跳墙会重新进入爬墙状态，这里应该让物体离开墙体一段距离 防止再次爬墙。
                    transform.position += -_hitDir * 0.2f;

                    _state.movementFSM.Exit(PlayerMovementState.WallClamp);
                    _state.positionFSM.Exit(PlayerPositionState.FixedAir);
                    _state.positionFSM.ChangeState(PlayerPositionState.MidAir);
                    return;
                }
                
                if (_readyToJump && _env.CheckIsGrounded())
                {
                    _state.movementFSM.ChangeState(PlayerMovementState.Jump);
                    return;
                }
            };
        }

        void SwitchPlayerStates()
        {
            bool isGrounded = _env.CheckIsGrounded();

            if (isGrounded && !_readyToJump)
            {
                _state.movementFSM.Exit(PlayerMovementState.Jump);
            }

            if (isGrounded && _state.movementFSM.IsNotRunning(PlayerMovementState.Jump))
                _state.positionFSM.ChangeState(PlayerPositionState.Grounded);
            else if (_env.CheckFaceWall(out var t, out var t2) &&
                     _state.positionFSM.IsRunning(PlayerPositionState.FixedAir))
            {
                _state.positionFSM.ChangeState(PlayerPositionState.FixedAir);
            }
            else
            {
                _state.positionFSM.ChangeState(PlayerPositionState.MidAir);
            }

            if (isGrounded) // 落地才允许进行移动
            {
                if (_state.movementFSM.IsRunning(PlayerMovementState.Jump))
                    _state.movementFSM.ChangeState(PlayerMovementState.Jump);
                else if (_state.movementFSM.IsRunning(PlayerMovementState.Aim))
                    _state.movementFSM.ChangeState(PlayerMovementState.Aim);
                else if (_input.inputMoveVec2.magnitude == 0)
                    _state.movementFSM.ChangeState(PlayerMovementState.Idle);
                else if (_input.isInputRunning)
                    _state.movementFSM.ChangeState(PlayerMovementState.Run);
                else
                    _state.movementFSM.ChangeState(PlayerMovementState.Walk);
            }

            if (_state.positionFSM.IsRunning(PlayerPositionState.MidAir))
            {
                // 在空中
                if (_readyClimb && _env.CheckFaceWall(out _hitPos, out _hitDir)) // 碰到墙
                {
                    // 触发爬墙
                    _state.movementFSM.ChangeState(PlayerMovementState.WallClamp);
                    _state.positionFSM.ChangeState(PlayerPositionState.FixedAir);
                }
            }
        }


        #region Cooldown

        private void CoolDownJump()
        {
            _readyToJump = false;
        }

        private void ResetJump()
        {
            _readyToJump = true;
        }

        private void CooldownClimb()
        {
            _readyClimb = false;
        }

        private void ResetClimb()
        {
            _readyClimb = true;
        }

        #endregion
    }
}