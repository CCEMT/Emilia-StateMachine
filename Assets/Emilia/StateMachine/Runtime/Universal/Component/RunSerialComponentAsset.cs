using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Emilia.StateMachine
{
    [StateMachineTitle("通用/串行组件"), Serializable]
    public class RunSerialComponentAsset : UniversalComponentAsset<RunSerialComponent>, ISuccessComponentAsset, IDescription
    {
        [LabelText("ID")]
        public int serialID;

        [LabelText("串行组件列表"), HideReferenceObjectPicker, ListDrawerSettings(HideAddButton = true)]
        public List<ISuccessComponentAsset> components = new List<ISuccessComponentAsset>();

#if UNITY_EDITOR
        public string text
        {
            get
            {
                string result = "";
                int amount = components.Count;
                for (int i = 0; i < amount; i++)
                {
                    IDescription description = components[i] as IDescription;
                    if (description == null) continue;
                    result += $"{description.text}\n";
                }
                return result;
            }
        }
#endif
    }

    public class RunSerialComponent : UniversalComponent<RunSerialComponentAsset>, ISuccessComponent
    {
        public const string RunSerialComponentEndIdentifier = "RunSerialComponentEndIdentifier";

        private int runIndex;
        private IStateComponent runComponent;
        private List<IStateComponent> components = new List<IStateComponent>();
        private List<ISuccessComponent> serialComponents = new List<ISuccessComponent>();

        protected override void OnInit()
        {
            int amount = this.asset.components.Count;
            for (int i = 0; i < amount; i++)
            {
                ISuccessComponentAsset componentAsset = this.asset.components[i];
                IStateComponentAsset stateComponent = componentAsset as IStateComponentAsset;
                ISuccessComponent serialComponent = stateComponent.CreateComponent() as ISuccessComponent;
                IStateComponent component = serialComponent as IStateComponent;
                component.Init(stateComponent, stateMachine);
                components.Add(component);
                serialComponents.Add(serialComponent);
            }
        }

        public override void Enter(StateMachine stateMachine)
        {
            base.Enter(stateMachine);
            runIndex = 0;
            string key = RunSerialComponentEndIdentifier + this.asset.serialID;
            stateMachine.stateVariablesManage.SetThisValue(key, false);

            if (this.runIndex < components.Count)
            {
                IStateComponent component = components[runIndex];
                component.Enter(stateMachine);
                this.runComponent = component;
            }
        }

        public override void Update(StateMachine stateMachine)
        {
            if (this.runIndex < 0) return;
            if (this.runIndex >= components.Count)
            {
                string key = RunSerialComponentEndIdentifier + this.asset.serialID;
                stateMachine.stateVariablesManage.SetThisValue(key, true);
                runIndex = -1;
                return;
            }
            IStateComponent component = components[runIndex];
            ISuccessComponent serialComponent = serialComponents[runIndex];
            component.Update(stateMachine);
            if (serialComponent.IsSuccess(stateMachine) == false) return;
            component.Exit(stateMachine);
            this.runComponent = null;

            runIndex++;

            if (this.runIndex < components.Count)
            {
                component = components[runIndex];
                component.Enter(stateMachine);
                this.runComponent = component;
            }
        }

        public override void Exit(StateMachine stateMachine)
        {
            base.Exit(stateMachine);
            if (this.runComponent != null) this.runComponent.Exit(stateMachine);
        }

        public override void Dispose(StateMachine stateMachine)
        {
            base.Dispose(stateMachine);
            runIndex = -1;
            int amount = components.Count;
            for (int i = 0; i < amount; i++)
            {
                IStateComponent component = components[i];
                component.Dispose(stateMachine);
            }
        }

        public bool IsSuccess(StateMachine stateMachine)
        {
            string key = RunSerialComponentEndIdentifier + this.asset.serialID;
            return stateMachine.stateVariablesManage.GetValue<bool>(key);
        }
    }
}