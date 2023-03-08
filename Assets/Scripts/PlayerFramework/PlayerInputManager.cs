using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        public void GetMoveInput(InputAction.CallbackContext ctx)
        {
            inputMoveVec2 = ctx.ReadValue<Vector2>();
        }

        public void GetRunInput(InputAction.CallbackContext ctx)
        {
            isInputRunning = ctx.ReadValueAsButton();
        }

        public void GetAimInput(InputAction.CallbackContext ctx)
        {
            isInputAiming = ctx.ReadValueAsButton();
        }

        public void GetJumpInput(InputAction.CallbackContext ctx)
        {
            isInputJumping = ctx.ReadValueAsButton();
        }

        #endregion
    }
}