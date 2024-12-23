namespace Emilia.StateMachine
{
    /// <summary>
    /// 带有成功条件的组件资产
    /// </summary>
    public interface ISuccessComponentAsset { }

    /// <summary>
    /// 带有成功条件的组件
    /// </summary>
    public interface ISuccessComponent
    {
        bool IsSuccess(StateMachine stateMachine);
    }
}