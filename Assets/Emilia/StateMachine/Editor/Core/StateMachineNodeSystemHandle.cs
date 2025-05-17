using System;
using Emilia.Kit;
using Emilia.Node.Editor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    [EditorHandle(typeof(EditorStateMachineAsset))]
    public class StateMachineNodeSystemHandle : NodeSystemHandle
    {
        public override void OnCreateNode(EditorGraphView graphView, IEditorNodeView editorNodeView)
        {
            base.OnCreateNode(graphView, editorNodeView);
            SubStateMachineNodeAsset stateMachineNodeAsset = editorNodeView.asset as SubStateMachineNodeAsset;
            if (stateMachineNodeAsset == null) return;

            Type stateMachineAsset = graphView.graphAsset.GetType();
            stateMachineNodeAsset.editorStateMachineAsset = ScriptableObject.CreateInstance(stateMachineAsset) as EditorStateMachineAsset;
            stateMachineNodeAsset.editorStateMachineAsset.name = string.IsNullOrEmpty(stateMachineNodeAsset.displayName) ? stateMachineNodeAsset.title : stateMachineNodeAsset.displayName;

            graphView.graphAsset.AddChild(stateMachineNodeAsset.editorStateMachineAsset);
        }
    }
}