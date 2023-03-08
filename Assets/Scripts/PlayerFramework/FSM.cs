using System;
using System.Collections.Generic;
using UnityEngine;


namespace FrameworkFSM
{
    public class FSM<T> // T state type
    {
        protected Dictionary<T, IState> _states = new Dictionary<T, IState>();

        protected IState _currentState;
        protected T _currentStateId;


        /// <summary>
        /// 注册状态机
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual FSMCustomState State(T t)
        {
            if (_states.ContainsKey(t))
            {
                return _states[t] as FSMCustomState;
            }

            var state = new FSMCustomState();
            _states.Add(t, state);
            return state;
        }

        public virtual void ChangeState(T t)
        {
            if ((_currentStateId).Equals(t) && _currentState != null) return;
            Debug.Log($"ChangeState To : '{t}'");
            ForceChangeState(t);
        }

        public void Exit(T t)
        {
            if (!(_currentStateId).Equals(t)) return;
            Debug.Log($"ExitState : '{_currentStateId}'");
            _currentState?.Exit(); // 退出上一个状态
            _currentState = null;
            _currentStateId = default;
        }

        public virtual void ForceChangeState(T t)
        {
            _currentState?.Exit(); // 退出上一个状态
            if (_states.TryGetValue(t, out var state))
            {
                _currentStateId = t;
                _currentState = state; // 进入下一个状态
            }

            _currentState?.Enter();
        }

        public virtual void StartState(T t)
        {
            if (_states.TryGetValue(t, out var state))
            {
                _currentState = state;
                _currentStateId = t;
                state.Enter();
            }
        }

        public virtual void Update()
        {
            _currentState?.Update();
        }

        public virtual void FixUpdate()
        {
            _currentState?.FixUpdate();
        }

        public virtual bool IsRunning(T stateId)
        {
            return stateId.Equals(_currentStateId) && _currentState != null;
        }
        
        public virtual bool IsNotRunning(T stateId)
        {
            return !IsRunning(stateId);
        }

        /// <summary>
        /// Clear时，如还有状态在运行中，这里不会再管理。直接clear。
        /// 也就是说如果在状态运行时发生clear，那么状态的OnExit不会被执行到
        /// </summary>
        public virtual void Clear()
        {
            _currentState = null;
            _currentStateId = default;
            _states.Clear();
        }
    }
}