using System;
using System.Collections.Generic;
using Emilia.Kit.Editor;
using Emilia.Node.Attributes;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    [NodeMenu("状态节点"), HideMonoScript]
    public class StateNodeAsset : StateMachineNodeAsset
    {
        [LabelText("状态名称"), ShowInInspector, PropertyOrder(-1)]
        public string stateName
        {
            get => displayName;
            set => displayName = value;
        }

        [LabelText("状态ID"), SerializeField]
        public uint stateId;

        [NonSerialized, OdinSerialize, HideReferenceObjectPicker, HideLabel]
        public EditorStateComponentGroup componentGroup = new EditorStateComponentGroup();

        public override int stateMachineId => (int) this.stateId;
        public override string title => "状态节点";
    }

    [Serializable]
    public class EditorStateComponentGroup
    {
        [LabelText("组件列表"), HideReferenceObjectPicker,
         ListDrawerSettings(ShowFoldout = false, CustomAddFunction = nameof(Add), CustomRemoveElementFunction = nameof(Remove))]
        public List<EditorStateComponentAsset> componentAssets = new List<EditorStateComponentAsset>();

        private void Add()
        {
            EditorGraphView graphView = EditorGraphView.focusedGraphView;
            EditorStateMachineAsset editorStateMachineAsset = graphView.graphAsset as EditorStateMachineAsset;
            if (editorStateMachineAsset == null) return;

            EditorStateMachineUtility.ShowSubTypesMenu("选择组件", editorStateMachineAsset.stateComponentSubTypes, AddComponent);

            void AddComponent(Type type)
            {
                EditorStateComponentAsset editorStateComponentAsset = new EditorStateComponentAsset();

                IStateComponentAsset addItem = ReflectUtility.CreateInstance(type) as IStateComponentAsset;
                editorStateComponentAsset.componentAsset = addItem;

                graphView.RegisterCompleteObjectUndo("Add Component");
                componentAssets.Add(editorStateComponentAsset);
            }
        }

        private void Remove(EditorStateComponentAsset item)
        {
            EditorGraphView graphView = EditorGraphView.focusedGraphView;
            graphView.RegisterCompleteObjectUndo("Remove Component");
            componentAssets.Remove(item);
        }
    }

    [Serializable]
    public class EditorStateComponentAsset
    {
        [SerializeField, HideReferenceObjectPicker, HideLabel]
        private IStateComponentAsset _componentAsset;

        public IStateComponentAsset componentAsset
        {
            get => this._componentAsset;
            set => this._componentAsset = value;
        }
    }

    [EditorNode(typeof(StateNodeAsset))]
    public class StateNodeView : StateMachineNodeView
    {
        protected override EditorPortDirection portDirection => EditorPortDirection.Any;
        public StateNodeAsset stateNodeAsset { get; private set; }

        protected override bool canRename => true;

        public override void Initialize(EditorGraphView graphView, EditorNodeAsset asset)
        {
            base.Initialize(graphView, asset);
            SetColor(Color.gray);
            stateNodeAsset = asset as StateNodeAsset;
        }
    }
}