using System;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Kit.Editor;
using UnityEditor;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(typeof(StateMachineBuildArgs)), BuildSequence(1000)]
    public class StateMachineOutputToEditor : IDataOutput
    {
        public void Output(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;
            StateMachineBuildArgs args = buildArgs as StateMachineBuildArgs;
            
            container.editorStateMachineAsset.cache = container.stateMachineAsset;
            container.editorStateMachineAsset.cacheEditorByRuntimeIdMap = container.editorByRuntimeMap;
            container.editorStateMachineAsset.cacheRuntimeByEditorIdMap = container.runtimeByEditorMap;
            container.buildReport.product = container.stateMachineAsset;

            container.editorStateMachineAsset.SetDirtyAll();
            if (args.isSaveAsset) AssetDatabase.SaveAssets();

            onFinished.Invoke();
        }
    }
}