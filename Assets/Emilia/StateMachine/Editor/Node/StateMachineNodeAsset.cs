using System.Collections.Generic;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using UnityEngine.UIElements;

namespace Emilia.StateMachine.Editor
{
    public abstract class StateMachineNodeAsset : UniversalNodeAsset
    {
        public abstract int stateMachineId { get; }
    }

    public abstract class StateMachineNodeView : UniversalEditorNodeView
    {
        public const string PortId = "Port";

        public StateMachineNodeAsset stateMachineNodeAsset { get; private set; }
        protected VisualElement connectorElement { get; private set; }
        protected abstract EditorPortDirection portDirection { get; }
        public override bool canExpanded => false;

        public override void Initialize(EditorGraphView graphView, EditorNodeAsset asset)
        {
            base.Initialize(graphView, asset);
            stateMachineNodeAsset = asset as StateMachineNodeAsset;

            connectorElement = this.Q<VisualElement>("connector");
            connectorElement.style.position = Position.Absolute;
            RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);

            titleLabel.style.marginRight = 6;
        }

        protected virtual void OnGeometryChangedEvent(GeometryChangedEvent evt)
        {
            VisualElement connectorParent = connectorElement.parent;

            connectorElement.style.left = connectorParent.layout.width / 2 - connectorElement.layout.width;
            connectorElement.style.top = (connectorParent.layout.height - connectorElement.layout.height) / 2;
        }

        public override List<EditorPortInfo> CollectStaticPortAssets()
        {
            List<EditorPortInfo> portAssets = new List<EditorPortInfo>();

            EditorPortInfo outputPort = new EditorPortInfo();
            outputPort.id = PortId;
            outputPort.orientation = EditorOrientation.Custom;
            outputPort.direction = portDirection;
            outputPort.canMultiConnect = true;
            portAssets.Add(outputPort);

            return portAssets;
        }

        protected override void AddCustomPortView(IEditorPortView portView, EditorPortInfo info)
        {
            portNodeBottomContainer.Add(portView.portElement);
        }
    }
}