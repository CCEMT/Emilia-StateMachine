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

                string fullPath = $"{loader.editorFilePath}/{fileName}.asset";
                EditorStateMachineAsset loadAsset = AssetDatabase.LoadAssetAtPath<EditorStateMachineAsset>(fullPath);
                Init(loadAsset.cache, owner);
            }
            catch (Exception e)
            {
                Debug.LogError($"{owner} Init 时出现错误：\n{e.ToUnityLogString()}");
                _stateMachine.Dispose();
            }
        }

        public void Init(StateMachineAsset stateMachineAsset, object owner = null)
        {
            uid = StateMachineRunnerUtility.GetId();
            this.owner = owner;
            
            _stateMachine = new StateMachine();
            _stateMachine.Init(uid, stateMachineAsset, owner);

            if (_runnerByAssetId.ContainsKey(stateMachineAsset.id) == false) _runnerByAssetId.Add(stateMachineAsset.id, new List<EditorStateMachineRunner>());
            _runnerByAssetId[stateMachineAsset.id].Add(this);

            _runnerByUid[uid] = this;
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

                if (_runnerByAssetId.ContainsKey(asset.id)) _runnerByAssetId[asset.id].Remove(this);

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