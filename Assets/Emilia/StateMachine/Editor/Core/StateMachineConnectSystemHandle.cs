using System;
using Emilia.Node.Editor;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineConnectSystemHandle : ConnectSystemHandle<EditorStateMachineAsset>
    {
        public override Type GetEdgeTypeByPort(IEditorPortView portView)
        {
            return typeof(StateMachineEdgeAsset);
        }

        public override bool BeforeConnect(IEditorPortView input, IEditorPortView output)
        {
            IEditorEdgeView connectEdgeView = smartValue.graphElementCache.GetEdgeView(input, output);
            return AddCondition(connectEdgeView, input, output);
        }

        public override void AfterConnect(IEditorEdgeView edgeView)
        {
            AddCondition(edgeView);
        }

        private bool AddCondition(IEditorEdgeView edgeView, IEditorPortView input = null, IEditorPortView output = null)
        {
            StateMachineEdgeView stateMachineEdgeView = edgeView as StateMachineEdgeView;
            if (stateMachineEdgeView == null) return false;
            StateMachineEdgeAsset stateMachineEdgeAsset = stateMachineEdgeView.asset as StateMachineEdgeAsset;
            if (stateMachineEdgeAsset == null) return false;

            bool isInversion = false;
            if (input != null && output != null) isInversion = input != edgeView.inputPortView && output != edgeView.outputPortView;

            bool isInput = (stateMachineEdgeView.inputPortView.portDirection == EditorPortDirection.Input ||
                            stateMachineEdgeView.inputPortView.portDirection == EditorPortDirection.InputOutput) &&
                           (stateMachineEdgeView.outputPortView.portDirection == EditorPortDirection.Output ||
                            stateMachineEdgeView.outputPortView.portDirection == EditorPortDirection.InputOutput);

            if (isInput)
            {
                smartValue.RegisterCompleteObjectUndo("Add ConditionGroup");
                if (isInversion == false) stateMachineEdgeAsset.inputCondition.Add(new StateMachineConditionGroup());
                else stateMachineEdgeAsset.outputCondition.Add(new StateMachineConditionGroup());
                edgeView.OnValueChanged();
                smartValue.UpdateSelectedInspector();
                return true;
            }

            bool isOutput = stateMachineEdgeView.inputPortView.portDirection == EditorPortDirection.Output && (
                stateMachineEdgeView.outputPortView.portDirection == EditorPortDirection.Input ||
                stateMachineEdgeView.outputPortView.portDirection == EditorPortDirection.InputOutput
            );

            if (isOutput)
            {
                smartValue.RegisterCompleteObjectUndo("Add ConditionGroup");
                if (isInversion == false) stateMachineEdgeAsset.outputCondition.Add(new StateMachineConditionGroup());
                else stateMachineEdgeAsset.inputCondition.Add(new StateMachineConditionGroup());
                edgeView.OnValueChanged();
                smartValue.UpdateSelectedInspector();
                return true;
            }

            return false;
        }
    }
}