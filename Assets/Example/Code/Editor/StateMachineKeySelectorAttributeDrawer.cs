﻿using Emilia.Kit;
using Emilia.StateMachine.Editor;
using Emilia.Variables.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using UnityEditor;
using UnityEngine;

namespace Emilia.SpineAnimator.Editor
{
    public class StateMachineKeySelectorAttributeDrawer : OdinAttributeDrawer<StateMachineKeySelectorAttribute>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (typeof(string).IsAssignableFrom(this.Property.ValueEntry.TypeOfValue) == false) return;
            string stringValue = this.Property.ValueEntry.WeakSmartValue as string;

            ValueResolver<string> keyValueResolver = ValueResolver.Get<string>(Property, Attribute.filePath);
            if (keyValueResolver.HasError) return;
            string path = keyValueResolver.GetValue();
            EditorStateMachineAsset behaviorTreeAsset = AssetDatabase.LoadAssetAtPath<EditorStateMachineAsset>(path);
            if (behaviorTreeAsset == null) return;

            GUILayout.BeginHorizontal();

            if (label != null) GUILayout.Label(label);

            string buttonLabel = GetDescription(behaviorTreeAsset, stringValue);
            if (GUILayout.Button(buttonLabel, EditorStyles.popup))
            {
                OdinMenu odinMenu = new OdinMenu();

                int count = behaviorTreeAsset.editorParametersManage.parameters.Count;
                for (var i = 0; i < count; i++)
                {
                    EditorParameter parameter = behaviorTreeAsset.editorParametersManage.parameters[i];
                    string itemName = parameter.description;
                    odinMenu.AddItem(itemName, () => Property.ValueEntry.WeakSmartValue = parameter.key);
                }

                odinMenu.ShowInPopup();
            }

            GUILayout.EndHorizontal();
        }

        private string GetDescription(EditorStateMachineAsset behaviorTreeAsset, string key)
        {
            EditorParameter editorParameter = behaviorTreeAsset.editorParametersManage.parameters.Find(p => p.key == key);
            if (editorParameter == null) return key;
            return editorParameter.description;
        }
    }
}