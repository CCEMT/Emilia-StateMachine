﻿using Emilia.Kit;
using Emilia.Node.Editor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Emilia.Node.Universal.Editor
{
    [EditorHandle(typeof(EditorUniversalGraphAsset))]
    public class UniversalGraphHandle : GraphHandle
    {
        public const string GridBackgroundStyleFilePath = "Node/Styles/GridBackground.uss";
        public const string GraphViewStyleFilePath = "Node/Styles/UniversalEditorGraphView.uss";

        private EditorGraphView editorGraphView;
        private GraphLoadingContainer loadingContainer;
        private GraphCompilationContainer currentCompilationContainer;

        public override void Initialize(EditorGraphView graphView)
        {
            base.Initialize(graphView);
            editorGraphView = graphView;
        }

        public override void OnLoadBefore(EditorGraphView graphView)
        {
            base.OnLoadBefore(graphView);
            AddManipulator();
            GraphViewInitialize();
            AddGridBackground();
            AddLoadingMask();

            graphView.onLogicTransformChange -= OnLogicTransformChange;
            graphView.onLogicTransformChange += OnLogicTransformChange;

            CompilationPipeline.compilationStarted -= OnCompilationStarted;
            CompilationPipeline.compilationStarted += OnCompilationStarted;
        }

        private void OnLogicTransformChange(Vector3 position, Vector3 scale)
        {
            UniversalGraphAssetLocalSetting setting = editorGraphView.graphLocalSettingSystem.assetSetting as UniversalGraphAssetLocalSetting;
            setting.position = position;
            setting.scale = scale;
        }

        private void OnCompilationStarted(object context)
        {
            currentCompilationContainer = new GraphCompilationContainer();
            editorGraphView.Add(currentCompilationContainer);
            editorGraphView.SetEnabled(false);

            editorGraphView.onUpdate += CheckCompilationFinished;
        }

        private void CheckCompilationFinished()
        {
            if (EditorApplication.isCompiling) return;
            this.currentCompilationContainer.RemoveFromHierarchy();

            if (this.editorGraphView != null) this.editorGraphView.SetEnabled(true);

            EditorApplication.update -= CheckCompilationFinished;
            this.currentCompilationContainer = null;
        }

        protected virtual void AddManipulator()
        {
            editorGraphView.AddManipulator(new ContentDragger());
            editorGraphView.AddManipulator(new GraphSelectionDragger());
            editorGraphView.AddManipulator(new GraphRectangleSelector());
        }

        protected void AddGridBackground()
        {
            GridBackground background = new GridBackground();
            StyleSheet styleSheet = ResourceUtility.LoadResource<StyleSheet>(GridBackgroundStyleFilePath);
            background.styleSheets.Add(styleSheet);

            editorGraphView.Insert(0, background);
        }

        protected void GraphViewInitialize()
        {
            StyleSheet graphViewStyleSheet = ResourceUtility.LoadResource<StyleSheet>(GraphViewStyleFilePath);
            editorGraphView.styleSheets.Add(graphViewStyleSheet);
        }

        private void AddLoadingMask()
        {
            if (this.loadingContainer == null)
            {
                this.loadingContainer = new GraphLoadingContainer(editorGraphView);
                editorGraphView.Add(this.loadingContainer);
            }

            this.loadingContainer.style.display = DisplayStyle.Flex;
            this.loadingContainer.DisplayLoading();

            editorGraphView.SetEnabled(false);
        }

        public override void OnLoadAfter(EditorGraphView graphView)
        {
            base.OnLoadAfter(graphView);
            this.loadingContainer.style.display = DisplayStyle.None;
            graphView.SetEnabled(true);

            UniversalGraphAssetLocalSetting universalSetting = graphView.graphLocalSettingSystem.assetSetting as UniversalGraphAssetLocalSetting;
            graphView.UpdateViewTransform(universalSetting.position, universalSetting.scale);
        }

        public override void Dispose(EditorGraphView graphView)
        {
            graphView.onLogicTransformChange -= OnLogicTransformChange;
            CompilationPipeline.compilationStarted -= OnCompilationStarted;
            graphView.onUpdate -= CheckCompilationFinished;

            
            editorGraphView = null;
            currentCompilationContainer = null;
        }
    }
}