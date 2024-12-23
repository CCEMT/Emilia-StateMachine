using System;

namespace Emilia.StateMachine.Editor
{
    public class StateMachineTitleMenuAttribute : Attribute
    {
        public string displayName;

        public StateMachineTitleMenuAttribute(string displayName)
        {
            this.displayName = displayName;
        }
    }
}