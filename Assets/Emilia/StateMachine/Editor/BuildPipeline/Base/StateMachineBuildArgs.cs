﻿using System;
using Emilia.DataBuildPipeline.Editor;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineBuildArgs : BuildArgs
    {
        public EditorStateMachineAsset editorAsset;
        public string outputPath;
        
        public bool isGenerateFile;
        public bool updateRunner = true;

        public StateMachineBuildArgs(EditorStateMachineAsset asset, string outputPath = null, Action<BuildReport> onBuildComplete = null)
        {
            this.outputPath = outputPath;
            this.editorAsset = asset;
            this.onBuildComplete = onBuildComplete;
            isGenerateFile = true;
        }
    }
}