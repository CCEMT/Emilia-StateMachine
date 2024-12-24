using Emilia.Reference;

namespace Emilia.StateMachine
{
    /// <summary>
    /// 条件资产
    /// </summary>
    public interface IConditionAsset
    {
        /// <summary>
        /// 创建指定条件
        /// </summary>
        ICondition CreateCondition();
    }

    /// <summary>
    /// 条件（运行）
    /// </summary>
    public interface ICondition : IReference
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        void Init(IConditionAsset conditionAsset);

        /// <summary>
        /// 是否满足条件
        /// </summary>
        bool IsSuccess(StateMachine stateMachine);
    }
}