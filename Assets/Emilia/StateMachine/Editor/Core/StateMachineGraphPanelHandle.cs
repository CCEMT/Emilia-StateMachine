using Emilia.Kit;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;

namespace Emilia.StateMachine.Editor
{
    [EditorHandle(typeof(EditorStateMachineAsset))]
    public class StateMachineGraphPanelHandle : GraphPanelHandle
    {
        public override void LoadPanel(EditorGraphView graphView, GraphPanelSystem system)
        {
            base.LoadPanel(graphView, system);
            system.OpenDockPanel<StateMachineToolbarView>(20, GraphDockPosition.Top);
            system.OpenDockPanel<LayerView>(22, GraphDockPosition.Top);
        }
    }
}