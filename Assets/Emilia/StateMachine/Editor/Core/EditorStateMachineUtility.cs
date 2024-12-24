using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Kit;
using UnityEditor;

namespace Emilia.StateMachine.Editor
{
    public static class EditorStateMachineUtility
    {
        public static void DataBuild(EditorStateMachineAsset editorStateMachineAsset, Action<BuildReport> onBuildComplete = null)
        {
            if (editorStateMachineAsset == null) return;

            string path = editorStateMachineAsset.outputFilePath;

            StateMachineBuildArgs stateMachineBuildArgs = new StateMachineBuildArgs(editorStateMachineAsset, path);
            stateMachineBuildArgs.onBuildComplete = onBuildComplete;

            DataBuildUtility.Build(stateMachineBuildArgs);
        }

        public static void DataBuildAll<T>() where T : EditorStateMachineAsset
        {
            T[] editorStateMachineAssets = EditorAssetKit.GetEditorResources<T>();
            int amount = editorStateMachineAssets.Length;
            for (int i = 0; i < amount; i++)
            {
                EditorStateMachineAsset editorStateMachineAsset = editorStateMachineAssets[i];
                DataBuild(editorStateMachineAsset);
            }
        }

        public static void ShowSubTypesMenu(string title, Type[] subTypes, Action<Type> onSelected)
        {
            List<Type> types = GetSubTypes(subTypes);

            OdinMenu odinMenu = new OdinMenu(title);

            int amount = types.Count;
            for (int i = 0; i < amount; i++)
            {
                Type type = types[i];

                StateMachineHideAttribute hideAttribute = type.GetCustomAttribute<StateMachineHideAttribute>();
                if (hideAttribute != null) continue;

                string label = type.Name;
                StateMachineTitleAttribute titleAttribute = type.GetCustomAttribute<StateMachineTitleAttribute>();
                if (titleAttribute != null) label = titleAttribute.title;

                odinMenu.AddItem(label, () => onSelected?.Invoke(type));
            }

            odinMenu.ShowInPopup();
        }

        public static List<Type> GetSubTypes(Type[] types)
        {
            List<Type> subTypes = new List<Type>();

            int amount = types.Length;
            for (int i = 0; i < amount; i++)
            {
                Type type = types[i];
                subTypes.AddRange(TypeCache.GetTypesDerivedFrom(type).Where((t) => t.IsAbstract == false && t.IsInterface == false));
            }

            return subTypes;
        }
    }
}