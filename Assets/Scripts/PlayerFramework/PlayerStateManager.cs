using System;
using System.Collections;
using System.Collections.Generic;
using FrameworkFSM;
using UnityEngine;

namespace PlayerFramework
{
    /// <summary>
    /// 将角色状态分为如下几类：
    /// 1、角色的垂直位置：是落在地面，还是在空中
    /// 2、角色的移动状态：是idle，还是walk，还是run，jump、持枪、爬墙
    /// 4、角色的周围环境位置：是否面对墙壁，是否左右有墙壁
    /// </summary>
    public class PlayerStateManager : MonoBehaviour
    {
        public readonly FSM<PlayerPositionState> positionFSM = new FSM<PlayerPositionState>();
        public readonly FSM<PlayerMovementState> movementFSM = new FSM<PlayerMovementState>();
    }

    [Flags]
    public enum PlayerPositionState
    {
        Grounded = 0,
        MidAir = 1 << 1,
        FixedAir = 1 << 2,
    }

    [Flags]
    public enum PlayerMovementState
    {
        Idle = 0,
        Walk = 1 << 1,
        Run = 1 << 2,
        Jump = 1 << 3,
        WallClamp = 1 << 4, // 爬墙
        Aim, // 瞄准-持枪
    }

    // public enum PlayerEnvState
    // {
    // }
}