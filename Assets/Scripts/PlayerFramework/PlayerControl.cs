using FrameworkAnimation;
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
                    _movement.RotateWithInputCameraSpace(_input.inputMoveVec2);
                    _anim.Walk(_movement.Speed * _movement.walkSpeed);
                })
                .OnExit(() => { _movement.RotateWithInputCameraSpace(Vector2.zero); });

            _state.movementFSM.State(PlayerMovementState.Run)
                .OnUpdate(() =>
                {
                    _movement.RotateWithInputCameraSpace(_input.inputMoveVec2);
                    _anim.Run(_movement.Speed * _movement.runSpeed);
                }).OnExit(() => { _movement.RotateWithInputCameraSpace(Vector2.zero); });

            _state.movementFSM.State(PlayerMovementState.Jump)
                .OnEnter(() =>
                {
                    _anim.Jump();
                    _movement.Jump();
                    Invoke(nameof(CoolDownJump), 0.1f);
                })
                .OnFixUpdate(() =>
                {
                    _movement.RotateWithInputCameraSpace(_input.inputMoveVec2);
                    _movement.AirMovementWithInput(_input.inputMoveVec2);
                }).OnExit(() =>
                {
                    Invoke(nameof(ResetJump), jumpCooldown); 
                });

            _state.positionFSM.State(PlayerPositionState.FixedAir)
                .OnEnter(() => { _movement.speedY = 0; });
            _state.movementFSM.State(PlayerMovementState.WallClamp)
                .OnEnter(() =>
                {
                    Invoke(nameof(CooldownClimb), 0.1f);
                    // ?????????????????????????????????
                    // ????????????????????????????????????????????????
                    // ????????????
                    // TODO ????????????????????????
                    transform.position = _env.climbingWall.hitReltivePosition;
                    transform.forward = -_env.climbingWall.hisPositionNormal;
                    _anim.Climb(0f);
                    _movement.isEnableRootMotionMove = false;
                })
                .OnUpdate(() =>
                {
                    // ????????????????????????????????????????????????????????????????????????????????????
                    if (_env.CheckTopHasSpace())
                    {
                        _state.movementFSM.ChangeState(PlayerMovementState.ClimbUp);
                        return;
                    }
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
                .OnEnter(() =>
                {
                    CameraManager.ChangeCameraState(CameraMode.WalkingAim);
                    _anim.Aim(0f);
                })
                .OnUpdate(() =>
                {
                    _movement.RotateFollowMouse(_input.mouseMoveDir);
                    _anim.Aim(_movement.aimWalkSpeed * _input.inputMoveVec2.y);
                })
                .OnExit(() =>
                {
                    _anim.AimEnd(); 
                    CameraManager.ChangeCameraState(CameraMode.Free);
                });

            _state.movementFSM.State(PlayerMovementState.ClimbUp)
                .OnEnter(() =>
                {
                    _anim.CleanMoveSpeed();
                    _anim.ClimbUp();
                    _movement.isSyncAnimatorSpeedY = true;
                    // _movement.isEnableRootMotionMove = true;
                    _movement.speedY = 0;
                })
                .OnExit(() =>
                {
                    _movement.isSyncAnimatorSpeedY = false;
                });
            AnimationEventManager.OnStateExit(AnimationStateName.ClimbUp, () =>
            {
                _state.movementFSM.Exit(PlayerMovementState.ClimbUp);
                _state.positionFSM.Exit(PlayerPositionState.FixedAir);
            });
            
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
                    // ???????????????????????? ????????????
                    // TODO Bug - ????????????????????????????????????????????????????????????????????????????????? ?????????????????????
                    transform.position -= transform.forward * 0.2f;

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
            else if (_state.positionFSM.IsRunning(PlayerPositionState.FixedAir))
            {
                _state.positionFSM.ChangeState(PlayerPositionState.FixedAir);
            }
            else
            {
                _state.positionFSM.ChangeState(PlayerPositionState.MidAir);
            }

            if (isGrounded) // ???????????????????????????
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
                // ?????????
                if (_readyClimb && _env.CheckFaceWall()) // ?????????
                {
                    // ????????????
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