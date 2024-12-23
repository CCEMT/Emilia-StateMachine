using Emilia.Node.Attributes;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    [NodeMenu("任意节点")]
    public class AnyNodeAsset : StateMachineNodeAsset
    {
        public const int StateMachineId = -3;
        public override int stateMachineId => StateMachineId;
        public override string title => "任意节点";
    }

    [EditorNode(typeof(AnyNodeAsset))]
    public class AnyNodeView : StateMachineNodeView
    {
        protected override EditorPortDirection portDirection => EditorPortDirection.Output;

        public override void Initialize(EditorGraphView graphView, EditorNodeAsset asset)
        {
            base.Initialize(graphView, asset);
            SetColor(Color.cyan);
        }
    }
}