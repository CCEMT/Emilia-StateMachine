﻿using System.Collections.Generic;

namespace Emilia.Node.Editor
{
    public static class IEditorNodeViewExtension
    {
        /// <summary>
        /// 根据Id获取NodeView
        /// </summary>
        public static IEditorNodeView GetEditorNodeView(this IEditorNodeView nodeView, string id) => nodeView.graphView.graphElementCache.nodeViewById.GetValueOrDefault(id);

        /// <summary>
        /// 根据Id获取EdgeView
        /// </summary>
        public static IEditorEdgeView GetEditorEdgeView(this IEditorNodeView nodeView, string id) => nodeView.graphView.graphElementCache.edgeViewById.GetValueOrDefault(id);

        /// <summary>
        /// 根据Id获取ItemView
        /// </summary>
        public static IEditorItemView GetEditorItemView(this IEditorNodeView nodeView, string id) => nodeView.graphView.graphElementCache.itemViewById.GetValueOrDefault(id);

        /// <summary>
        /// 获取所有Output节点
        /// </summary>
        public static List<IEditorNodeView> GetOutputNodeViews(this IEditorNodeView editorNodeView)
        {
            List<IEditorNodeView> outputNodeViews = new List<IEditorNodeView>();

            for (var i = 0; i < editorNodeView.graphView.edgeViews.Count; i++)
            {
                IEditorEdgeView edgeView = editorNodeView.graphView.edgeViews[i];

                if (edgeView.GetOutputNodeId() != editorNodeView.asset.id) continue;

                string inputNodeId = edgeView.GetInputNodeId();

                IEditorNodeView outputNodeView = editorNodeView.graphView.graphElementCache.nodeViewById.GetValueOrDefault(inputNodeId);
                if (outputNodeView == null) continue;

                outputNodeViews.Add(outputNodeView);
            }

            return outputNodeViews;
        }

        /// <summary>
        /// 获取所有Input节点
        /// </summary>
        public static List<IEditorNodeView> GetInputNodeViews(this IEditorNodeView editorNodeView)
        {
            List<IEditorNodeView> inputNodeViews = new List<IEditorNodeView>();

            for (var i = 0; i < editorNodeView.graphView.edgeViews.Count; i++)
            {
                IEditorEdgeView edgeView = editorNodeView.graphView.edgeViews[i];

                if (edgeView.GetInputNodeId() != editorNodeView.asset.id) continue;

                string outputNodeId = edgeView.GetOutputNodeId();

                IEditorNodeView inputNodeView = editorNodeView.GetEditorNodeView(outputNodeId);
                if (inputNodeView == null) continue;

                inputNodeViews.Add(inputNodeView);
            }

            return inputNodeViews;
        }

        /// <summary>
        /// 获取所有Output节点
        /// </summary>
        public static List<IEditorNodeView> GetAllOutputNodeViews(this IEditorNodeView editorNodeView)
        {
            List<IEditorNodeView> outputNodeViews = new List<IEditorNodeView>();

            for (var i = 0; i < editorNodeView.graphView.edgeViews.Count; i++)
            {
                IEditorEdgeView edgeView = editorNodeView.graphView.edgeViews[i];

                if (edgeView.GetOutputNodeId() != editorNodeView.asset.id) continue;

                string inputNodeId = edgeView.GetInputNodeId();

                IEditorNodeView outputNodeView = editorNodeView.GetEditorNodeView(inputNodeId);
                if (outputNodeView == null) continue;

                outputNodeViews.Add(outputNodeView);
                outputNodeViews.AddRange(outputNodeView.GetAllOutputNodeViews());
            }

            return outputNodeViews;
        }
        
        /// <summary>
        /// 获取所有Input节点
        /// </summary>
        public static List<IEditorNodeView> GetAllInputNodeViews(this IEditorNodeView editorNodeView)
        {
            List<IEditorNodeView> inputNodeViews = new List<IEditorNodeView>();

            for (var i = 0; i < editorNodeView.graphView.edgeViews.Count; i++)
            {
                IEditorEdgeView edgeView = editorNodeView.graphView.edgeViews[i];

                if (edgeView.GetInputNodeId() != editorNodeView.asset.id) continue;

                string outputNodeId = edgeView.GetOutputNodeId();

                IEditorNodeView inputNodeView = editorNodeView.GetEditorNodeView(outputNodeId);
                if (inputNodeView == null) continue;

                inputNodeViews.Add(inputNodeView);
                inputNodeViews.AddRange(inputNodeView.GetAllInputNodeViews());
            }

            return inputNodeViews;
        }

        /// <summary>
        /// 获取所有Input EdgeViews
        /// </summary>
        public static List<IEditorEdgeView> GetInputEdgeViews(this IEditorNodeView editorNodeView)
        {
            List<IEditorEdgeView> inputEdgeViews = new List<IEditorEdgeView>();

            for (var i = 0; i < editorNodeView.graphView.edgeViews.Count; i++)
            {
                IEditorEdgeView edgeView = editorNodeView.graphView.edgeViews[i];

                if (edgeView.GetInputNodeId() != editorNodeView.asset.id) continue;

                inputEdgeViews.Add(edgeView);
            }

            return inputEdgeViews;
        }
        
        /// <summary>
        /// 获取所有Output EdgeViews
        /// </summary>
        public static List<IEditorEdgeView> GetOutputEdgeViews(this IEditorNodeView editorNodeView)
        {
            List<IEditorEdgeView> outputEdgeViews = new List<IEditorEdgeView>();

            for (var i = 0; i < editorNodeView.graphView.edgeViews.Count; i++)
            {
                IEditorEdgeView edgeView = editorNodeView.graphView.edgeViews[i];

                if (edgeView.GetOutputNodeId() != editorNodeView.asset.id) continue;

                outputEdgeViews.Add(edgeView);
            }

            return outputEdgeViews;
        }
    }
}