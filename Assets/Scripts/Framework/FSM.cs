using System;
using System.Collections.Generic;


namespace Framework
{
    public class FSM<T>  // T state type
    {
        private Dictionary<T, IState> _states = new Dictionary<T, IState>();

        private IState _currentState;
        private T _currentStateId;

        public IState CurrentState => _currentState;
        public T CurrentStateId => _currentStateId;

        /// <summary>
        /// 注册状态机
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public FSMCustomState State(T t)
        {
            if (_states.ContainsKey(t))
            {
                return _states[t] as FSMCustomState;
            }

            var state = new FSMCustomState();
            _states.Add(t, state);
            return state;
        }

        public void ChangeState(T t)
        {
            if ((_currentStateId).Equals(t)) return;
            ForceChangeState(t);
        }

        public void ForceChangeState(T t)
        {
            if (_states.TryGetValue(t, out var state))
            {
                _currentState?.Exit(); // 退出上一个状态
                _currentStateId = t;
                _currentState = state; // 进入下一个状态
                _currentState.Enter();
            }
        }

        public void StartState(T t)
        {
            if (_states.TryGetValue(t, out var state))
            {
                _currentState = state;
                _currentStateId = t;
                state.Enter();
            }
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void FixUpdate()
        {
            _currentState?.FixUpdate();
        }

        /// <summary>
        /// Clear时，如还有状态在运行中，这里不会再管理。直接clear。
        /// 也就是说如果在状态运行时发生clear，那么状态的OnExit不会被执行到
        /// </summary>
        public void Clear()
        {
            _currentState = null;
            _currentStateId = default;
            _states.Clear();
        }
    }
}