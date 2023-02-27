using Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CharControl
{
    public class YokuYPlayerControlFBX : MonoBehaviour
    {
        private FSM<CharMoveState> _fsm = new FSM<CharMoveState>();
        private Vector2 _moveVec2;
        private Transform _transform;
        public float rotateSpeed = 100f;

        public CharacterController characterController;

        public float walkingSpeed = 1.6f;
        private float _targetSpeed;
        private static readonly int WalkingSpeed = Animator.StringToHash("WalkingSpeed");

        public Animator animator;
        private static readonly int PistolAim = Animator.StringToHash("PistolAim");
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");

        public float runningSpeed = 3.442973f;

        private void Awake()
        {
            _transform = transform;
            _fsm.State(CharMoveState.Idle).OnEnter(() => { Debug.Log("Idle OnEnter"); });
            _fsm.StartState(CharMoveState.Idle);

            _fsm.State(CharMoveState.Moving)
                .OnUpdate(() =>
                {
                    RotatePlayer();
                    MovePlayer();
                }).OnExit(()=>_targetSpeed = 0f);

            _fsm.State(CharMoveState.PistolAim)
                .OnEnter(() =>
                {
                    animator.SetBool(PistolAim, true);
                    
                })
                .OnExit(() =>
                {
                    animator.SetBool(PistolAim, false);
                });
            
            _fsm.State(CharMoveState.Running)
                .OnEnter(() =>
                {
                    animator.SetBool(IsRunning, true);
                })
                .OnUpdate(() =>
                {
                    RotatePlayer();
                    MovePlayer();
                })
                .OnExit(() =>
                {
                    animator.SetBool(IsRunning, false);
                });
        }

        private void Update()
        {
            _fsm.Update();
        }

        private void FixedUpdate()
        {
            _fsm.FixUpdate();
        }

        public void OnMoveInput(InputAction.CallbackContext ctx)
        {
            Debug.Log("OnMoveInput");
            if (ctx.phase == InputActionPhase.Started) return;

            if (ctx.phase == InputActionPhase.Canceled)
            {
                _fsm.ChangeState(CharMoveState.Idle);
            }
            else
            {
                _moveVec2 = ctx.ReadValue<Vector2>();
                _fsm.ChangeState(CharMoveState.Running);
            }
        }

        public void OnAim(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Performed)
            {
                if (_fsm.CurrentStateId == CharMoveState.PistolAim)
                {
                    _fsm.ChangeState(CharMoveState.Idle);
                    return;
                }
                _fsm.ChangeState(CharMoveState.PistolAim);
            }
        }

        private void MovePlayer()
        {
            characterController.SimpleMove(transform.forward * _targetSpeed);
            
            _targetSpeed = Mathf.Lerp(_targetSpeed, walkingSpeed, 0.5f); // 插值
        }

        private void RotatePlayer()
        {
            var rotateVec = new Vector3(_moveVec2.x, 0, _moveVec2.y);
            Quaternion target = Quaternion.LookRotation(rotateVec, Vector3.up);
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation,
                target, rotateSpeed * Time.deltaTime);
        }

        private void OnDestroy()
        {
            _fsm.Clear();
        }
    }

    public enum CharMoveState
    {
        Idle,
        Moving, // 移动
        PistolAim, // 瞄准
        Running, // 奔跑中
    }
}