using System.Collections.Generic;
using Emilia.Kit;
using Emilia.Node.Editor;

namespace Emilia.StateMachine.Editor
{
    [EditorHandle(typeof(EditorStateMachineAsset))]
    public class StateMachineGraphSelectedHandle : GraphSelectedHandle
    {
        public override void CollectSelectedDrawer(EditorGraphView graphView, List<IGraphSelectedDrawer> drawers)
        {
            drawers.Add(new LinkSelectedDrawer());

        }
    }
}