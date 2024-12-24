using System;
using System.Collections.Generic;
using Emilia.Kit.Editor;
using Emilia.Node.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    [Serializable]
    public class StateMachineConditionGroup
    {
        [LabelText("描述")]
        public string description;

        [LabelText("条件列表"), HideReferenceObjectPicker,
         ListDrawerSettings(ShowFoldout = false, CustomAddFunction = nameof(Add), CustomRemoveElementFunction = nameof(Remove))]
        public List<EditorConditionAsset> conditionAssets = new List<EditorConditionAsset>();

        void Add()
        {
            EditorGraphView graphView = EditorGraphView.focusedGraphView;
            EditorStateMachineAsset editorStateMachineAsset = graphView.graphAsset as EditorStateMachineAsset;
            if (editorStateMachineAsset == null) return;

            EditorStateMachineUtility.ShowSubTypesMenu("选择条件", editorStateMachineAsset.conditionSubTypes, AddCondition);

            void AddCondition(Type type)
            {
                EditorConditionAsset editorConditionAsset = new EditorConditionAsset();

                IConditionAsset conditionAsset = ReflectUtility.CreateInstance(type) as IConditionAsset;
                editorConditionAsset.conditionAsset = conditionAsset;

                graphView.RegisterCompleteObjectUndo("Add Condition");
                conditionAssets.Add(editorConditionAsset);
            }
        }

        void Remove(EditorConditionAsset item)
        {
            EditorGraphView graphView = EditorGraphView.focusedGraphView;
            graphView.RegisterCompleteObjectUndo("Remove Condition");
            conditionAssets.Remove(item);
        }
    }

    [Serializable]
    public class EditorConditionAsset
    {
        [SerializeField, HideInInspector]
        private IConditionAsset _conditionAsset;

        [ShowInInspector, HideLabel, HideReferenceObjectPicker]
        public IConditionAsset conditionAsset
        {
            get => _conditionAsset;
            set => _conditionAsset = value;
        }
    }
}