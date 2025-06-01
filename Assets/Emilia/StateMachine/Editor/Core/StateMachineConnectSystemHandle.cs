using System;
using Emilia.Kit;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;

namespace Emilia.StateMachine.Editor
{
    [EditorHandle(typeof(EditorStateMachineAsset))]
    public class StateMachineConnectSystemHandle : UniversalConnectSystemHandle
    {
        public override Type GetEdgeAssetTypeByPort(EditorGraphView graphView, IEditorPortView portView) => typeof(StateMachineEdgeAsset);

        public override bool BeforeConnect(EditorGraphView graphView, IEditorPortView input, IEditorPortView output)
        {
            IEditorEdgeView connectEdgeView = graphView.graphElementCache.GetEdgeView(input, output);
            return AddCondition(graphView, connectEdgeView, input, output);
        }

        public override void AfterConnect(EditorGraphView graphView, IEditorEdgeView edgeView)
        {
            AddCondition(graphView, edgeView);
        }

        private bool AddCondition(EditorGraphView graphView, IEditorEdgeView edgeView, IEditorPortView input = null, IEditorPortView output = null)
        {
            StateMachineEdgeView stateMachineEdgeView = edgeView as StateMachineEdgeView;
            if (stateMachineEdgeView == null) return false;
            StateMachineEdgeAsset stateMachineEdgeAsset = stateMachineEdgeView.asset as StateMachineEdgeAsset;
            if (stateMachineEdgeAsset == null) return false;

            bool isInversion = false;
            if (input != null && output != null) isInversion = input != edgeView.inputPortView && output != edgeView.outputPortView;

            bool isInput = (stateMachineEdgeView.inputPortView.portDirection == EditorPortDirection.Input ||
                            stateMachineEdgeView.inputPortView.portDirection == EditorPortDirection.Any) &&
                           (stateMachineEdgeView.outputPortView.portDirection == EditorPortDirection.Output ||
                            stateMachineEdgeView.outputPortView.portDirection == EditorPortDirection.Any);

            if (isInput)
            {
                graphView.RegisterCompleteObjectUndo("Add ConditionGroup");
                if (isInversion == false) stateMachineEdgeAsset.inputCondition.Add(new StateMachineConditionGroup());
                else stateMachineEdgeAsset.outputCondition.Add(new StateMachineConditionGroup());
                edgeView.OnValueChanged();
                graphView.UpdateSelected();
                return true;
            }

            bool isOutput = stateMachineEdgeView.inputPortView.portDirection == EditorPortDirection.Output && (
                stateMachineEdgeView.outputPortView.portDirection == EditorPortDirection.Input ||
                stateMachineEdgeView.outputPortView.portDirection == EditorPortDirection.Any
            );

            if (isOutput)
            {
                graphView.RegisterCompleteObjectUndo("Add ConditionGroup");
                if (isInversion == false) stateMachineEdgeAsset.outputCondition.Add(new StateMachineConditionGroup());
                else stateMachineEdgeAsset.inputCondition.Add(new StateMachineConditionGroup());
                edgeView.OnValueChanged();
                graphView.UpdateSelected();
                return true;
            }

            return false;
        }
    }
}