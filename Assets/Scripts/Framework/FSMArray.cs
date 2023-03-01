using System.Collections.Generic;
using UnityEngine;

namespace FrameworkFSM
{
    public class FSMArray<T>
    {
        private readonly Dictionary<T, IState> _states = new Dictionary<T, IState>();
        private readonly Dictionary<T, List<T>> _fsmArrayMap = new Dictionary<T, List<T>>();

        private List<T> _currentStateIds = new List<T>();
        private List<IState> _currentStates = new List<IState>();

        private List<IState> _updateTempStates = new List<IState>(); // 用于刷新的临时状态数组，防止更新时修改 导致混乱

        public FSMCustomState Register(T stateId)
        {
            if (_states.TryGetValue(stateId, out var state))
            {
                Debug.LogWarning($"cannot register, state '{stateId}' was registered!");
                return state as FSMCustomState;
            }

            Debug.Log($"Register state '{stateId}' success!");
            state = new FSMCustomState();
            _states.Add(stateId, state);
            return (FSMCustomState)state;
        }

        /// <summary>
        /// 注册这个可以并行的状态，并行状态在进行切换时会加入，而不是切换
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="joinStateIds">可并行的状态</param>
        /// <returns></returns>
        public FSMCustomState Register(T stateId, params T[] joinStateIds)
        {
            _fsmArrayMap.Add(stateId, new List<T>(joinStateIds));
            return Register(stateId);
        }
        

        private void DoChangeState(T stateId, IState state)
        {
            Exit();

            var t1 = _currentStates;
            var t2 = _currentStateIds;

            _currentStates = new List<IState> { state };
            _currentStateIds = new List<T> { stateId };

            t1.Clear();
            t2.Clear();

            Debug.Log($"DoChangeState to '{stateId}'");
            state?.Enter();
        }

        private void DoJoinState(T stateId, IState state)
        {
            var t1 = _currentStates;
            var t2 = _currentStateIds;

            _currentStates = new List<IState> { state };
            _currentStateIds = new List<T> { stateId };
            _currentStates.AddRange(t1);
            _currentStateIds.AddRange(t2);

            t1.Clear();
            t2.Clear();

            Debug.Log($"DoJoinState to '{stateId}'");
            state?.Enter();
        }

        /// <summary>
        /// 切换或者加入
        /// 1、同一状态是不可重入的，会忽略重复状态
        /// </summary>
        public void ChangeState(T stateId)
        {
            if (!_states.TryGetValue(stateId, out var state))
            {
                Debug.LogError($"Cannot found state '{stateId}'");
                return;
            }
            if(IsRunning(stateId)) return;

            if (CanJoinAllState(stateId))
            {
                // 是 可并行类型
                DoJoinState(stateId, state);
                return;
            }
            DoChangeState(stateId, state);
        }

        private bool CanJoinAllState(T stateId)
        {
            if (_currentStates.Count == 0) return false;
            // 当前运行中，全部可以和其进行并行，那就并行
            foreach (var itemStateId in _currentStateIds)
            {
                if(!_fsmArrayMap.TryGetValue(itemStateId, out var joinList)) return false;
                if (joinList == null || !joinList.Contains(stateId))
                {
                    Debug.LogWarning($"State '{itemStateId}' cannot join '{stateId}'");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 直接退出状态，由于可能存在多状态运行，运行提前退出而不切换
        /// </summary>
        /// <param name="stateId"></param>
        public void ExitState(T stateId)
        {
            if (!_states.TryGetValue(stateId, out var state)) return;

            _currentStateIds.Remove(stateId);
            _currentStates.Remove(state);
            
            _currentStateIds = new List<T>(_currentStateIds);
            _currentStates = new List<IState>(_currentStates);
            
            Debug.Log($"ExitState '{stateId}', exists states:{string.Join(",", _currentStateIds)}");
            state?.Exit();
        }

        public void Update()
        {
            _updateTempStates = _currentStates;
            for (var i = 0; i < _updateTempStates.Count; i++) _updateTempStates[i]?.Update();
        }

        public void FixUpdate()
        {
            _updateTempStates = _currentStates;
            for (var i = 0; i < _updateTempStates.Count; i++) _updateTempStates[i]?.FixUpdate();
        }

        private void Exit()
        {
            _updateTempStates = _currentStates;
            for (var i = 0; i < _updateTempStates.Count; i++) _updateTempStates[i]?.Exit();
        }

        public bool IsRunning(T stateId)
        {
            return _currentStateIds.Contains(stateId);
        }
        
        /// <summary>
        /// 其中一个正在运行，就返回true
        /// </summary>
        /// <param name="stateIds"></param>
        /// <returns></returns>
        public bool IsRunningAny(params T[] stateIds)
        {
            foreach (var stateId in stateIds)
                if (IsRunning(stateId)) return true;
            return false;
        }

        public void Clear()
        {
            _states.Clear();
            _fsmArrayMap.Clear();
            _currentStates.Clear();
            _currentStateIds.Clear();
        }
    }
}