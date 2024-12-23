using System;
using Emilia.Reference;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Emilia.StateMachine
{
    [Serializable, StateMachineHide]
    public class SubStateMachineComponentAsset : UniversalComponentAsset<SubStateMachineComponent>, ISuccessComponentAsset
    {
        [SerializeField, PropertyOrder]
        public StateMachineAsset stateMachineAsset;
    }

    public class SubStateMachineComponent : UniversalComponent<SubStateMachineComponentAsset>, ISuccessComponent
    {
        private StateMachine subStateMachine;

        public override void Enter(StateMachine stateMachine)
        {
            subStateMachine = ReferencePool.Acquire<StateMachine>();
            subStateMachine.Init(stateMachine.uid, this.asset.stateMachineAsset, this.owner, stateMachine);
            this.subStateMachine.Start();
            stateMachine.children.Add(subStateMachine);
        }

        public override void Update(StateMachine stateMachine)
        {
            subStateMachine?.Update();
        }

        public bool IsSuccess(StateMachine stateMachine)
        {
            if (this.subStateMachine != null) return this.subStateMachine.runState == StateMachine.RunState.Close;
            return false;
        }

        public override void Exit(StateMachine stateMachine)
        {
            if (subStateMachine == null) return;
            stateMachine.children.Remove(subStateMachine);
            subStateMachine.Dispose();
            subStateMachine = null;
        }
    }
}