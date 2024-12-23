using System.Collections.Generic;
using UnityEditor;

namespace Emilia.StateMachine.Editor
{
    public class EditorStateMachineRunner : IStateMachineRunner
    {
        private static Dictionary<string, List<EditorStateMachineRunner>> _runnerByAssetId = new Dictionary<string, List<EditorStateMachineRunner>>();
        private static Dictionary<int, EditorStateMachineRunner> _runnerByUid = new Dictionary<int, EditorStateMachineRunner>();
        public static IReadOnlyDictionary<string, List<EditorStateMachineRunner>> runnerByAssetId => _runnerByAssetId;
        public static IReadOnlyDictionary<int, EditorStateMachineRunner> runnerByUid => _runnerByUid;

        private IStateMachineLoader stateMachineLoader;
        private EditorStateMachineAsset _editorStateMachineAsset;

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

        public EditorStateMachineAsset editorStateMachineAsset => _editorStateMachineAsset;

        public void Start()
        {
            if (stateMachine == default) return;
            stateMachine.Start();
        }

        public void Init(string fileName, IStateMachineLoader loader, object owner)
        {
            uid = StateMachineRunnerUtility.GetId();
            stateMachineLoader = loader;

            string fullPath = $"{loader.editorFilePath}/{fileName}.asset";
            EditorStateMachineAsset loadAsset = AssetDatabase.LoadAssetAtPath<EditorStateMachineAsset>(fullPath);
            this._editorStateMachineAsset = loadAsset;

            _stateMachine = new StateMachine();
            _stateMachine.Init(uid, loadAsset.cache, owner);

            if (_runnerByAssetId.ContainsKey(loadAsset.id) == false) _runnerByAssetId.Add(loadAsset.id, new List<EditorStateMachineRunner>());
            _runnerByAssetId[loadAsset.id].Add(this);

            _runnerByUid[uid] = this;
        }

        public void Update()
        {
            if (_stateMachine == default) return;
            _stateMachine.Update();
        }

        public void Dispose()
        {
            if (_runnerByUid.ContainsKey(uid)) _runnerByUid.Remove(uid);

            StateMachineRunnerUtility.RecycleId(uid);
            uid = -1;

            if (_stateMachine == default) return;

            if (_runnerByAssetId.ContainsKey(this._editorStateMachineAsset.id)) _runnerByAssetId[this._editorStateMachineAsset.id].Remove(this);

            _stateMachine.Dispose();
            _stateMachine = default;
        }
    }
}