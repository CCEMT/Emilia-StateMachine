using System;
using Emilia.Node.Universal.Editor;
using Sirenix.Serialization;

namespace Emilia.StateMachine.Editor
{
    [NodeToEditor(typeof(StateMachineNodeAsset))]
    public abstract class EditorStateMachineAsset : EditorUniversalGraphAsset
    {
        [NonSerialized, OdinSerialize]
        public StateMachineAsset cache;

        public abstract string outputFilePath { get; }
        public abstract Type[] stateComponentSubTypes { get; }
        public abstract Type[] conditionSubTypes { get; }
    }
}