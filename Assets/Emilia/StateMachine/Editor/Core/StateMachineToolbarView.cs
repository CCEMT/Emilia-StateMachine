﻿using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Kit;
using Emilia.Node.Attributes;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using Emilia.Reflection.Editor;
using UnityEditor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineToolbarView : ToolbarView
    {
        private EditorStateMachineAsset stateMachineAsset;

        public override void Initialize(EditorGraphView graphView)
        {
            stateMachineAsset = graphView.graphAsset as EditorStateMachineAsset;
            base.Initialize(graphView);
        }

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

        private void OnEditorParameter()
        {
            EditorParametersManage editorParametersManage = stateMachineAsset.editorParametersManage;
            if (editorParametersManage == null)
            {
                editorParametersManage = stateMachineAsset.editorParametersManage = ScriptableObject.CreateInstance<EditorParametersManage>();
                EditorAssetKit.SaveAssetIntoObject(editorParametersManage, this.stateMachineAsset);
            }

            Selection.activeObject = editorParametersManage;
        }

        private void OnEditorRuntimeParameter()
        {
            GetStateMachineRunnerEvent getStateMachineRunnerEvent = GetStateMachineRunnerEvent.GetPooled();
            getStateMachineRunnerEvent.target = graphView;

            graphView.SendEvent_Internal(getStateMachineRunnerEvent, DispatchMode_Internals.Immediate);
            StateMachineRuntimeParameter stateMachineRuntimeParameter = new StateMachineRuntimeParameter(getStateMachineRunnerEvent.runner);
            EditorKit.SetSelection(stateMachineRuntimeParameter, "运行参数");
        }

        private OdinMenu BuildRunnerMenu()
        {
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
                if (string.IsNullOrEmpty(runner.editorStateMachineAsset.description) == false) itemName = $"{runner.editorStateMachineAsset.description}({runner.editorStateMachineAsset.name})";
                odinMenu.AddItem(itemName, () => {
                    SetStateMachineRunnerEvent e = SetStateMachineRunnerEvent.Create(runner);
                    e.target = graphView;
                    graphView.SendEvent(e);
                });
            }

            return odinMenu;
        }

        private void OnSave()
        {
            EditorStateMachineUtility.DataBuild(this.stateMachineAsset, (report) => {
                if (report.result == BuildResult.Succeeded) graphView.window.ShowNotification(new GUIContent("保存成功"), 1.5f);
            });
        }
    }
}