using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameworkFSM
{
    public class FSMCustomState : IState
    {
        private Action _onEnter;
        private Action _onUpdate;
        private Action _onFixUpdate;
        private Action _onExit;

        public FSMCustomState OnEnter(Action action)
        {
            _onEnter = action;
            return this;
        }

        public FSMCustomState OnUpdate(Action action)
        {
            _onUpdate = action;
            return this;
        }

        public FSMCustomState OnFixUpdate(Action action)
        {
            _onFixUpdate = action;
            return this;
        }

        /// <summary>
        /// OnExit 用来在退出时清理某些变量或者值
        /// 不允许切换状态，，否则会造成递归调用
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public FSMCustomState OnExit(Action action)
        {
            _onExit = action;
            return this;
        }

        IState IState.Enter()
        {
            _onEnter?.Invoke();
            return this;
        }

        IState IState.Update()
        {
            // _onUpdate?.Method.Invoke(null, null);
            _onUpdate?.Invoke();
            return this;
        }

        IState IState.FixUpdate()
        {
            _onFixUpdate?.Invoke();
            return this;
        }

        IState IState.Exit()
        {
            _onExit?.Invoke();
            return this;
        }
    }
}