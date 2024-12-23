using System;

namespace Emilia.StateMachine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StateMachineTitleAttribute : Attribute
    {
        public string title;

        public StateMachineTitleAttribute(string title)
        {
            this.title = title;
        }
    }
}