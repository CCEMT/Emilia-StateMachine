using System;
using System.Collections;
using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Node.Editor;
using Sirenix.Serialization;
using Unity.EditorCoroutines.Editor;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(StateMachineBuildPipeline.PipelineName), BuildSequence(2000)]
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

            int amount = editorStateMachineAsset.nodes.Count;
            for (int i = 0; i < amount; i++)
            {
                EditorNodeAsset editorNodeAsset = editorStateMachineAsset.nodes[i];

                StateMachineNodeAsset stateMachineNodeAsset = editorNodeAsset as StateMachineNodeAsset;
                if (stateMachineNodeAsset == default) continue;

                BuildStatePack buildStatePack = new BuildStatePack();
                yield return ToStateAsset(stateMachineNodeAsset, buildStatePack);
                if (buildStatePack.stateAsset == default) continue;

                stateAssets.Add(buildStatePack.stateAsset);
            }

            container.stateAssets = stateAssets;

            onFinished?.Invoke();
        }

        private IEnumerator ToStateAsset(StateMachineNodeAsset stateMachineNodeAsset, BuildStatePack pack)
        {
            switch (stateMachineNodeAsset)
            {
                case SubStateMachineNodeAsset subStateMachineNodeAsset:
                    yield return ToSubStateMachineAsset(subStateMachineNodeAsset, pack);
                    break;
                case StateNodeAsset stateNodeAsset:
                    pack.stateAsset = ToStateAsset(stateNodeAsset);
                    break;
                case EnterNodeAsset enterNodeAsset:
                    pack.stateAsset = ToEnterState(enterNodeAsset);
                    break;
                case ExitNodeAsset exitNodeAsset:
                    pack.stateAsset = ToExitState(exitNodeAsset);
                    break;
            }
        }

        private IEnumerator ToSubStateMachineAsset(SubStateMachineNodeAsset subStateMachineNodeAsset, BuildStatePack pack)
        {
            StateAsset stateAsset = ToStateAsset(subStateMachineNodeAsset);

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

        private StateAsset ToStateAsset(StateNodeAsset stateNodeAsset)
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

            StateAsset stateAsset = new StateAsset(stateNodeAsset.stateMachineId, new List<TransitionAsset>(), stateComponentAssets);
            return stateAsset;
        }

        private StateAsset ToEnterState(EnterNodeAsset enterNodeAsset)
        {
            StateAsset stateAsset = new StateAsset(enterNodeAsset.stateMachineId, new List<TransitionAsset>(), new List<IStateComponentAsset>());
            return stateAsset;
        }

        private StateAsset ToExitState(ExitNodeAsset exitNodeAsset)
        {
            ExitComponentAsset exitComponentAsset = new ExitComponentAsset();
            StateAsset stateAsset = new StateAsset(exitNodeAsset.stateMachineId, new List<TransitionAsset>(), new List<IStateComponentAsset> {exitComponentAsset});
            return stateAsset;
        }
    }
}