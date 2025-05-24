﻿#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Emilia.Kit
{
    [CustomEditor(typeof(TitleAsset), true), CanEditMultipleObjects]
    public class EditorTitleAsset : OdinEditor
    {
        private GUIStyle headerStyle;

        protected override void OnHeaderGUI()
        {
            if (this.headerStyle == null) InitStyle();
            TitleAsset titleAsset = target as TitleAsset;
            GUILayout.Box(new GUIContent(titleAsset.title), headerStyle, GUILayout.ExpandWidth(true), GUILayout.Height(60));
            GUILayout.Space(5);
        }

        protected void InitStyle()
        {
            headerStyle = new GUIStyle(GUI.skin.box);
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.fontSize = 36;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = Color.white;
            headerStyle.hover.textColor = Color.white;
            headerStyle.active.textColor = Color.white;
        }
    }
}
#endif