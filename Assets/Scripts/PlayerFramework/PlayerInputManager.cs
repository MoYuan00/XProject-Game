using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PlayerFramework
{
    public class PlayerInputManager : MonoBehaviour
    {
        #region 输入

        public Vector2 inputMoveVec2 { get; private set; }
        public bool isInputRunning { get; private set; }
        public bool isInputAiming { get; private set; }
        public bool isInputJumping { get; private set; }
        
        /// <summary>
        /// 鼠标移动触发，
        /// 值：鼠标移动方向的相对像素个数
        /// </summary>
        public Vector2 mouseMoveDir { get; private set; } 

        public void GetMoveInput(InputAction.CallbackContext ctx)
        {
            inputMoveVec2 = ctx.ReadValue<Vector2>();
        }

        public void GetMouseRollInput(InputAction.CallbackContext ctx)
        {
            mouseMoveDir = ctx.ReadValue<Vector2>();
        }

        public void GetRunInput(InputAction.CallbackContext ctx)
        {
            isInputRunning = ctx.ReadValueAsButton();
        }

        public void GetAimInput(InputAction.CallbackContext ctx)
        {
            isInputAiming = ctx.ReadValueAsButton();
            if (ctx.phase == InputActionPhase.Started)
                onInputAim?.Invoke();
        }

        public void GetJumpInput(InputAction.CallbackContext ctx)
        {
            isInputJumping = ctx.ReadValueAsButton();
            if(ctx.phase == InputActionPhase.Started)
                onInputJump?.Invoke();
        }

        #endregion

        public Action onInputAim = null;
        public Action onInputJump = null;
    }
}