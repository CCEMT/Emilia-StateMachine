using System.Reflection;
using Emilia.Kit.Editor;
using Emilia.Node.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineTitleAttributeDrawer : OdinAttributeDrawer<StateMachineTitleAttribute>
    {
        private GUIStyle labelStyle;
        private GUIStyle menuStyle;

        public StateMachineTitleAttributeDrawer()
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 12;
            this.labelStyle.fontStyle = FontStyle.Bold;

            menuStyle = new GUIStyle(GUIStyle.none);
            menuStyle.alignment = TextAnchor.MiddleCenter;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUILayout.Space(3);

            GUILayout.Box(string.Empty, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(25f));

            Rect rect = GUILayoutUtility.GetLastRect();

            Vector2 size = labelStyle.CalcSize(new GUIContent(Attribute.title));

            Rect labelRect = rect;
            labelRect.width = size.x;
            GUI.Label(labelRect, Attribute.title, labelStyle);

            Rect menuRect = rect;
            float buttonWidth = 25f;
            menuRect.x = rect.xMax - buttonWidth;
            menuRect.width = buttonWidth;
            if (GUI.Button(menuRect, EditorGUIUtility.IconContent("d__Menu"), menuStyle)) ShowMenu();

            SirenixEditorGUI.HorizontalLineSeparator(SirenixGUIStyles.HighlightPropertyColor);
            GUILayout.Space(2);
            CallNextDrawer(label);
        }

        private void ShowMenu()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("打开脚本"), false, () => OpenScriptUtility.OpenScript(Property.ValueEntry.WeakSmartValue));

            CollectMenuItem(menu);

            menu.ShowAsContext();
        }

        private void CollectMenuItem(GenericMenu menu)
        {
            EditorGraphView graphView = EditorGraphView.focusedGraphView;
            if (graphView == null) return;

            EditorStateMachineAsset stateMachineAsset = graphView.graphAsset as EditorStateMachineAsset;
            if (stateMachineAsset == null) return;

            IStateComponentAsset stateComponentAsset = Property.ValueEntry.WeakSmartValue as IStateComponentAsset;
            IConditionAsset conditionAsset = Property.ValueEntry.WeakSmartValue as IConditionAsset;

            if (stateMachineAsset == null && stateComponentAsset == null && conditionAsset == null) return;

            MethodInfo[] methodInfos = stateMachineAsset.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            int count = methodInfos.Length;
            for (var i = 0; i < count; i++)
            {
                MethodInfo methodInfo = methodInfos[i];
                StateMachineTitleMenuAttribute menuAttribute = methodInfo.GetCustomAttribute<StateMachineTitleMenuAttribute>();
                if (menuAttribute == null) continue;

                if (stateComponentAsset != null && methodInfo.GetParameters().Length == 1 && typeof(IStateComponentAsset).IsAssignableFrom(methodInfo.GetParameters()[0].ParameterType))
                {
                    menu.AddItem(new GUIContent(menuAttribute.displayName), false, () => { methodInfo.Invoke(stateMachineAsset, new object[] {stateComponentAsset}); });
                    continue;
                }

                if (conditionAsset != null && methodInfo.GetParameters().Length == 1 && typeof(IConditionAsset).IsAssignableFrom(methodInfo.GetParameters()[0].ParameterType))
                {
                    menu.AddItem(new GUIContent(menuAttribute.displayName), false, () => { methodInfo.Invoke(stateMachineAsset, new object[] {conditionAsset}); });
                    continue;
                }

                if (methodInfo.GetParameters().Length == 1)
                {
                    menu.AddItem(new GUIContent(menuAttribute.displayName), false, () => { methodInfo.Invoke(stateMachineAsset, new[] {Property.ValueEntry.WeakSmartValue}); });
                    continue;
                }
            }
        }
    }
}