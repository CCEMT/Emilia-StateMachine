using Emilia.Reference;
using UnityEngine;

namespace Emilia.StateMachine
{
    public class RuntimeStateMachineRunner : IStateMachineRunner, IReference
    {
        private StateMachine _stateMachine;

        public int uid { get; private set; }
        public string fileName { get; private set; }
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
            this.fileName = fileName;
            uid = StateMachineRunnerUtility.GetId();

            string fullPath = $"{loader.runtimeFilePath}/{fileName}.bytes";
            TextAsset textAsset = loader.LoadAsset(fullPath) as TextAsset;
            StateMachineAsset stateMachineAsset = loader.LoadStateMachineAsset(textAsset.bytes);
            loader.ReleaseAsset(fullPath);

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
            fileName = null;
            
            if (uid != -1) StateMachineRunnerUtility.RecycleId(uid);
            uid = -1;

            _stateMachine = null;

        }
    }
}