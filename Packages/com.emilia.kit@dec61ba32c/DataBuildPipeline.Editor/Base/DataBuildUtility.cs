﻿using UnityEngine;

namespace Emilia.DataBuildPipeline.Editor
{
    public static class DataBuildUtility
    {
        public static void Build(BuildArgs buildArgs)
        {
            IBuildPipeline defaultPipeline = BuildPipelineManage.instance.GetPipeline(buildArgs.GetType());
            defaultPipeline.Run(buildArgs);
        }
    }
}