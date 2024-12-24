using System;
using Emilia.Reference;

namespace Emilia.StateMachine
{
    /// <summary>
    /// 条件资产的抽象实现
    /// </summary>
    [Serializable]
    public abstract class ConditionAsset<T> : IConditionAsset where T : class, ICondition, new()
    {
        public ICondition CreateCondition()
        {
            return ReferencePool.Acquire<T>();
        }
    }

    /// <summary>
    /// 条件的抽象实现
    /// </summary>
    public abstract class Condition<T> : ICondition where T : class, IConditionAsset
    {
        protected T asset;

        public void Init(IConditionAsset conditionAsset)
        {
            this.asset = conditionAsset as T;
            OnInit();
        }

        protected virtual void OnInit() { }
        public abstract bool IsSuccess(StateMachine stateMachine);
        public virtual void Clear() { }
    }
}