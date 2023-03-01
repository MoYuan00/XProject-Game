using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameworkAnimation
{
    public enum AnimationStateName
    {
        JumpingUp,
        JumpingDown,
    }

    public class AnimationEventManager
    {
        private static readonly Dictionary<AnimationStateName, Action> _eventMap =
            new Dictionary<AnimationStateName, Action>();

        public delegate void OnStateExitDelegate(AnimationStateName stateEvent);

        #region 开放事件注册

        /// <summary>
        /// 动画播放结束事件
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="action"></param>
        public static void OnStateExit(AnimationStateName stateName, Action @action)
        {
            if (_eventMap.TryGetValue(stateName, out var value)) value += action;
            else value = action;
            _eventMap[stateName] = value;
        }

        #endregion


        #region AnimationStateEvent进行调用

        internal static void CallStateExit(AnimationStateName stateName)
        {
            if (_eventMap.TryGetValue(stateName, out var action)) action();
        }

        #endregion
    }
}