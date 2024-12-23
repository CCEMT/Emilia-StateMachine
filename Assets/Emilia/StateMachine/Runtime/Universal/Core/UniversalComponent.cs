namespace Emilia.StateMachine
{
    public interface IUniversalComponentAsset { }

    /// <summary>
    /// 通用组件资产
    /// </summary>
    public abstract class UniversalComponentAsset<T> : StateComponentAsset<T>, IUniversalComponentAsset where T : class, IStateComponent, new() { }

    /// <summary>
    /// 通用组件
    /// </summary>
    public abstract class UniversalComponent<T> : StateComponent<T> where T : class, IStateComponentAsset { }
}