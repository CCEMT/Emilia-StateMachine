namespace Emilia.StateMachine
{
    public interface IStateMachineRunner
    {
        int uid { get; }
        string fileName { get; }

        StateMachineAsset asset { get; }
        StateMachine stateMachine { get; }

        bool isActive { get; }

        void Init(string fileName, IStateMachineLoader loader, object owner = null);
        void Init(StateMachineAsset stateMachineAsset, object owner = null);

        void Start();
        void Update();
        void Dispose();
    }
}