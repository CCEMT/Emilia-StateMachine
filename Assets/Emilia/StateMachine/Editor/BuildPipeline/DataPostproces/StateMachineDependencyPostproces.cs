using System;
using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using Sirenix.Utilities;

namespace Emilia.StateMachine.Editor
{
    [BuildPipeline(typeof(StateMachineBuildArgs)), BuildSequence(1000)]
    public class StateMachineDependencyPostproces : IDataPostprocess
    {
        public void Postprocess(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            StateMachineBuildContainer container = buildContainer as StateMachineBuildContainer;
            GenerateDependencyComponent(container.stateMachineAsset);
            onFinished.Invoke();
        }

        private void GenerateDependencyComponent(StateMachineAsset stateMachineAsset)
        {
            Dictionary<StateAsset, Dictionary<Type, List<IStateComponentAsset>>> stateComponentTypes = new Dictionary<StateAsset, Dictionary<Type, List<IStateComponentAsset>>>();

            int stateCount = stateMachineAsset.stateAssets.Count;
            for (int i = 0; i < stateCount; i++)
            {
                StateAsset stateAsset = stateMachineAsset.stateAssets[i];
                int transitionCount = stateAsset.transitionAssets.Count;
                for (int j = 0; j < transitionCount; j++)
                {
                    TransitionAsset transitionAsset = stateAsset.transitionAssets[j];
                    int conditionCount = transitionAsset.conditionAssets.Count;
                    for (int k = 0; k < conditionCount; k++)
                    {
                        IConditionAsset conditionAsset = transitionAsset.conditionAssets[k];
                        AddComponent(conditionAsset, stateAsset);
                    }
                }
            }

            foreach (var stateComponentType in stateComponentTypes)
            {
                StateAsset stateAsset = stateComponentType.Key;
                Dictionary<Type, List<IStateComponentAsset>> componentTypes = stateComponentType.Value;
                foreach (var componentType in componentTypes)
                {
                    List<IStateComponentAsset> componentAssets = componentType.Value;

                    List<IStateComponentAsset> stateComponentAssets = stateAsset.componentAssets as List<IStateComponentAsset>;
                    stateComponentAssets.AddRange(componentAssets);
                }
            }

            void AddComponent(IConditionAsset conditionAsset, StateAsset stateAsset)
            {
                Type conditionType = conditionAsset.GetType();
                DependencyComponentAttribute dependencyComponentAttribute = conditionType.GetCustomAttribute<DependencyComponentAttribute>();
                if (dependencyComponentAttribute == null) return;
                Type componentType = dependencyComponentAttribute.componentType;

                if (stateComponentTypes.TryGetValue(stateAsset, out Dictionary<Type, List<IStateComponentAsset>> componentTypes) == false)
                {
                    componentTypes = new Dictionary<Type, List<IStateComponentAsset>>();
                    stateComponentTypes.Add(stateAsset, componentTypes);
                }

                if (componentTypes.TryGetValue(componentType, out List<IStateComponentAsset> componentAssets) == false)
                {
                    componentAssets = new List<IStateComponentAsset>();
                    componentTypes.Add(componentType, componentAssets);
                }

                bool isSingleton = dependencyComponentAttribute.isSingleton;
                bool isExist = componentAssets.Count > 0;

                if (isSingleton && isExist) return;

                IStateComponentAsset componentAsset = Activator.CreateInstance(componentType) as IStateComponentAsset;
                if (componentAsset == null) return;
                componentAsset.id = Guid.NewGuid().ToString();
                IDependencyComponentAsset dependencyComponentAsset = componentAsset as IDependencyComponentAsset;
                if (dependencyComponentAsset != null) dependencyComponentAsset.dependencyAsset = conditionAsset;
                componentAssets.Add(componentAsset);
            }
        }
    }
}