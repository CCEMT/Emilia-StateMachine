using System;
using Emilia.Reference;
using UnityEngine;

namespace Emilia.StateMachine
{
    /// <summary>
    /// 组件资产的抽象实现
    /// </summary>
    [Serializable]
    public abstract class StateComponentAsset<T> : IStateComponentAsset where T : class, IStateComponent, new()
    {
        [SerializeField, HideInInspector]
        protected string _id;

        public string id
        {
            get => this._id;
            set => this._id = value;
        }

        public IStateComponent CreateComponent()
        {
            return ReferencePool.Acquire<T>();
        }
    }

    /// <summary>
    /// 组件的抽象实现
    /// </summary>
    public abstract class StateComponent<T> : IStateComponent where T : class, IStateComponentAsset
    {
        protected IStateComponentAsset stateComponentAsset;
        protected StateMachine stateMachine;
        protected object owner;
        protected T asset;

        public void Init(IStateComponentAsset stateComponentAsset, StateMachine stateMachine)
        {
            this.stateComponentAsset = stateComponentAsset;
            this.stateMachine = stateMachine;
            this.owner = stateMachine.owner;
            asset = stateComponentAsset as T;

            OnInit();
        }

        protected virtual void OnInit() { }

        public virtual void Enter(StateMachine stateMachine) { }

        public virtual void Update(StateMachine stateMachine) { }

        public virtual void Exit(StateMachine stateMachine) { }

        public virtual void Dispose(StateMachine stateMachine) { }

        public virtual void Clear() { }
    }
}