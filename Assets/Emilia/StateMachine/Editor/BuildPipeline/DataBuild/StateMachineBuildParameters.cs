﻿using System;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Variables;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(StateMachineBuildPipeline.PipelineName), BuildSequence(1000)]
    public class StateMachineBuildParameters : IDataBuild
    {
        public void Build(IBuildContainer buildContainer, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;
            if (container.editorStateMachineAsset.editorParametersManage == null) container.variablesManage = new VariablesManage();
            else
            {
                VariablesManage rootVariablesManage = container.editorStateMachineAsset.editorParametersManage.ToParametersManage();
                container.variablesManage = rootVariablesManage;
            }

            onFinished.Invoke();
        }
    }
}