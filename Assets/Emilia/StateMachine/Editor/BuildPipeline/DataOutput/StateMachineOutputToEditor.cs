using System;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Kit.Editor;
using Emilia.Node.Editor;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(typeof(StateMachineBuildArgs)), BuildSequence(1000)]
    public class StateMachineOutputToEditor : IDataOutput
    {
        public void Output(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;
            container.editorStateMachineAsset.cache = container.stateMachineAsset;
            container.editorStateMachineAsset.cacheEditorByRuntimeIdMap = container.editorByRuntimeMap;
            container.editorStateMachineAsset.cacheRuntimeByEditorIdMap = container.runtimeByEditorMap;
            container.buildReport.product = container.stateMachineAsset;

            container.editorStateMachineAsset.SaveAll();

            onFinished.Invoke();
        }
    }
}