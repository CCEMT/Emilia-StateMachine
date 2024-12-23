using System;

namespace Emilia.StateMachine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LinkAttribute : Attribute
    {
        public Type linkComponentType;

        public LinkAttribute(Type type)
        {
            this.linkComponentType = type;
            if (! typeof(IStateComponentAsset).IsAssignableFrom(type)) throw new Exception($"GuideDisposeAttribute: {type} 类型错误");
        }
    }
}