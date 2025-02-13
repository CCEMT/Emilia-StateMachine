using System;
using System.Collections.Generic;
using System.Linq;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Kit;
using Emilia.Kit.Editor;
using Emilia.Variables;
using UnityEditor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(StateMachineBuildPipeline.PipelineName), BuildSequence(3000)]
    public class StateMachineOutputToRunner : IDataOutput
    {
        public void Output(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;
            StateMachineBuildArgs args = buildArgs as StateMachineBuildArgs;

            if (args.updateRunner == false || EditorApplication.isPlaying == false)
            {
                onFinished.Invoke();
                return;
            }

            List<EditorStateMachineRunner> runners = EditorStateMachineRunner.runnerByAssetId.GetValueOrDefault(container.editorStateMachineAsset.id);
            if (runners == default)
            {
                onFinished.Invoke();
                return;
            }

            int runnersCount = runners.Count;
            for (int i = 0; i < runnersCount; i++)
            {
                EditorStateMachineRunner runner = runners[i];
                if (runner == null) continue;
                if (runner.isActive == false) continue;

                try
                {
                    ReflectUtility.SetValue(runner.stateMachine, "asset", container.stateMachineAsset);
                    UpdateStateMachine(runner.stateMachine);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToUnityLogString());
                }
            }

            onFinished.Invoke();
        }

        private static void UpdateStateMachine(StateMachine stateMachine)
        {
            foreach (var value in stateMachine.asset.variablesManage.variableMap)
            {
                bool isExist = stateMachine.userVariablesManage.variableMap.ContainsKey(value.Key);
                if (isExist) continue;
                Variable newValue = VariableUtility.Create(value.Value.type);
                stateMachine.userVariablesManage.SetThisValue(value.Key, newValue);
            }

            int assetAmount = stateMachine.asset.stateAssets.Count;
            for (int i = 0; i < assetAmount; i++)
            {
                StateAsset stateAsset = stateMachine.asset.stateAssets[i];
                if (stateAsset == default) continue;
                if (stateMachine.statesMap.ContainsKey(stateAsset.id)) continue;
                ReflectUtility.Invoke(stateMachine, "AddState", new object[] {stateAsset});
            }

            int enterId = EnterNodeAsset.StateMachineId;
            if (stateMachine.currentState != default)
            {
                enterId = stateMachine.currentState.id;
                stateMachine.currentState.Exit(stateMachine);
            }

            int stateAmount = stateMachine.statesList.Count;
            for (int i = stateAmount - 1; i >= 0; i--)
            {
                State state = stateMachine.statesList[i];
                if (state == default) continue;
                StateAsset stateAsset = stateMachine.asset.stateAssets.FirstOrDefault((asset) => asset.id == state.id);
                if (stateAsset != default) continue;

                List<State> statesList = stateMachine.statesList as List<State>;
                statesList.RemoveAt(i);

                Dictionary<int, State> statesMap = stateMachine.statesMap as Dictionary<int, State>;
                statesMap.Remove(state.id);
            }

            int amount = stateMachine.statesList.Count;
            for (int i = 0; i < amount; i++)
            {
                State item = stateMachine.statesList[i];
                if (item == default) continue;
                StateAsset newAsset = stateMachine.asset.stateAssets.FirstOrDefault((asset) => asset.id == item.id);
                if (newAsset == default) continue;
                item.Dispose(stateMachine);
                item.Init(newAsset, stateMachine);
            }

            State enterState = stateMachine.statesMap.GetValueOrDefault(enterId);
            if (enterState != default) enterState.Enter(stateMachine);
        }
    }
}