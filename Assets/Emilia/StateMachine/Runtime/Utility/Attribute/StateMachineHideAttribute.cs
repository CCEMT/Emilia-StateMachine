using System;

namespace Emilia.StateMachine
{
    /// <summary>
    /// 隐藏特性，用于隐藏组件和条件上在添加菜单中的显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StateMachineHideAttribute : Attribute { }
}