using System.Collections.Generic;
using Emilia.Kit;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;

namespace Emilia.StateMachine.Editor
{
    [EditorHandle(typeof(EditorStateMachineAsset))]
    public class StateMachineGraphSelectedHandle : UniversalGraphSelectedHandle
    {
        public override void CollectSelectedDrawer(EditorGraphView graphView, List<IGraphSelectedDrawer> drawers)
        {
            drawers.Add(new LinkSelectedDrawer());

        }
    }
}