using System.Linq;
using Emilia.Kit;
using Emilia.Node.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    public class StateSelectorDrawer : OdinValueDrawer<StateSelector>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            StateSelector stateSelector = ValueEntry.SmartValue;

            EditorGraphView editorGraphView = EditorGraphView.focusedGraphView;

            EditorStateMachineAsset stateMachineAsset = editorGraphView.graphAsset as EditorStateMachineAsset;
            if (stateMachineAsset == default) return;

            string labelString = GetLabel(stateSelector.stateId);
            if (GUILayout.Button(labelString, "MiniButton"))
            {
                StateMachineNodeAsset[] stateMachineNodeAssets = stateMachineAsset.nodes.OfType<StateMachineNodeAsset>().ToArray();

                OdinMenu odinMenu = new OdinMenu("选择状态");

                int amount = stateMachineNodeAssets.Length;
                for (int i = 0; i < amount; i++)
                {
                    StateMachineNodeAsset stateMachineNodeAsset = stateMachineNodeAssets[i];
                    if (stateMachineNodeAsset.stateMachineId <= 0) continue;

                    string itemName = $"{GetLabel(stateMachineNodeAsset.stateMachineId)}({stateMachineNodeAsset.stateMachineId})";
                    odinMenu.AddItem(itemName, () => stateSelector.stateId = stateMachineNodeAsset.stateMachineId);
                }

                odinMenu.ShowInPopup();
            }

            string GetLabel(int stateId)
            {
                string name = "";
                int amount = stateMachineAsset.nodes.Count;
                for (int i = 0; i < amount; i++)
                {
                    EditorNodeAsset nodeAsset = stateMachineAsset.nodes[i];

                    StateMachineNodeAsset stateMachineNodeAsset = nodeAsset as StateMachineNodeAsset;
                    if (stateMachineNodeAsset == default) continue;

                    if (stateMachineNodeAsset.stateMachineId != stateId) continue;

                    name = string.IsNullOrEmpty(stateMachineNodeAsset.displayName) ? stateMachineNodeAsset.title : stateMachineNodeAsset.displayName;
                }

                return string.IsNullOrEmpty(name) ? stateId.ToString() : $"{stateId}({name})";
            }
        }
    }
}