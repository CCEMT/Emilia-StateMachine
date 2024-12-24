using System;

namespace Emilia.StateMachine
{
    public interface IUniversalConditionAsset { }

    /// <summary>
    /// 通用条件资产
    /// </summary>
    [Serializable]
    public abstract class UniversalConditionAsset<T> : ConditionAsset<T>, IUniversalConditionAsset where T : class, ICondition, new() { }

    /// <summary>
    /// 通用条件
    /// </summary>
    public abstract class UniversalCondition<T> : Condition<T> where T : class, IConditionAsset { }
}