using System;
using System.IO;
using System.Threading.Tasks;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Kit;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(typeof(StateMachineBuildArgs)), BuildSequence(2000)]
    public class StateMachineOutputToFile : IDataOutput
    {
        public void Output(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;
            StateMachineBuildArgs args = buildArgs as StateMachineBuildArgs;

            if (args.isGenerateFile == false || string.IsNullOrEmpty(args.outputPath))
            {
                onFinished.Invoke();
                return;
            }

            string dataPathNoAssets = Directory.GetParent(Application.dataPath).ToString();
            string path = $"{dataPathNoAssets}/{args.outputPath}/{container.editorStateMachineAsset.name}.bytes";

            onFinished.Invoke();

            Task.Run(() => {
                
                try
                {
                    if (File.Exists(path)) File.Delete(path);
                    byte[] bytes = TagSerializationUtility.IgnoreTagSerializeValue(container.stateMachineAsset, DataFormat.Binary, SerializeTagDefine.DefaultIgnoreTag);
                    File.WriteAllBytes(path, bytes);
                    EditorApplication.delayCall += RefreshAssetDatabase;
                }
                catch (Exception e)
                {
                    EditorApplication.delayCall += () => Debug.LogError(e.ToUnityLogString());
                }
                
            });

            void RefreshAssetDatabase()
            {
                if (args.isSaveAsset) AssetDatabase.SaveAssets();
                if (args.isRefresh) AssetDatabase.Refresh();
                args.generateFileCallback?.Invoke();
            }
        }
    }
}