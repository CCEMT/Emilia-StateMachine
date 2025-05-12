using System.Collections.Generic;
using System.Linq;
using Emilia.Kit;
using Emilia.Node.Attributes;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Emilia.StateMachine.Editor
{
    [NodeMenu("子状态机")]
    public class SubStateMachineNodeAsset : StateNodeAsset
    {
        [SerializeField, HideInInspector]
        private EditorStateMachineAsset _editorStateMachineAsset;

        public EditorStateMachineAsset editorStateMachineAsset
        {
            get => this._editorStateMachineAsset;
            set => this._editorStateMachineAsset = value;
        }

        public override string title => "子状态机";

        public override void SetChildren(List<Object> childAssets)
        {
            _editorStateMachineAsset = null;
            base.SetChildren(childAssets);

            EditorStateMachineAsset asset = childAssets.OfType<EditorStateMachineAsset>().FirstOrDefault();
            if (asset == null) return;

            this._editorStateMachineAsset = asset;
            EditorAssetKit.SaveAssetIntoObject(this._editorStateMachineAsset, this);
        }

        public override List<Object> GetChildren()
        {
            var list = base.GetChildren();
            list.Add(this._editorStateMachineAsset);
            return list;
        }
    }

    [EditorNode(typeof(SubStateMachineNodeAsset))]
    public class SubStateMachineNodeView : StateNodeView
    {
        private SubStateMachineNodeAsset subStateMachineNodeAsset;
        protected override EditorPortDirection portDirection => EditorPortDirection.Any;

        public override void Initialize(EditorGraphView graphView, EditorNodeAsset asset)
        {
            base.Initialize(graphView, asset);
            subStateMachineNodeAsset = asset as SubStateMachineNodeAsset;
            SetColor(new Color(1, 0.5f, 0, 1));

            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.clickCount == 2) graphView.Reload(subStateMachineNodeAsset.editorStateMachineAsset);
        }

        public override void OnValueChanged(bool isSilent = false)
        {
            base.OnValueChanged(isSilent);
            subStateMachineNodeAsset.editorStateMachineAsset.name = string.IsNullOrEmpty(subStateMachineNodeAsset.stateName) ? subStateMachineNodeAsset.title : subStateMachineNodeAsset.stateName;
        }
    }
}