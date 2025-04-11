using System;
using System.Collections.Generic;
using Emilia.Kit;
using UnityEditor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    public class EditorStateMachineRunner : IStateMachineRunner
    {
        private static Dictionary<string, List<EditorStateMachineRunner>> _runnerByAssetId = new Dictionary<string, List<EditorStateMachineRunner>>();
        private static Dictionary<int, EditorStateMachineRunner> _runnerByUid = new Dictionary<int, EditorStateMachineRunner>();
        public static IReadOnlyDictionary<string, List<EditorStateMachineRunner>> runnerByAssetId => _runnerByAssetId;
        public static IReadOnlyDictionary<int, EditorStateMachineRunner> runnerByUid => _runnerByUid;

        private object owner;
        private IStateMachineLoader stateMachineLoader;
        private EditorStateMachineAsset _editorStateMachineAsset;

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

        public EditorStateMachineAsset editorStateMachineAsset => _editorStateMachineAsset;

        public void Start()
        {
            if (stateMachine == default) return;

            try
            {
                stateMachine.Start();
            }
            catch (Exception e)
            {
                Debug.LogError($"{owner} Start 时出现错误：\n{e.ToUnityLogString()}");
                _stateMachine.Dispose();
            }
        }

        public void Init(string fileName, IStateMachineLoader loader, object owner)
        {
            try
            {
                this.fileName = fileName;
                this.owner = owner;
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
            catch (Exception e)
            {
                Debug.LogError($"{owner} Init 时出现错误：\n{e.ToUnityLogString()}");
                _stateMachine.Dispose();
            }
        }

        public void Update()
        {
            if (_stateMachine == default) return;
            try
            {
                _stateMachine.Update();
            }
            catch (Exception e)
            {
                Debug.LogError($"{owner} Update 时出现错误：\n{e.ToUnityLogString()}");
                _stateMachine.Dispose();
            }
        }

        public void Dispose()
        {
            try
            {
                if (_runnerByUid.ContainsKey(uid)) _runnerByUid.Remove(uid);

                fileName = null;

                if (uid != -1) StateMachineRunnerUtility.RecycleId(uid);
                uid = -1;

                if (_stateMachine == default) return;

                if (_runnerByAssetId.ContainsKey(this._editorStateMachineAsset.id)) _runnerByAssetId[this._editorStateMachineAsset.id].Remove(this);

                if (isActive) _stateMachine.Dispose();
                _stateMachine = default;

                this.owner = null;
            }
            catch (Exception e)
            {
                Debug.LogError($"{owner} Dispose 时出现错误：\n{e.ToUnityLogString()}");
            }
        }
    }
}