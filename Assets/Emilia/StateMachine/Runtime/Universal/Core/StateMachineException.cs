using System;

namespace Emilia.StateMachine
{
    public class StateMachineException : Exception
    {
        public StateMachineException() : base() { }
        public StateMachineException(string message) : base(message) { }
        public StateMachineException(string message, Exception innerException) : base(message, innerException) { }

        public override string ToString()
        {
            return $"{this.Message}\n{this.StackTrace}";
        }
    }
}