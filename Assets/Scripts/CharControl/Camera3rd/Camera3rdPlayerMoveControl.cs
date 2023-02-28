using Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CharControl
{
    public class Camera3rdPlayerMoveControl : MonoBehaviour
    {
        private FSM<CharMoveState> _fsm = new FSM<CharMoveState>();
        private Transform _transform;

        public CharacterController characterController;
        public Animator animator;
        public Transform bodyCenter;

        private static readonly int PistolAim = Animator.StringToHash("PistolAim");
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");

        public float runningSpeed = 3.442973f;
        private float _targetSpeed = 0f;


        private void Awake()
        {
            _transform = transform;
            _fsm.State(CharMoveState.Idle).OnEnter(() => { Debug.Log("Idle OnEnter"); });
            _fsm.StartState(CharMoveState.Idle);

            _fsm.State(CharMoveState.PistolAim)
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

            _fsm.State(CharMoveState.Running)
                .OnEnter(() => { animator.SetBool(IsRunning, true); })
                .OnUpdate(() => { MovePlayer(); })
                .OnExit(() => { animator.SetBool(IsRunning, false); });
        }

        private void Update()
        {
            _fsm.Update();
        }

        private void OnDestroy()
        {
            _fsm.Clear();
        }

        private void MovePlayer()
        {
            if (_targetSpeed <= 0) return;
            characterController.SimpleMove(_transform.forward * _targetSpeed);
            _targetSpeed = Mathf.Lerp(_targetSpeed, runningSpeed, 0.5f); // 插值
        }


        #region InputEvent

        public void OnMoveInput(InputAction.CallbackContext ctx)
        {
            switch (ctx.phase)
            {
                case InputActionPhase.Started:
                    return;
                case InputActionPhase.Canceled:
                    _fsm.ChangeState(CharMoveState.Idle);
                    break;
                default:
                    _fsm.ChangeState(CharMoveState.Running);
                    break;
            }
        }

        public void OnPistolAim(InputAction.CallbackContext ctx)
        {
            if (ctx.phase != InputActionPhase.Performed) return;
            _fsm.ChangeState(_fsm.CurrentStateId == CharMoveState.PistolAim
                ? CharMoveState.Idle
                : CharMoveState.PistolAim);
        }

        #endregion
    }

    public enum CharMoveState
    {
        Idle,
        Moving, // 移动
        PistolAim, // 瞄准
        Running, // 奔跑中
    }
}