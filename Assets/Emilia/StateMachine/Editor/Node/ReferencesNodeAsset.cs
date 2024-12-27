using System;
using System.Linq;
using Emilia.Kit;
using Emilia.Node.Attributes;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    [NodeMenu("引用节点")]
    public class ReferencesNodeAsset : StateMachineNodeAsset
    {
        [NonSerialized, OdinSerialize, HideInInspector]
        private StateMachineNodeAsset _referenceNodeAsset;

        [OnInspectorGUI, PropertyOrder(-1)]
        void SelectStateNode()
        {
            GUILayout.BeginVertical("frameBox");

            GUILayout.Label("引用的节点：");

            string labelString = "";
            if (_referenceNodeAsset != null) labelString = GetLabelString(_referenceNodeAsset);
            if (GUILayout.Button(labelString))
            {
                OdinMenu odinMenu = new OdinMenu("选择节点");

                EditorGraphView graphView = EditorGraphView.focusedGraphView;

                StateMachineNodeAsset[] stateMachineNodeAssets = graphView.graphAsset.nodes.OfType<StateMachineNodeAsset>().ToArray();
                int amount = stateMachineNodeAssets.Length;
                for (int i = 0; i < amount; i++)
                {
                    StateMachineNodeAsset node = stateMachineNodeAssets[i];
                    if (node == null) continue;

                    if (node.stateMachineId <= 0) continue;
                    if (node is ReferencesNodeAsset) continue;

                    odinMenu.AddItem(GetLabelString(node), () => _referenceNodeAsset = node);
                }

                odinMenu.ShowInPopup();
            }

            GUILayout.EndVertical();

            string GetLabelString(StateMachineNodeAsset node)
            {
                string label = string.IsNullOrEmpty(node.displayName) ? node.title : node.displayName;
                return $"{label}({node.stateMachineId})";
            }
        }

        [ShowInInspector, HideLabel, HideReferenceObjectPicker, InlineEditor(InlineEditorObjectFieldModes.Boxed, Expanded = true)]
        public StateMachineNodeAsset referenceNodeAsset => this._referenceNodeAsset;

        public override int stateMachineId => this._referenceNodeAsset?.stateMachineId ?? 0;
        public override string title => "引用节点";
    }

    [EditorNode(typeof(ReferencesNodeAsset))]
    public class ReferencesNodeView : StateMachineNodeView
    {
        protected override EditorPortDirection portDirection => EditorPortDirection.Any;

        public override void Initialize(EditorGraphView graphView, EditorNodeAsset asset)
        {
            base.Initialize(graphView, asset);
            SetColor(Color.yellow);
        }
    }
}