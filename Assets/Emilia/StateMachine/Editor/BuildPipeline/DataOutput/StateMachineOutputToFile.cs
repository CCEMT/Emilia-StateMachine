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
    [BuildPipeline(StateMachineBuildPipeline.PipelineName), BuildSequence(2000)]
    public class StateMachineOutputToFile : IDataOutput
    {
        public void Output(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;
            StateMachineBuildArgs args = buildArgs as StateMachineBuildArgs;

            if (string.IsNullOrEmpty(args.outputPath))
            {
                onFinished.Invoke();
                return;
            }

            string dataPathNoAssets = Directory.GetParent(Application.dataPath).ToString();
            string path = $"{dataPathNoAssets}/{args.outputPath}/{container.editorStateMachineAsset.name}.bytes";

            onFinished.Invoke();

            Task.Run(() => {
                if (File.Exists(path)) File.Delete(path);
                byte[] bytes = TagSerializationUtility.IgnoreTagSerializeValue(container.stateMachineAsset, DataFormat.Binary, SerializeTagDefine.DefaultIgnoreTag);
                File.WriteAllBytes(path, bytes);
                EditorKit.UnityInvoke(RefreshAssetDatabase);
            });

            void RefreshAssetDatabase()
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}