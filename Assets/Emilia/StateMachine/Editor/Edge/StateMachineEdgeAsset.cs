using System;
using System.Collections.Generic;
using System.Linq;
using Emilia.Node.Editor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Emilia.StateMachine.Editor
{
    [Serializable, HideMonoScript]
    public class StateMachineEdgeAsset : EditorEdgeAsset
    {
        [NonSerialized, OdinSerialize, HideInInspector]
        public List<StateMachineConditionGroup> inputCondition = new List<StateMachineConditionGroup>();

        [NonSerialized, OdinSerialize, HideInInspector]
        public List<StateMachineConditionGroup> outputCondition = new List<StateMachineConditionGroup>();

        public override string title => "过渡编辑";

        private bool outputOrInput;
        private int index = -1;

        private bool isShow => this.index >= 0;

        [OnInspectorGUI, PropertyOrder(-1)]
        public void EditorListDraw()
        {
            EditorGraphView graphView = EditorGraphView.focusedGraphView;
            if (graphView == null) return;

            StateMachineEdgeView edgeView = graphView.graphElementCache.edgeViewById.GetValueOrDefault(id) as StateMachineEdgeView;
            if (edgeView == null) return;
            if (edgeView.isDrag || edgeView.inputPortView == null || edgeView.outputPortView == null)
            {
                index = -1;
                return;
            }

            StateMachineNodeView inputNodeView = edgeView.inputPortView.master as StateMachineNodeView;
            StateMachineNodeView outputNodeView = edgeView.outputPortView.master as StateMachineNodeView;
            SirenixEditorGUI.BeginBox("转换列表");

            for (int i = 0; i < inputCondition.Count; i++)
            {
                if (index == -1)
                {
                    outputOrInput = false;
                    index = i;
                }

                bool isSelect = outputOrInput == false && this.index == i;

                GUILayout.BeginHorizontal();

                GUI.color = isSelect ? Color.cyan : Color.white;

                string labelString = outputNodeView.title + "->" + inputNodeView.title;
                if (GUILayout.Button(labelString, "ToolbarButton"))
                {
                    outputOrInput = false;
                    index = i;
                }

                GUI.color = Color.white;

                if (GUILayout.Button("X", "ToolbarButton", GUILayout.Width(25)))
                {
                    if (isSelect) index = -1;
                    edgeView.OnValueChanged();
                    edgeView.graphView.RegisterCompleteObjectUndo("Remove ConditionGroup");
                    inputCondition.RemoveAt(i);
                    i--;
                }

                GUILayout.EndHorizontal();
            }

            for (int i = 0; i < outputCondition.Count; i++)
            {
                if (index == -1)
                {
                    outputOrInput = true;
                    index = i;
                }

                bool isSelect = outputOrInput && this.index == i;

                GUILayout.BeginHorizontal();

                GUI.color = isSelect ? Color.cyan : Color.white;

                string labelString = inputNodeView.title + "->" + outputNodeView.title;
                if (GUILayout.Button(labelString, "ToolbarButton"))
                {
                    outputOrInput = true;
                    index = i;
                }

                GUI.color = Color.white;

                if (GUILayout.Button("X", "ToolbarButton", GUILayout.Width(25)))
                {
                    if (isSelect) this.index = -1;
                    edgeView.OnValueChanged();
                    edgeView.graphView.RegisterCompleteObjectUndo("Remove ConditionGroup");
                    outputCondition.RemoveAt(i);
                    i--;
                }

                GUILayout.EndHorizontal();
            }

            SirenixEditorGUI.EndBox();

        }

        [ShowInInspector, HideReferenceObjectPicker, HideLabel, ShowIf(nameof(isShow))]
        public StateMachineConditionGroup editorConditionGroup
        {
            get
            {
                if (isShow == false) return default;
                if (this.outputOrInput) return this.outputCondition[this.index];
                else return this.inputCondition[this.index];
            }

            set { }
        }
    }

    [EditorEdge(typeof(StateMachineEdgeAsset))]
    public class StateMachineEdgeView : EditorEdgeView
    {
        private const float offset = 12f;

        private StateMachineEdgeAsset stateMachineEdgeAsset;

        private VisualElement outputArrow;
        private VisualElement inputArrow;

        public override void Initialize(EditorGraphView graphView, EditorEdgeAsset asset)
        {
            base.Initialize(graphView, asset);
            this.stateMachineEdgeAsset = asset as StateMachineEdgeAsset;

            StyleSheet styleSheet = ResourceUtility.LoadResource<StyleSheet>("StateMachine/Styles/StateMachineEdge.uss");
            styleSheets.Add(styleSheet);
        }

        public override void OnValueChanged()
        {
            base.OnValueChanged();
            UpdateIcon();
            UpdateArrowTransform();
        }

        public override bool UpdateEdgeControl()
        {
            bool isUpdate = base.UpdateEdgeControl();

            UpdateIcon();
            if (editorEdgeControl.renderPointsDirty_Internals) UpdateArrowTransform();

            return isUpdate;
        }

        private void UpdateIcon()
        {
            if (this.stateMachineEdgeAsset == null) return;
            int outputAmount = stateMachineEdgeAsset.outputCondition.Count;

            if (outputAmount > 0)
            {
                if (this.outputArrow == null)
                {
                    outputArrow = new VisualElement();
                    outputArrow.style.position = Position.Absolute;
                    Add(this.outputArrow);
                }

                if (outputAmount > 1)
                {
                    if (this.outputArrow.name != "outputArrows")
                    {
                        outputArrow.name = "outputArrows";
                        Texture2D icon = ResourceUtility.LoadResource<Texture2D>("StateMachine/Icons/OutputArrows.png");
                        outputArrow.style.backgroundImage = icon;
                    }
                }
                else
                {
                    if (this.outputArrow.name != "outputArrow")
                    {
                        outputArrow.name = "outputArrow";
                        Texture2D icon = ResourceUtility.LoadResource<Texture2D>("StateMachine/Icons/OutputArrow.png");
                        outputArrow.style.backgroundImage = icon;
                    }
                }
            }
            else
            {
                if (this.outputArrow != null)
                {
                    outputArrow.RemoveFromHierarchy();
                    this.outputArrow = null;
                }
            }

            int inputAmount = stateMachineEdgeAsset.inputCondition.Count;

            if (inputAmount > 0)
            {
                if (this.inputArrow == null)
                {
                    this.inputArrow = new VisualElement();
                    inputArrow.style.position = Position.Absolute;
                    Add(this.inputArrow);
                }

                if (inputAmount > 1)
                {
                    if (this.inputArrow.name != "inputArrows")
                    {
                        inputArrow.name = "inputArrows";
                        Texture2D icon = ResourceUtility.LoadResource<Texture2D>("StateMachine/Icons/InputArrows.png");
                        inputArrow.style.backgroundImage = icon;
                    }
                }
                else
                {
                    if (this.inputArrow.name != "inputArrow")
                    {
                        inputArrow.name = "inputArrow";
                        Texture2D icon = ResourceUtility.LoadResource<Texture2D>("StateMachine/Icons/InputArrow.png");
                        inputArrow.style.backgroundImage = icon;
                    }
                }
            }
            else
            {
                if (this.inputArrow != null)
                {
                    inputArrow.RemoveFromHierarchy();
                    this.inputArrow = null;
                }
            }
        }

        private void UpdateArrowTransform()
        {
            if (PointsAndTangents == null) return;
            Vector2 centerPoint = GetPointByRate(0.5f);

            Vector2 firstPoint = PointsAndTangents.First();
            Vector2 lastPoint = PointsAndTangents.Last();

            if (this.inputArrow != null)
            {
                Vector2 targetPosition = centerPoint;
                Vector2 direction = lastPoint - firstPoint;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
                float angle = rotation.eulerAngles.z - 90;

                targetPosition -= direction.normalized * offset;

                this.inputArrow.transform.rotation = Quaternion.Euler(0, 0, angle);
                this.inputArrow.transform.position = targetPosition;
            }

            if (this.outputArrow != null)
            {
                Vector2 targetPosition = centerPoint;
                Vector2 direction = firstPoint - lastPoint;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
                float angle = rotation.eulerAngles.z + 90;

                targetPosition -= direction.normalized * offset;

                this.outputArrow.transform.rotation = Quaternion.Euler(0, 0, angle);
                this.outputArrow.transform.position = targetPosition;
            }
        }
    }
}