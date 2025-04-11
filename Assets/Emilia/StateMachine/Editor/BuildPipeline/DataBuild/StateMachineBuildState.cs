using System;
using System.Collections;
using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Node.Editor;
using Sirenix.Serialization;
using Unity.EditorCoroutines.Editor;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(typeof(StateMachineBuildArgs)), BuildSequence(2000)]
    public class StateMachineBuildState : IDataBuild
    {
        class BuildStatePack
        {
            public StateAsset stateAsset;
        }

        public void Build(IBuildContainer buildContainer, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;
            EditorCoroutineUtility.StartCoroutineOwnerless(OnBuild(container, onFinished));
        }

        private IEnumerator OnBuild(StateMachineBuildContainer container, Action onFinished)
        {
            EditorStateMachineAsset editorStateMachineAsset = container.editorStateMachineAsset;

            List<StateAsset> stateAssets = new List<StateAsset>();

            int id = 0;

            int amount = editorStateMachineAsset.nodes.Count;
            for (int i = 0; i < amount; i++)
            {
                EditorNodeAsset editorNodeAsset = editorStateMachineAsset.nodes[i];

                StateMachineNodeAsset stateMachineNodeAsset = editorNodeAsset as StateMachineNodeAsset;
                if (stateMachineNodeAsset == default) continue;

                id++;

                BuildStatePack buildStatePack = new BuildStatePack();
                yield return ToStateAsset(id, stateMachineNodeAsset, buildStatePack);
                if (buildStatePack.stateAsset == default) continue;

                stateAssets.Add(buildStatePack.stateAsset);
                
                container.editorByRuntimeMap[buildStatePack.stateAsset.id] = editorNodeAsset.id;
                container.runtimeByEditorMap[editorNodeAsset.id] = buildStatePack.stateAsset.id;
            }

            container.stateAssets = stateAssets;
        

            onFinished?.Invoke();
        }

        private IEnumerator ToStateAsset(int id, StateMachineNodeAsset stateMachineNodeAsset, BuildStatePack pack)
        {
            switch (stateMachineNodeAsset)
            {
                case SubStateMachineNodeAsset subStateMachineNodeAsset:
                    yield return ToSubStateMachineAsset(id, subStateMachineNodeAsset, pack);
                    break;
                case StateNodeAsset stateNodeAsset:
                    pack.stateAsset = ToStateAsset(id, stateNodeAsset);
                    break;
                case EnterNodeAsset enterNodeAsset:
                    pack.stateAsset = ToEnterState(id, enterNodeAsset);
                    break;
                case ExitNodeAsset exitNodeAsset:
                    pack.stateAsset = ToExitState(id, exitNodeAsset);
                    break;
            }
        }

        private IEnumerator ToSubStateMachineAsset(int id, SubStateMachineNodeAsset subStateMachineNodeAsset, BuildStatePack pack)
        {
            StateAsset stateAsset = ToStateAsset(id, subStateMachineNodeAsset);

            BuildReport buildReport = null;
            StateMachineBuildArgs args = new StateMachineBuildArgs(subStateMachineNodeAsset.editorStateMachineAsset, onBuildComplete: (x) => { buildReport = x; });
            args.updateRunner = false;

            DataBuildUtility.Build(args);

            while (buildReport == null) yield return 0;

            if (buildReport.result == BuildResult.Failed) yield break;

            StateMachineAsset stateMachineAsset = buildReport.product as StateMachineAsset;
            if (stateMachineAsset == default) yield break;

            SubStateMachineComponentAsset subStateMachineComponentAsset = new SubStateMachineComponentAsset();
            subStateMachineComponentAsset.stateMachineAsset = stateMachineAsset;

            List<IStateComponentAsset> componentAsset = stateAsset.componentAssets as List<IStateComponentAsset>;
            componentAsset.Add(subStateMachineComponentAsset);

            pack.stateAsset = stateAsset;
        }

        private StateAsset ToStateAsset(int id, StateNodeAsset stateNodeAsset)
        {
            List<IStateComponentAsset> stateComponentAssets = new List<IStateComponentAsset>();

            int amount = stateNodeAsset.componentGroup.componentAssets.Count;
            for (int i = 0; i < amount; i++)
            {
                EditorStateComponentAsset editorStateComponentAsset = stateNodeAsset.componentGroup.componentAssets[i];
                if (editorStateComponentAsset == default) continue;

                IStateComponentAsset stateComponentAsset = editorStateComponentAsset.componentAsset;
                if (stateComponentAsset == default) continue;

                IStateComponentAsset copyStateComponentAsset = SerializationUtility.CreateCopy(stateComponentAsset) as IStateComponentAsset;
                stateComponentAssets.Add(copyStateComponentAsset);
            }

            StateAsset stateAsset = new StateAsset(id, stateNodeAsset.stateMachineId, new List<TransitionAsset>(), stateComponentAssets);
            return stateAsset;
        }

        private StateAsset ToEnterState(int id, EnterNodeAsset enterNodeAsset)
        {
            StateAsset stateAsset = new StateAsset(id, enterNodeAsset.stateMachineId, new List<TransitionAsset>(), new List<IStateComponentAsset>());
            return stateAsset;
        }

        private StateAsset ToExitState(int id, ExitNodeAsset exitNodeAsset)
        {
            ExitComponentAsset exitComponentAsset = new ExitComponentAsset();
            StateAsset stateAsset = new StateAsset(id, exitNodeAsset.stateMachineId, new List<TransitionAsset>(), new List<IStateComponentAsset> {exitComponentAsset});
            return stateAsset;
        }
    }
}