using System.Collections.Generic;
using System.Linq;
using Emilia.Node.Editor;
using UnityEditor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineGraphHandle : GraphHandle<EditorStateMachineAsset>
    {
        private EditorStateMachineRunner debugRunner;
        private StateMachineNodeView runningNodeView;

        public override void Initialize(object weakSmartValue)
        {
            base.Initialize(weakSmartValue);
            this.smartValue.RegisterCallback<GetStateMachineRunnerEvent>(OnGetStateMachineRunnerEvent);
            this.smartValue.RegisterCallback<SetStateMachineRunnerEvent>(OnSetStateMachineRunnerEvent);
        }

        public override void OnUpdate()
        {
            if (EditorApplication.isPlaying == false)
            {
                ClearRunner();
                return;
            }

            if (this.debugRunner == null)
            {
                List<EditorStateMachineRunner> runners = EditorStateMachineRunner.runnerByAssetId.GetValueOrDefault(smartValue.graphAsset.id);
                if (runners != null && runners.Count == 1) this.debugRunner = runners.FirstOrDefault();
            }
            else
            {
                if (EditorStateMachineRunner.runnerByAssetId.ContainsKey(smartValue.graphAsset.id) == false) ClearRunner();
                else if (EditorStateMachineRunner.runnerByAssetId[smartValue.graphAsset.id].Contains(this.debugRunner) == false) ClearRunner();
            }

            DrawDebug();
        }

        private void OnGetStateMachineRunnerEvent(GetStateMachineRunnerEvent evt)
        {
            evt.runner = this.debugRunner;
        }

        private void OnSetStateMachineRunnerEvent(SetStateMachineRunnerEvent evt)
        {
            ClearRunner();
            this.debugRunner = evt.runner;
        }

        private void DrawDebug()
        {
            if (this.debugRunner == null)
            {
                ClearRunner();
                return;
            }

            StateMachine stateMachine = null;

            Queue<StateMachine> queue = new Queue<StateMachine>();

            queue.Enqueue(this.debugRunner.stateMachine);

            while (queue.Count > 0)
            {
                StateMachine machine = queue.Dequeue();

                if (machine.asset.id == smartValue.graphAsset.id)
                {
                    stateMachine = machine;
                    break;
                }

                int childrenCount = machine.children.Count;
                for (int i = 0; i < childrenCount; i++)
                {
                    StateMachine child = machine.children[i];
                    queue.Enqueue(child);
                }
            }

            if (stateMachine == null)
            {
                ClearRunner();
                return;
            }

            if (stateMachine.runState != StateMachine.RunState.Running)
            {
                ClearRunner();
                return;
            }

            int nodeViewCount = smartValue.nodeViews.Count;
            for (int i = 0; i < nodeViewCount; i++)
            {
                StateMachineNodeView nodeView = smartValue.nodeViews[i] as StateMachineNodeView;
                if (nodeView == null) continue;

                if (nodeView.stateMachineNodeAsset.stateMachineId != stateMachine.currentState.id) continue;

                if (this.runningNodeView != null) this.runningNodeView.ClearFocus();

                this.runningNodeView = nodeView;
                this.runningNodeView.SetFocus(Color.green);
            }
        }

        private void ClearRunner()
        {
            if (this.debugRunner == null) return;
            this.debugRunner = null;
            if (this.runningNodeView == null) return;
            this.runningNodeView.ClearFocus();
            this.runningNodeView = null;
        }
    }
}