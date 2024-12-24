namespace Emilia.StateMachine
{
    /// <summary>
    /// 描述接口，当IStateComponentAsset继承了IDescription接口时，可以在编辑器中显示描述信息
    /// </summary>
    public interface IDescription
    {
#if UNITY_EDITOR
        /// <summary>
        /// 描述信息
        /// </summary>
        string text { get; }
#endif
    }
}