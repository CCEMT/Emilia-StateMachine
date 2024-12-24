using System.Collections.Generic;
using Emilia.Node.Editor;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineGraphSelectedHandle : GraphSelectedHandle<EditorStateMachineAsset>
    {
        public override void CollectSelectedDrawer(List<IGraphSelectedDrawer> drawers)
        {
            drawers.Add(new LinkSelectedDrawer());
        }
    }
}