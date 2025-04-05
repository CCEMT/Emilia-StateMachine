using System.Collections.Generic;
using Emilia.Kit;
using Emilia.Node.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Emilia.StateMachine.Editor
{
    public class LinkSelectedDrawer : GraphSelectedDrawer
    {
        private Texture2D linkIcon;
        private Dictionary<StateNodeView, VisualElement> linkIcons = new Dictionary<StateNodeView, VisualElement>();

        public override void Initialize(EditorGraphView graphView)
        {
            base.Initialize(graphView);
            this.linkIcon = ResourceUtility.LoadResource<Texture2D>("StateMachine/Icons/LinkIcon.png");
        }

        public override void SelectedUpdate(List<ISelectedHandle> selection)
        {
            RemoveAllLinkIcon();

            int amount = graphView.nodeViews.Count;
            for (int i = 0; i < amount; i++)
            {
                StateNodeView nodeView = graphView.nodeViews[i] as StateNodeView;
                if (nodeView == null) continue;

                int componentAmount = nodeView.stateNodeAsset.componentGroup.componentAssets.Count;
                for (int j = 0; j < componentAmount; j++)
                {
                    EditorStateComponentAsset editorStateComponentAsset = nodeView.stateNodeAsset.componentGroup.componentAssets[j];
                    if (editorStateComponentAsset == null) continue;
                    AddLink(nodeView, editorStateComponentAsset, selection);
                }
            }
        }

        private void AddLink(StateNodeView nodeView, EditorStateComponentAsset editorStateComponentAsset, List<ISelectedHandle> selection)
        {
            ILinkState linkState = editorStateComponentAsset.componentAsset as ILinkState;
            if (linkState == null) return;

            StateNodeView linkNode = GetStateNodeViewByStateId(linkState.stateSelector.stateId);
            if (linkNode == null) return;

            bool isSelected = selection.Contains(nodeView) || selection.Contains(linkNode);
            if (isSelected) AddLinkIcon(nodeView, linkNode);
        }

        private StateNodeView GetStateNodeViewByStateId(int stateId)
        {
            int amount = graphView.nodeViews.Count;
            for (int i = 0; i < amount; i++)
            {
                IEditorNodeView editorNodeView = graphView.nodeViews[i];
                if (editorNodeView == null) continue;

                StateNodeView stateNodeView = editorNodeView as StateNodeView;
                if (stateNodeView == null) continue;

                if (stateNodeView.stateNodeAsset.stateId == stateId) return stateNodeView;
            }

            return default;
        }

        private void AddLinkIcon(StateNodeView thisNode, StateNodeView linkNode)
        {
            if (linkIcons.ContainsKey(thisNode) == false)
            {
                VisualElement linkIconElement = CreateLinkIcon();
                thisNode.Add(linkIconElement);
                linkIcons[thisNode] = linkIconElement;
            }

            if (linkIcons.ContainsKey(linkNode) == false)
            {
                VisualElement linkIconElement = CreateLinkIcon();
                linkNode.Add(linkIconElement);
                linkIcons[linkNode] = linkIconElement;
            }
        }

        private VisualElement CreateLinkIcon()
        {
            VisualElement linkIconElement = new VisualElement();
            linkIconElement.name = "linkIcon";
            linkIconElement.style.backgroundImage = this.linkIcon;
            linkIconElement.style.position = Position.Absolute;
            linkIconElement.style.alignSelf = Align.Center;
            linkIconElement.style.width = 24;
            linkIconElement.style.height = 24;
            linkIconElement.transform.position = new Vector3(0, -24, 0);
            return linkIconElement;
        }

        private void RemoveAllLinkIcon()
        {
            foreach (var pair in this.linkIcons) pair.Value.RemoveFromHierarchy();
            linkIcons.Clear();
        }

        public override void Dispose()
        {
            base.Dispose();
            RemoveAllLinkIcon();
            linkIcon = null;
        }
    }
}