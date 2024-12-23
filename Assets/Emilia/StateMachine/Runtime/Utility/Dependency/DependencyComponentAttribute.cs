using System;

namespace Emilia.StateMachine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyComponentAttribute : Attribute
    {
        public Type componentType;

        public bool isSingleton;

        public DependencyComponentAttribute(Type type, bool isSingleton = true)
        {
            this.componentType = type;
            this.isSingleton = isSingleton;
        }
    }
}