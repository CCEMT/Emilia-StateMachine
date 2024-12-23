using Emilia.Node.Attributes;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    [NodeMenu("退出节点")]
    public class ExitNodeAsset : StateMachineNodeAsset
    {
        public const int StateMachineId = -2;
        public override int stateMachineId => StateMachineId;
        public override string title => "退出节点";
    }

    [EditorNode(typeof(ExitNodeAsset))]
    public class ExitNodeView : StateMachineNodeView
    {
        protected override EditorPortDirection portDirection => EditorPortDirection.Input;

        public override void Initialize(EditorGraphView graphView, EditorNodeAsset asset)
        {
            base.Initialize(graphView, asset);
            SetColor(Color.red);
        }
    }
}