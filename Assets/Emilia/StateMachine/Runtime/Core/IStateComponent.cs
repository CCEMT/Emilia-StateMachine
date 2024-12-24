using Emilia.Reference;

namespace Emilia.StateMachine
{
    /// <summary>
    /// 状态组件资产
    /// </summary>
    public interface IStateComponentAsset
    {
        /// <summary>
        /// 组件ID
        /// </summary>
        string id { get; set; }

        /// <summary>
        /// 创建指定组件
        /// </summary>
        IStateComponent CreateComponent();
    }

    /// <summary>
    /// 状态机组件(运行)
    /// </summary>
    public interface IStateComponent : IReference
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        void Init(IStateComponentAsset stateComponentAsset, StateMachine stateMachine);

        /// <summary>
        /// 状态进入函数
        /// </summary>
        /// <param name="stateMachine"></param>
        void Enter(StateMachine stateMachine);

        /// <summary>
        /// 更新函数（状态在进入后才会调用更新函数）
        /// </summary>
        void Update(StateMachine stateMachine);

        /// <summary>
        /// 状态退出函数
        /// </summary>
        /// <param name="stateMachine"></param>
        void Exit(StateMachine stateMachine);

        /// <summary>
        /// 销毁函数
        /// </summary>
        /// <param name="stateMachine"></param>
        void Dispose(StateMachine stateMachine);
    }
}