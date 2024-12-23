using UnityEngine.UIElements;

namespace Emilia.StateMachine.Editor
{
    public class SetStateMachineRunnerEvent : EventBase<SetStateMachineRunnerEvent>
    {
        public EditorStateMachineRunner runner;

        public static SetStateMachineRunnerEvent Create(EditorStateMachineRunner runner)
        {
            SetStateMachineRunnerEvent e = GetPooled();
            e.runner = runner;
            return e;
        }

        protected override void Init()
        {
            base.Init();
            this.runner = null;
        }
    }
}