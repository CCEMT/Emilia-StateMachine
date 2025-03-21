using System;
using System.Collections.Generic;
using Emilia.Reference;
using UnityEngine;

namespace Emilia.StateMachine
{
    /// <summary>
    /// 状态资产
    /// </summary>
    [Serializable]
    public class StateAsset
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private List<TransitionAsset> _transitionAssets;

        [SerializeField]
        private List<IStateComponentAsset> _componentAssets;

        /// <summary>
        /// 状态ID
        /// </summary>
        public int id => this._id;

        /// <summary>
        /// 过渡资产
        /// </summary>
        public IReadOnlyList<TransitionAsset> transitionAssets => this._transitionAssets;

        /// <summary>
        /// 组件资产
        /// </summary>
        public IReadOnlyList<IStateComponentAsset> componentAssets => this._componentAssets;

        public StateAsset(int id, List<TransitionAsset> transitionAssets, List<IStateComponentAsset> componentAssets)
        {
            this._id = id;
            this._transitionAssets = transitionAssets;
            this._componentAssets = componentAssets;
        }
    }

    public class State : IReference
    {
        private StateAsset _asset;
        private List<IStateComponent> _stateComponents = new List<IStateComponent>();
        private List<Transition> _transforms = new List<Transition>();

        /// <summary>
        /// 状态ID
        /// </summary>
        public int id => this._asset.id;

        /// <summary>
        /// 组件列表
        /// </summary>
        public IReadOnlyList<IStateComponent> stateComponents => this._stateComponents;

        /// <summary>
        /// 过渡列表
        /// </summary>
        public IReadOnlyList<Transition> transforms => _transforms;

        /// <summary>
        /// 初始化状态
        /// </summary>
        public void Init(StateAsset stateAsset, StateMachine stateMachine)
        {
            this._asset = stateAsset;
            _stateComponents.Clear();
            _transforms.Clear();

            InitComponent(stateMachine);
            InitTransform();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitComponent(StateMachine stateMachine)
        {
            int amount = this._asset.componentAssets.Count;
            for (int i = 0; i < amount; i++)
            {
                IStateComponentAsset componentAsset = this._asset.componentAssets[i];
                IStateComponent component = componentAsset.CreateComponent();
                component.Init(componentAsset, stateMachine);
                this._stateComponents.Add(component);
            }
        }

        /// <summary>
        /// 初始化Transform
        /// </summary>
        private void InitTransform()
        {
            int amount = this._asset.transitionAssets.Count;
            for (int i = 0; i < amount; i++)
            {
                TransitionAsset transitionAsset = this._asset.transitionAssets[i];
                Transition transition = ReferencePool.Acquire<Transition>();
                transition.Init(transitionAsset);
                this._transforms.Add(transition);
            }
        }

        /// <summary>
        /// 进入状态
        /// </summary>
        public void Enter(StateMachine stateMachine)
        {
            int amount = this._stateComponents.Count;
            for (int i = 0; i < amount; i++)
            {
                IStateComponent component = this._stateComponents[i];
                component.Enter(stateMachine);
            }
        }

        /// <summary>
        /// 退出状态
        /// </summary>
        public void Exit(StateMachine stateMachine)
        {
            int amount = this._stateComponents.Count;
            for (int i = 0; i < amount; i++)
            {
                IStateComponent component = this._stateComponents[i];
                component.Exit(stateMachine);
            }
        }

        /// <summary>
        /// 获取下一个状态ID
        /// </summary>
        public int GetNextStateId(StateMachine stateMachine)
        {
            int amount = transforms.Count;
            for (int i = 0; i < amount; i++)
            {
                Transition transition = transforms[i];
                if (transition.IsSuccess(stateMachine) == false) continue;
                return transition.nextId;
            }

            return default;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        public void Update(StateMachine stateMachine)
        {
            int amount = this._stateComponents.Count;
            for (int i = 0; i < amount; i++)
            {
                IStateComponent component = this._stateComponents[i];
                component.Update(stateMachine);
            }
        }

        /// <summary>
        /// 销毁状态
        /// </summary>
        public void Dispose(StateMachine stateMachine)
        {
            int stateAmount = this._stateComponents.Count;
            for (int i = 0; i < stateAmount; i++)
            {
                IStateComponent component = this._stateComponents[i];
                component.Dispose(stateMachine);
            }

            int transformAmount = this._transforms.Count;
            for (int i = 0; i < transformAmount; i++)
            {
                Transition transition = this._transforms[i];
                transition.Dispose();
            }

            ReferencePool.Release(this);
        }

        /// <summary>
        /// 清理状态
        /// </summary>
        void IReference.Clear()
        {
            this._asset = null;
            this._stateComponents.Clear();
            this._transforms.Clear();
        }
    }
}