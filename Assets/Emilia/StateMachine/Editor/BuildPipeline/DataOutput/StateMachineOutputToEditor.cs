using System;
using Emilia.DataBuildPipeline.Editor;
using UnityEditor;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(StateMachineBuildPipeline.PipelineName), BuildSequence(1000)]
    public class StateMachineOutputToEditor : IDataOutput
    {
        public void Output(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;
            container.editorStateMachineAsset.cache = container.stateMachineAsset;
            container.buildReport.product = container.stateMachineAsset;

            AssetDatabase.SaveAssetIfDirty(container.editorStateMachineAsset);
            onFinished.Invoke();
        }
    }
}