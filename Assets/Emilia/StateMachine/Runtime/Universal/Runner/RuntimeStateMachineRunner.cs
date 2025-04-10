using Emilia.Reference;
using UnityEngine;

namespace Emilia.StateMachine
{
    public class RuntimeStateMachineRunner : IStateMachineRunner, IReference
    {
        private IStateMachineLoader stateMachineLoader;
        private StateMachine _stateMachine;

        public int uid { get; private set; }
        public StateMachineAsset asset => _stateMachine.asset;
        public StateMachine stateMachine => _stateMachine;

        public bool isActive
        {
            get
            {
                if (this._stateMachine == null) return false;
                return stateMachine.runState == StateMachine.RunState.Running;
            }
        }

        public void Start()
        {
            if (stateMachine == default) return;
            stateMachine.Start();
        }

        public void Init(string fileName, IStateMachineLoader loader, object owner)
        {
            uid = StateMachineRunnerUtility.GetId();

            stateMachineLoader = loader;

            string fullPath = $"{this.stateMachineLoader.runtimeFilePath}/{fileName}.bytes";
            TextAsset textAsset = this.stateMachineLoader.LoadAsset(fullPath) as TextAsset;
            StateMachineAsset stateMachineAsset = this.stateMachineLoader.LoadStateMachineAsset(textAsset.bytes);
            stateMachineLoader.ReleaseAsset(fullPath);

            _stateMachine = ReferencePool.Acquire<StateMachine>();
            _stateMachine.Init(uid, stateMachineAsset, owner);
        }

        public void Update()
        {
            if (_stateMachine == default) return;
            _stateMachine.Update();
        }

        public void Dispose()
        {
            _stateMachine?.Dispose();
            ReferencePool.Release(this);
        }

        void IReference.Clear()
        {
            StateMachineRunnerUtility.RecycleId(uid);
            uid = -1;

            _stateMachine = null;

        }
    }
}