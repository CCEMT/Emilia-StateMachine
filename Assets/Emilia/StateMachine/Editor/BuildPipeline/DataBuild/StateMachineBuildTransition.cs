using System;
using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Node.Editor;
using Sirenix.Serialization;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(typeof(StateMachineBuildArgs)), BuildSequence(3000)]
    public class StateMachineBuildTransition : IDataBuild
    {
        public void Build(IBuildContainer buildContainer, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;
            container.stateAssets = BuildTransition(container.editorStateMachineAsset, container.stateAssets);
            onFinished.Invoke();
        }

        private List<StateAsset> BuildTransition(EditorStateMachineAsset editorStateMachineAsset, List<StateAsset> stateAssets)
        {
            List<StateMachineEdgeAsset> anyEdges = new List<StateMachineEdgeAsset>();

            int edgeAmount = editorStateMachineAsset.edges.Count;
            for (int i = 0; i < edgeAmount; i++)
            {
                EditorEdgeAsset edgeAsset = editorStateMachineAsset.edges[i];

                bool isAny = false;

                EditorNodeAsset inputNode = editorStateMachineAsset.nodeMap.GetValueOrDefault(edgeAsset.inputNodeId);
                if (inputNode is AnyNodeAsset) isAny = true;

                EditorNodeAsset outputNode = editorStateMachineAsset.nodeMap.GetValueOrDefault(edgeAsset.outputNodeId);
                if (outputNode is AnyNodeAsset) isAny = true;

                if (isAny == false) continue;

                StateMachineEdgeAsset stateMachineEdgeAsset = edgeAsset as StateMachineEdgeAsset;
                anyEdges.Add(stateMachineEdgeAsset);
            }

            int anyAmount = anyEdges.Count;
            for (int i = 0; i < anyAmount; i++)
            {
                StateMachineEdgeAsset edgeAsset = anyEdges[i];
                StateMachineNodeAsset inputNode = editorStateMachineAsset.nodeMap.GetValueOrDefault(edgeAsset.inputNodeId) as StateMachineNodeAsset;
                StateMachineNodeAsset outputNode = editorStateMachineAsset.nodeMap.GetValueOrDefault(edgeAsset.outputNodeId) as StateMachineNodeAsset;

                int inputAmount = edgeAsset.inputCondition.Count;
                for (int j = 0; j < inputAmount; j++)
                {
                    StateMachineConditionGroup stateMachineConditionGroup = edgeAsset.inputCondition[j];
                    AddAnyTransition(inputNode.stateMachineId, stateMachineConditionGroup);
                }

                int outputAmount = edgeAsset.outputCondition.Count;
                for (int j = 0; j < outputAmount; j++)
                {
                    StateMachineConditionGroup stateMachineConditionGroup = edgeAsset.outputCondition[j];
                    AddAnyTransition(outputNode.stateMachineId, stateMachineConditionGroup);
                }
            }

            for (int i = 0; i < edgeAmount; i++)
            {
                EditorEdgeAsset edgeAsset = editorStateMachineAsset.edges[i];
                StateMachineEdgeAsset stateMachineEdgeAsset = edgeAsset as StateMachineEdgeAsset;

                StateMachineNodeAsset inputNode = editorStateMachineAsset.nodeMap.GetValueOrDefault(edgeAsset.inputNodeId) as StateMachineNodeAsset;
                StateMachineNodeAsset outputNode = editorStateMachineAsset.nodeMap.GetValueOrDefault(edgeAsset.outputNodeId) as StateMachineNodeAsset;

                int inputAmount = stateMachineEdgeAsset.inputCondition.Count;
                for (int j = 0; j < inputAmount; j++)
                {
                    StateMachineConditionGroup stateMachineConditionGroup = stateMachineEdgeAsset.inputCondition[j];
                    AddTransition(outputNode, inputNode.stateMachineId, stateMachineConditionGroup);
                }

                int outputAmount = stateMachineEdgeAsset.outputCondition.Count;
                for (int j = 0; j < outputAmount; j++)
                {
                    StateMachineConditionGroup stateMachineConditionGroup = stateMachineEdgeAsset.outputCondition[j];
                    AddTransition(inputNode, outputNode.stateMachineId, stateMachineConditionGroup);
                }
            }

            void AddAnyTransition(int nextId, StateMachineConditionGroup stateMachineConditionGroup)
            {
                int nodeAmount = editorStateMachineAsset.nodes.Count;
                for (int i = 0; i < nodeAmount; i++)
                {
                    EditorNodeAsset nodeAsset = editorStateMachineAsset.nodes[i];
                    StateMachineNodeAsset stateMachineNodeAsset = nodeAsset as StateMachineNodeAsset;
                    if (stateMachineNodeAsset == null) continue;

                    StateAsset stateAsset = stateAssets.Find((x) => x.id == stateMachineNodeAsset.stateMachineId);
                    if (stateAsset == default) continue;

                    if (stateMachineNodeAsset.stateMachineId <= 0) continue;
                    TransitionAsset transitionAsset = ToTransitionAsset(nextId, stateMachineConditionGroup);

                    List<TransitionAsset> transitionAssets = stateAsset.transitionAssets as List<TransitionAsset>;
                    transitionAssets.Add(transitionAsset);
                }
            }

            void AddTransition(StateMachineNodeAsset stateMachineNodeAsset, int nextId, StateMachineConditionGroup stateMachineConditionGroup)
            {
                StateAsset stateAsset = stateAssets.Find((x) => x.id == stateMachineNodeAsset.stateMachineId);
                if (stateAsset == default) return;

                TransitionAsset transitionAsset = ToTransitionAsset(nextId, stateMachineConditionGroup);

                List<TransitionAsset> transitionAssets = stateAsset.transitionAssets as List<TransitionAsset>;
                transitionAssets.Add(transitionAsset);
            }

            TransitionAsset ToTransitionAsset(int nextId, StateMachineConditionGroup stateMachineConditionGroup)
            {
                List<IConditionAsset> conditionAssets = new List<IConditionAsset>();

                int conditionAmount = stateMachineConditionGroup.conditionAssets.Count;
                for (int i = 0; i < conditionAmount; i++)
                {
                    EditorConditionAsset editorConditionAsset = stateMachineConditionGroup.conditionAssets[i];
                    IConditionAsset clone = SerializationUtility.CreateCopy(editorConditionAsset.conditionAsset) as IConditionAsset;
                    conditionAssets.Add(clone);
                }

                return new TransitionAsset(nextId, conditionAssets);
            }

            return stateAssets;
        }
    }
}