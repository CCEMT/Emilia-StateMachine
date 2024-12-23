using System;
using System.Collections.Generic;
using System.Linq;
using Emilia.DataBuildPipeline.Editor;
using Sirenix.Utilities;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(StateMachineBuildPipeline.PipelineName), BuildSequence(2000)]
    public class StateMachineLinkPostproces : IDataPostproces
    {
        public void Postprocess(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;
            GenerateLink(container.stateMachineAsset);
            onFinished.Invoke();
        }

        private void GenerateLink(StateMachineAsset stateMachineAsset)
        {
            int amount = stateMachineAsset.stateAssets.Count;
            for (int i = 0; i < amount; i++)
            {
                StateAsset stateAsset = stateMachineAsset.stateAssets[i];
                int componentCount = stateAsset.componentAssets.Count;
                for (int j = componentCount - 1; j >= 0; j--)
                {
                    IStateComponentAsset componentAsset = stateAsset.componentAssets[j];
                    LinkComponent(stateMachineAsset, componentAsset);
                }
            }
        }

        private void LinkComponent(StateMachineAsset stateMachineAsset, IStateComponentAsset componentAsset)
        {
            Type componentAssetType = componentAsset.GetType();
            LinkAttribute disposeAttribute = componentAssetType.GetCustomAttribute<LinkAttribute>(true);
            if (disposeAttribute == null) return;

            Type disposeType = disposeAttribute.linkComponentType;

            IStateComponentAsset linkComponent = GenerateLinkComponent(disposeType);
            ILinkState disposeState = componentAsset as ILinkState;
            AddComponent(stateMachineAsset, disposeState.stateSelector.stateId, linkComponent);
            if (linkComponent == default) return;

            string key = componentAssetType.FullName + linkComponent.GetHashCode();
            ILinkArg disposeArg = componentAsset as ILinkArg;
            disposeArg.keySelector.key = key;

            ILinkArg disposeArgComponentDisposeArg = linkComponent as ILinkArg;
            disposeArgComponentDisposeArg.keySelector.key = key;
        }

        private IStateComponentAsset GenerateLinkComponent(Type disposeComponentType)
        {
            object instance = Activator.CreateInstance(disposeComponentType);
            if (instance == default) return default;
            IStateComponentAsset disposeComponent = instance as IStateComponentAsset;
            return disposeComponent;
        }

        private void AddComponent(StateMachineAsset stateMachineAsset, int stateId, IStateComponentAsset componentAsset)
        {
            StateAsset stateAsset = stateMachineAsset.stateAssets.FirstOrDefault((state) => state.id == stateId);
            if (stateAsset == default) return;
            List<IStateComponentAsset> stateComponentAssets = stateAsset.componentAssets as List<IStateComponentAsset>;
            stateComponentAssets.Add(componentAsset);
        }
    }
}