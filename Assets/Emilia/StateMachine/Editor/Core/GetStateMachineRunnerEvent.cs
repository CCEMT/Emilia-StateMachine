using UnityEngine.UIElements;

namespace Emilia.StateMachine.Editor
{
    public class GetStateMachineRunnerEvent : EventBase<GetStateMachineRunnerEvent>
    {
        public EditorStateMachineRunner runner;

        protected override void Init()
        {
            base.Init();
            this.runner = null;
        }
    }
}