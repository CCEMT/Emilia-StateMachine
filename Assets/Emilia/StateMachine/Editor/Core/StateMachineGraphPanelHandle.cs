using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineGraphPanelHandle : GraphPanelHandle<EditorStateMachineAsset>
    {
        public override void LoadPanel(GraphPanelSystem system)
        {
            system.OpenDockPanel<StateMachineToolbarView>(20, GraphDockPosition.Top);
            system.OpenDockPanel<LayerView>(22, GraphDockPosition.Top);
        }
    }
}