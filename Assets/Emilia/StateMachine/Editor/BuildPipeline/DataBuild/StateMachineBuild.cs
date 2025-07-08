using System;
using Emilia.DataBuildPipeline.Editor;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(typeof(StateMachineBuildArgs)), BuildSequence(4000)]
    public class StateMachineBuild : IDataBuild
    {
        public void Build(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;

            string id = container.editorStateMachineAsset.id;
            string description = container.editorStateMachineAsset.description;
            StateMachineAsset stateMachineAsset = new StateMachineAsset(id, description, container.variablesManage, EnterNodeAsset.StateMachineId, container.stateAssets, null);
            container.stateMachineAsset = stateMachineAsset;

            onFinished.Invoke();
        }
    }
}