using System;
using Emilia.Kit;
using Emilia.Node.Editor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineNodeSystemHandle : NodeSystemHandle<EditorStateMachineAsset>
    {
        public override void OnCreateNode(IEditorNodeView editorNodeView)
        {
            SubStateMachineNodeAsset stateMachineNodeAsset = editorNodeView.asset as SubStateMachineNodeAsset;
            if (stateMachineNodeAsset == null) return;

            Type stateMachineAsset = smartValue.graphAsset.GetType();
            stateMachineNodeAsset.editorStateMachineAsset = ScriptableObject.CreateInstance(stateMachineAsset) as EditorStateMachineAsset;
            stateMachineNodeAsset.editorStateMachineAsset.name = string.IsNullOrEmpty(stateMachineNodeAsset.displayName) ? stateMachineNodeAsset.title : stateMachineNodeAsset.displayName;
            stateMachineNodeAsset.editorStateMachineAsset.parent = smartValue.graphAsset;

            EditorAssetKit.SaveAssetIntoObject(stateMachineNodeAsset.editorStateMachineAsset, smartValue.graphAsset);
        }
    }
}