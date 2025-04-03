using System;
using System.Collections.Generic;
using Emilia.Reference;
using Emilia.Variables;
using UnityEngine;

namespace Emilia.StateMachine
{
    /// <summary>
    /// 状态机资产
    /// </summary>
    [Serializable]
    public class StateMachineAsset
    {
        [SerializeField]
        private string _id;

        [SerializeField]
        private string _description;

        [SerializeField]
        private VariablesManage _variablesManage;

        [SerializeField]
        private int _enterStateId;

        [SerializeField]
        private List<StateAsset> _stateAssets;

        [SerializeField]
        private object _userData;

        public string id => this._id;

        /// <summary>
        /// 描述
        /// </summary>
        public string description => this._description;

        /// <summary>
        /// 参数
        /// </summary>
        public VariablesManage variablesManage => this._variablesManage;

        /// <summary>
        /// 进入状态Id
        /// </summary>
        public int enterStateId => this._enterStateId;

        /// <summary>
        /// 状态资产
        /// </summary>
        public IReadOnlyList<StateAsset> stateAssets => this._stateAssets;

        /// <summary>
        /// 用户数据
        /// </summary>
        public object userData
        {
            get => this._userData;
            internal set => this._userData = value;
        }

        public StateMachineAsset(string id, string description, VariablesManage variablesManage, int enterStateId, List<StateAsset> stateAssets, object userData)
        {
            this._id = id;
            this._description = description;
            this._variablesManage = variablesManage;
            this._enterStateId = enterStateId;
            this._stateAssets = stateAssets;
            this._userData = userData;
        }
    }

    public class StateMachine : IReference
    {
        /// <summary>
        /// 运行状态
        /// </summary>
        public enum RunState
        {
            /// <summary>
            /// 无
            /// </summary>
            None,

            /// <summary>
            /// 运行中
            /// </summary>
            Running,

            /// <summary>
            /// 关闭
            /// </summary>
            Close,
        }

        private List<State> _statesList = new List<State>();
        private Dictionary<int, State> _statesMap = new Dictionary<int, State>();

        public int uid { get; private set; }
        public StateMachineAsset asset { get; private set; }
        public object owner { get; private set; }

        /// <summary>
        /// 父状态机
        /// </summary>
        public StateMachine parentStateMachine { get; private set; }

        /// <summary>
        /// 子状态机
        /// </summary>
        public List<StateMachine> children { get; private set; } = new List<StateMachine>();

        /// <summary>
        /// 用户定义的参数
        /// </summary>
        public VariablesManage userVariablesManage { get; private set; }

        /// <summary>
        /// 用户数据
        /// </summary>
        public object userData { get; private set; }

        /// <summary>
        /// 状态机运行状态
        /// </summary>
        public RunState runState { get; private set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public State currentState { get; private set; }

        /// <summary>
        /// 当前状态参数（内置）（状态切换时参数会被清理）
        /// </summary>
        public VariablesManage stateVariablesManage { get; private set; } = new VariablesManage();

        /// <summary>
        /// 状态机参数（内置）
        /// </summary>
        public VariablesManage stateMachineVariablesManage { get; private set; } = new VariablesManage();

        public IReadOnlyList<State> statesList => this._statesList;
        public IReadOnlyDictionary<int, State> statesMap => this._statesMap;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(int uid, StateMachineAsset stateMachineAsset, object owner = default, StateMachine parentStateMachine = default)
        {
            this.uid = uid;
            asset = stateMachineAsset;
            this.owner = owner;
            this.parentStateMachine = parentStateMachine;

            userVariablesManage = stateMachineAsset.variablesManage.Clone();
            runState = RunState.None;
            currentState = default;
            stateVariablesManage.Clear();
            stateMachineVariablesManage.Clear();

            if (parentStateMachine != null) userVariablesManage.parentManage = parentStateMachine.userVariablesManage;

            InitState();
        }

        private void InitState()
        {
            int amount = asset.stateAssets.Count;
            for (int i = 0; i < amount; i++)
            {
                StateAsset stateAsset = asset.stateAssets[i];
                AddState(stateAsset);
            }
        }

        private void AddState(StateAsset stateAsset)
        {
            State state = ReferencePool.Acquire<State>();
            state.Init(stateAsset, this);
            this._statesList.Add(state);
            this._statesMap.Add(state.id, state);
        }

        /// <summary>
        /// 开启状态机
        /// </summary>
        public void Start()
        {
            runState = RunState.Running;
            SwitchState(asset.enterStateId);
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void SwitchState(int stateId)
        {
            if (this._statesMap.ContainsKey(stateId) == false) return;
            if (currentState != default) currentState.Exit(this);
            stateVariablesManage.Clear();
            currentState = this._statesMap[stateId];
            currentState.Enter(this);
        }

        public void Update()
        {
            if (currentState == default) return;

            int nextStateId = currentState.GetNextStateId(this);
            if (nextStateId != 0)
            {
                SwitchState(nextStateId);
                return;
            }

            currentState.Update(this);
        }

        /// <summary>
        /// 关闭状态机
        /// </summary>
        public void Dispose()
        {
            if (runState != RunState.Running) return;
            runState = RunState.Close;

            int stateAmount = this._statesList.Count;
            for (int i = 0; i < stateAmount; i++)
            {
                State state = this._statesList[i];
                state.Dispose(this);
            }

            ReferencePool.Release(this);
        }

        void IReference.Clear()
        {
            uid = -1;
            asset = null;
            owner = null;

            parentStateMachine = null;
            children.Clear();

            currentState = null;
            userData = null;

            userVariablesManage.Clear();
            stateVariablesManage.Clear();
            stateMachineVariablesManage.Clear();
            this._statesList.Clear();
            this._statesMap.Clear();
        }
    }
}