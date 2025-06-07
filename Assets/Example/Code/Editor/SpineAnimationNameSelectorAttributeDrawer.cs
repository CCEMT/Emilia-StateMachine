using Emilia.Kit;
using Emilia.Node.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Emilia.SpineAnimator.Editor
{
    public class SpineAnimationNameSelectorAttributeDrawer : OdinAttributeDrawer<SpineAnimationNameSelectorAttribute>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (Property.ValueEntry.TypeOfValue != typeof(string)) return;

            EditorGraphView editorGraphView = EditorGraphView.focusedGraphView;
            if (editorGraphView == null) return;

            EditorSpineAnimatorAsset editorSpineAnimatorAsset = editorGraphView.graphAsset as EditorSpineAnimatorAsset;
            if (editorSpineAnimatorAsset == null) return;

            if (editorSpineAnimatorAsset.skeletonDataAsset == null) return;

            GUILayout.BeginHorizontal();

            if (label != null) GUILayout.Label(label);

            string labelString = (string) Property.ValueEntry.WeakSmartValue;
            if (GUILayout.Button(labelString, "MiniPopup"))
            {
                OdinMenu odinMenu = new OdinMenu();
                var animationNames = editorSpineAnimatorAsset.skeletonDataAsset.GetSkeletonData(true).Animations;
                foreach (var animation in animationNames) odinMenu.AddItem(animation.Name, () => Property.ValueEntry.WeakSmartValue = animation.Name);
                odinMenu.ShowInPopup();
            }

            GUILayout.EndHorizontal();
        }
    }
}