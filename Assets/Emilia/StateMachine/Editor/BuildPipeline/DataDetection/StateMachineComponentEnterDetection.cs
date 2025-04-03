using Emilia.DataBuildPipeline.Editor;
using Emilia.StateMachine.Editor;

namespace Emilia.StateMachineComponent.Editor
{
    [BuildPipeline(typeof(StateMachineBuildArgs))]
    public class StateMachineComponentEnterDetection : IDataDetection
    {
        public bool Detection(IBuildContainer buildContainer, IBuildArgs buildArgs)
        {
            StateMachineBuildContainer stateMachineBuildContainer = buildContainer as StateMachineBuildContainer;

            int enterNodeAmount = stateMachineBuildContainer.allEnterNodeAssets.Count;
            if (enterNodeAmount == 0)
            {
                buildContainer.buildReport.AddErrorMessage("没有进入状态");
                return false;
            }

            return true;
        }
    }
}