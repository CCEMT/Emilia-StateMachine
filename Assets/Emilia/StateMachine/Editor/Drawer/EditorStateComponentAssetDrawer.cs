using Emilia.Kit.Editor;
using Emilia.Node.Editor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    public class EditorStateComponentAssetDrawer : OdinProValueDrawer<EditorStateComponentAsset>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGraphView graphView = EditorGraphView.focusedGraphView;
            CallNextDrawer(label);
        }
    }
}