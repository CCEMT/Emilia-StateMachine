using System;

namespace Emilia.StateMachine
{
    [Serializable, StateMachineHide]
    public class ExitComponentAsset : UniversalComponentAsset<ExitComponent> { }

    public class ExitComponent : UniversalComponent<ExitComponentAsset>
    {
        public override void Enter(StateMachine stateMachine)
        {
            stateMachine.Dispose();
        }
    }
}