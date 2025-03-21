using System;
using System.Collections.Generic;
using Emilia.Reference;
using UnityEngine;

namespace Emilia.StateMachine
{
    /// <summary>
    /// 过渡资产
    /// </summary>
    [Serializable]
    public class TransitionAsset
    {
        [SerializeField]
        private int _nextId;

        [SerializeField]
        private List<IConditionAsset> _conditionAssets;

        /// <summary>
        /// 下个状态ID
        /// </summary>
        public int nextId => this._nextId;

        /// <summary>
        /// 条件资产
        /// </summary>
        public IReadOnlyList<IConditionAsset> conditionAssets => _conditionAssets;

        public TransitionAsset(int nextId, List<IConditionAsset> conditionAssets)
        {
            this._nextId = nextId;
            this._conditionAssets = conditionAssets;
        }
    }

    public class Transition : IReference
    {
        protected TransitionAsset _asset;
        protected List<ICondition> _conditions = new List<ICondition>();

        /// <summary>
        /// 下一个状态ID
        /// </summary>
        public int nextId => _asset.nextId;

        /// <summary>
        /// 条件列表
        /// </summary>
        public IReadOnlyList<ICondition> conditions => _conditions;

        /// <summary>
        /// 初始化函数
        /// </summary>
        public void Init(TransitionAsset transitionAsset)
        {
            this._asset = transitionAsset;

            int amount = this._asset.conditionAssets.Count;
            for (int i = 0; i < amount; i++)
            {
                IConditionAsset conditionAsset = this._asset.conditionAssets[i];
                ICondition condition = conditionAsset.CreateCondition();
                condition.Init(conditionAsset);
                this._conditions.Add(condition);
            }
        }

        /// <summary>
        /// Transform是否成功
        /// </summary>
        public bool IsSuccess(StateMachine stateMachine)
        {
            int amount = this.conditions.Count;
            for (int i = 0; i < amount; i++)
            {
                ICondition condition = conditions[i];
                if (condition.IsSuccess(stateMachine) == false) return false;
            }
            return true;
        }

        public void Dispose()
        {
            int amount = this._conditions.Count;
            for (int i = 0; i < amount; i++)
            {
                ICondition condition = _conditions[i];
                condition.Dispose();
            }

            ReferencePool.Release(this);
        }

        void IReference.Clear()
        {
            _asset = null;
            _conditions.Clear();
        }
    }
}