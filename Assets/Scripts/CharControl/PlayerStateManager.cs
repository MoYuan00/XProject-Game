using System;
using UnityEngine;

namespace CharControl
{
    [Flags]
    public enum CharMoveState
    {
        Idle = 0,
        PistolAim = 2, // 瞄准
        JumpingUp = 4, // 跳起中
        Air = 8, // 是否在空中
        WallRunning = 16,
        JumpingDown = 32
    }

    public class PlayerStateManager : MonoBehaviour
    {
        private CharMoveState _currentState;

        public void Append(CharMoveState moveState)
        {
            if(Exists(moveState)) return;
            this._currentState |= moveState;
            Debug.Log($"State: add '{moveState}'  all: {_currentState}");
        }

        public void Remove(CharMoveState moveState)
        {
            if (Exists(moveState))
            {
                var t = _currentState;
                _currentState -= moveState;
                Debug.Log($"State: remove '{moveState}'  all: {_currentState}");
            }
        }

        public bool Exists(CharMoveState moveState)
        {
            return (_currentState & moveState) == moveState;
        }
        
        public bool NotExists(CharMoveState moveState)
        {
            return !Exists(moveState);
        }
    }
}