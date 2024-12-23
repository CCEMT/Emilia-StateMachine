namespace Emilia.StateMachine
{
    public interface ILinkState : ILinkArg
    {
#if UNITY_EDITOR

        StateSelector stateSelector { get; }
#endif
    }
}