using System.Collections.Generic;
using System.Linq;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Node.Editor;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(PipelineName)]
    public class StateMachineBuildPipeline : UniversalBuildPipeline
    {
        public const string PipelineName = "StateMachine";

        private StateMachineBuildArgs stateMachineBuildArgs;

        protected override void RunInitialize()
        {
            base.RunInitialize();
            this.stateMachineBuildArgs = this.buildArgs as StateMachineBuildArgs;
        }

        protected override IBuildContainer CreateContainer()
        {
            StateMachineBuildContainer stateMachineBuildContainer = new StateMachineBuildContainer();
            stateMachineBuildContainer.editorStateMachineAsset = this.stateMachineBuildArgs.editorAsset;

            stateMachineBuildContainer.allEnterNodeAssets = new List<EnterNodeAsset>();
            stateMachineBuildContainer.allEnterNodeAssets.AddRange(stateMachineBuildContainer.editorStateMachineAsset.nodes.OfType<EnterNodeAsset>());

            stateMachineBuildContainer.allSubStateMachineNodeAssets = new List<SubStateMachineNodeAsset>();
            GetSubStateMachineNodeAsset(stateMachineBuildContainer.editorStateMachineAsset, stateMachineBuildContainer.allSubStateMachineNodeAssets);

            return stateMachineBuildContainer;
        }

        private void GetSubStateMachineNodeAsset(EditorStateMachineAsset editorStateMachineAsset, List<SubStateMachineNodeAsset> list)
        {
            int amount = editorStateMachineAsset.nodes.Count;
            for (int i = 0; i < amount; i++)
            {
                EditorNodeAsset editorNodeAsset = editorStateMachineAsset.nodes[i];
                SubStateMachineNodeAsset subStateMachineNodeAsset = editorNodeAsset as SubStateMachineNodeAsset;
                if (subStateMachineNodeAsset == default) continue;
                list.Add(subStateMachineNodeAsset);
                GetSubStateMachineNodeAsset(subStateMachineNodeAsset.editorStateMachineAsset, list);
            }
        }
    }
}