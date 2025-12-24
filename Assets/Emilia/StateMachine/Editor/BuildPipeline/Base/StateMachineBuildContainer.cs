using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Variables;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineBuildContainer : BuildContainer
    {
        public EditorStateMachineAsset editorStateMachineAsset { get; set; }
        public List<EnterNodeAsset> allEnterNodeAssets { get; set; }
        public List<SubStateMachineNodeAsset> allSubStateMachineNodeAssets { get; set; }
        
        public Dictionary<int, string> editorByRuntimeMap { get; set; } = new Dictionary<int, string>();
        public Dictionary<string, int> runtimeByEditorMap { get; set; } = new Dictionary<string, int>();

        public VariablesManager variablesManage { get; set; }
        public List<StateAsset> stateAssets { get; set; }

        public StateMachineAsset stateMachineAsset { get; set; }
    }
}