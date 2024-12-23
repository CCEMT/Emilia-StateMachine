using Emilia.Node.Attributes;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    [NodeMenu("进入节点")]
    public class EnterNodeAsset : StateMachineNodeAsset
    {
        public const int StateMachineId = -1;
        public override int stateMachineId => StateMachineId;
        public override string title => "进入节点";
    }

    [EditorNode(typeof(EnterNodeAsset))]
    public class EnterNodeView : StateMachineNodeView
    {
        protected override EditorPortDirection portDirection => EditorPortDirection.Output;

        public override void Initialize(EditorGraphView graphView, EditorNodeAsset asset)
        {
            base.Initialize(graphView, asset);
            SetColor(Color.green);
        }
    }
}