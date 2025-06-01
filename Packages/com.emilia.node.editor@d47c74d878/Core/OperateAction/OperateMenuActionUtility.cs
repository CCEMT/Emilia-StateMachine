﻿using System;
using System.Collections.Generic;
using System.Linq;
using Emilia.Kit.Editor;
using Sirenix.Utilities;
using UnityEditor;

namespace Emilia.Node.Editor
{
    public static class OperateMenuActionUtility
    {
        private static List<OperateMenuActionInfo> _actions;
        private static Dictionary<Type, OperateMenuActionInfo> _actionMap;

        public static IReadOnlyList<OperateMenuActionInfo> actions => _actions;
        public static IReadOnlyDictionary<Type, OperateMenuActionInfo> actionMap => _actionMap;

        static OperateMenuActionUtility()
        {
            _actions = new List<OperateMenuActionInfo>();
            _actionMap = new Dictionary<Type, OperateMenuActionInfo>();

            IList<Type> types = TypeCache.GetTypesDerivedFrom<IOperateMenuAction>();
            for (var i = 0; i < types.Count; i++)
            {
                Type type = types[i];
                if (type.IsAbstract || type.IsInterface) continue;
                ActionAttribute actionAttribute = type.GetCustomAttribute<ActionAttribute>();
                if (actionAttribute == null) continue;

                IOperateMenuAction action = (IOperateMenuAction) ReflectUtility.CreateInstance(type);

                OperateMenuActionInfo actionInfo = new OperateMenuActionInfo();
                actionInfo.action = action;
                actionInfo.name = actionAttribute.name;
                actionInfo.category = actionAttribute.category;
                actionInfo.priority = actionAttribute.priority;
                actionInfo.tags = actionAttribute.tags;

                _actions.Add(actionInfo);
                _actionMap.Add(type, actionInfo);
            }
        }

        /// <summary>
        /// 获取操作菜单行为信息
        /// </summary>
        public static OperateMenuActionInfo GetAction<T>()
        {
            return _actionMap[typeof(T)];
        }

        /// <summary>
        /// 根据Tags获取操作菜单行为信息列表
        /// </summary>
        public static List<OperateMenuActionInfo> GetAction(params string[] tags)
        {
            List<OperateMenuActionInfo> result = new List<OperateMenuActionInfo>();
            for (var i = 0; i < _actions.Count; i++)
            {
                OperateMenuActionInfo actionInfo = _actions[i];
                if (actionInfo.tags.Intersect(tags).Any()) result.Add(actionInfo);
            }

            return result;
        }
    }
}