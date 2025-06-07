using System;

namespace Emilia.SpineAnimator
{
    public class StateMachineKeySelectorAttribute : Attribute
    {
        public string filePath;

        public StateMachineKeySelectorAttribute(string filePath)
        {
            this.filePath = filePath;
        }
    }
}