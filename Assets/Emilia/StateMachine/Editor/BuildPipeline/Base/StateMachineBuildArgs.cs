using System;
using Emilia.DataBuildPipeline.Editor;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineBuildArgs : BuildArgs
    {
        public EditorStateMachineAsset editorAsset;
        public string outputPath;
        
        public bool isGenerateFile;
        public bool isSaveAsset = true;
        public bool isRefresh = true;
        public bool updateRunner = true;
        public Action generateFileCallback;

        public StateMachineBuildArgs(EditorStateMachineAsset asset, string outputPath = null, Action<BuildReport> onBuildComplete = null)
        {
            this.outputPath = outputPath;
            this.editorAsset = asset;
            this.onBuildComplete = onBuildComplete;
            this.isGenerateFile = true;
            this.isSaveAsset = true;
            this.isRefresh = true;
        }
    }
}