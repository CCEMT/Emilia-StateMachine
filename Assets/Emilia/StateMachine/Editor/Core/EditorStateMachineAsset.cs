using System;
using System.Collections.Generic;
using Emilia.Node.Universal.Editor;
using Sirenix.Serialization;

namespace Emilia.StateMachine.Editor
{
    [NodeToEditor(typeof(StateMachineNodeAsset))]
    public abstract class EditorStateMachineAsset : EditorUniversalGraphAsset
    {
        [NonSerialized, OdinSerialize]
        public StateMachineAsset cache;

        [NonSerialized, OdinSerialize]
        public Dictionary<int, string> cacheEditorByRuntimeIdMap = new Dictionary<int, string>();

        [NonSerialized, OdinSerialize]
        public Dictionary<string, int> cacheRuntimeByEditorIdMap = new Dictionary<string, int>();
        
        public abstract string outputFilePath { get; }
        public abstract Type[] stateComponentSubTypes { get; }
        public abstract Type[] conditionSubTypes { get; }
    }
}