using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Kit;
using Emilia.Node.Attributes;
using Emilia.Node.Universal.Editor;
using Emilia.Reflection.Editor;
using Emilia.Variables.Editor;
using UnityEditor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineToolbarView : ToolbarView
    {
        protected override void InitControls()
        {
            AddControl(new ButtonToolbarViewControl("参数", OnEditorParameter));

            if (EditorApplication.isPlaying)
            {
                AddControl(new ButtonToolbarViewControl("运行参数", OnEditorRuntimeParameter));

                AddControl(new DropdownButtonToolbarViewControl("运行实例", BuildRunnerMenu));
            }

            AddControl(new ButtonToolbarViewControl("保存", OnSave), ToolbarViewControlPosition.RightOrBottom);
        }

        protected virtual void OnEditorParameter()
        {
            EditorStateMachineAsset stateMachineAsset = this.graphView.graphAsset as EditorStateMachineAsset;
            EditorParametersManager editorParametersManage = stateMachineAsset.editorParametersManage;
            if (editorParametersManage == null)
            {
                editorParametersManage = stateMachineAsset.editorParametersManage = ScriptableObject.CreateInstance<EditorParametersManager>();
                EditorAssetKit.SaveAssetIntoObject(editorParametersManage, stateMachineAsset);
            }

            graphView.graphSelected.UpdateSelected(new List<ISelectedHandle> {editorParametersManage});
        }

        protected virtual void OnEditorRuntimeParameter()
        {
            GetStateMachineRunnerEvent getStateMachineRunnerEvent = GetStateMachineRunnerEvent.GetPooled();
            getStateMachineRunnerEvent.target = graphView;

            graphView.SendEvent_Internal(getStateMachineRunnerEvent, DispatchMode_Internals.Immediate);

            EditorStateMachineAsset stateMachineAsset = this.graphView.graphAsset as EditorStateMachineAsset;
            StateMachineRuntimeParameter stateMachineRuntimeParameter = new StateMachineRuntimeParameter(getStateMachineRunnerEvent.runner, stateMachineAsset);

            EditorKit.SetSelection(stateMachineRuntimeParameter, "运行参数");
        }

        protected virtual OdinMenu BuildRunnerMenu()
        {
            EditorStateMachineAsset stateMachineAsset = this.graphView.graphAsset as EditorStateMachineAsset;

            OdinMenu odinMenu = new OdinMenu();
            odinMenu.defaultWidth = 300;

            if (EditorStateMachineRunner.runnerByAssetId == null) return odinMenu;
            List<EditorStateMachineRunner> runners = EditorStateMachineRunner.runnerByAssetId.GetValueOrDefault(stateMachineAsset.id);
            if (runners == null) return odinMenu;

            int count = runners.Count;
            for (int i = 0; i < count; i++)
            {
                EditorStateMachineRunner runner = runners[i];
                string itemName = runner.stateMachine.owner.ToString();
                if (string.IsNullOrEmpty(runner.asset.description) == false) itemName = $"{runner.asset.description}({runner.fileName})";
                odinMenu.AddItem(itemName, () => {
                    SetStateMachineRunnerEvent e = SetStateMachineRunnerEvent.Create(runner);
                    e.target = graphView;
                    graphView.SendEvent(e);
                });
            }

            return odinMenu;
        }

        protected virtual void OnSave()
        {
            EditorStateMachineAsset stateMachineAsset = graphView.graphAsset as EditorStateMachineAsset;
            EditorStateMachineAsset rootStateMachineAsset = stateMachineAsset.GetRootAsset() as EditorStateMachineAsset;

            EditorStateMachineUtility.DataBuild(rootStateMachineAsset, (report) => {
                if (report.result == BuildResult.Succeeded) graphView.window.ShowNotification(new GUIContent("保存成功"), 1.5f);
            });
        }
    }
}