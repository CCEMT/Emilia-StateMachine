using Emilia.Kit.Editor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    public class EditorConditionAssetDrawer : OdinProValueDrawer<EditorConditionAsset>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorConditionAsset editorConditionAsset = ValueEntry.SmartValue;
            CallNextDrawer(label);
        }
    }
}