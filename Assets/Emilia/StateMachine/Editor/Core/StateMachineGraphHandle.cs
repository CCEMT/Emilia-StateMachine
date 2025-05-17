using System.Collections.Generic;
using System.Linq;
using Emilia.Kit;
using Emilia.Node.Editor;
using UnityEditor;
using UnityEngine;

namespace Emilia.StateMachine.Editor
{
    [EditorHandle(typeof(EditorStateMachineAsset))]
    public class StateMachineGraphHandle : GraphHandle
    {
        private EditorGraphView editorGraphView;
        private EditorStateMachineRunner debugRunner;
        private StateMachineNodeView runningNodeView;

        public override void Initialize(EditorGraphView graphView)
        {
            base.Initialize(graphView);
            editorGraphView = graphView;
            graphView.RegisterCallback<GetStateMachineRunnerEvent>(OnGetStateMachineRunnerEvent);
            graphView.RegisterCallback<SetStateMachineRunnerEvent>(OnSetStateMachineRunnerEvent);
        }

        public override void OnUpdate(EditorGraphView graphView)
        {
            base.OnUpdate(graphView);
            if (EditorApplication.isPlaying == false)
            {
                ClearRunner();
                return;
            }

            if (this.debugRunner == null)
            {
                List<EditorStateMachineRunner> runners = EditorStateMachineRunner.runnerByAssetId.GetValueOrDefault(graphView.graphAsset.id);
                if (runners != null && runners.Count == 1) this.debugRunner = runners.FirstOrDefault();
            }
            else
            {
                if (EditorStateMachineRunner.runnerByAssetId.ContainsKey(graphView.graphAsset.id) == false) ClearRunner();
                else if (EditorStateMachineRunner.runnerByAssetId[graphView.graphAsset.id].Contains(this.debugRunner) == false) ClearRunner();
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

                if (machine.asset.id == editorGraphView.graphAsset.id)
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

            int nodeViewCount = editorGraphView.nodeViews.Count;
            for (int i = 0; i < nodeViewCount; i++)
            {
                StateMachineNodeView nodeView = editorGraphView.nodeViews[i] as StateMachineNodeView;
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