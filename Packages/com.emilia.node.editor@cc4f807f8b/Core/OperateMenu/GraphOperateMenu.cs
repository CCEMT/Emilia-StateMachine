﻿using System.Collections.Generic;
using System.Linq;
using Emilia.Kit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Emilia.Node.Editor
{
    public class GraphOperateMenu : BasicGraphViewModule
    {
        public const int SeparatorAt = 1200;

        private OperateMenuHandle handle;

        /// <summary>
        /// 缓存操作菜单信息
        /// </summary>
        public List<OperateMenuActionInfo> actionInfoCache { get; private set; } = new List<OperateMenuActionInfo>();

        public override int order => 1100;

        public override void Initialize(EditorGraphView graphView)
        {
            base.Initialize(graphView);
            actionInfoCache.Clear();
            handle = EditorHandleUtility.CreateHandle<OperateMenuHandle>(this.graphView.graphAsset.GetType());
        }

        public override void AllModuleInitializeSuccess()
        {
            base.AllModuleInitializeSuccess();
            handle?.InitializeCache(this.graphView, actionInfoCache);
        }

        /// <summary>
        /// 构建菜单
        /// </summary>
        public void BuildMenu(OperateMenuContext menuContext)
        {
            if (handle == null)
            {
                Debug.LogError("未找到操作菜单处理器，请创建并继承OperateMenuHandle<>");
                return;
            }

            List<OperateMenuItem> graphMenuItems = new List<OperateMenuItem>();
            handle.CollectMenuItems(this.graphView, graphMenuItems, menuContext);

            var sortedItems = graphMenuItems
                .GroupBy(x => string.IsNullOrEmpty(x.category) ? x.menuName : x.category)
                .OrderBy(x => x.Min(y => y.priority))
                .SelectMany(x => x.OrderBy(z => z.priority));

            int lastPriority = int.MinValue;
            string lastCategory = string.Empty;

            foreach (OperateMenuItem item in sortedItems)
            {
                if (item.state == OperateMenuActionValidity.NotApplicable) continue;

                int priority = item.priority;
                if (lastPriority != int.MinValue && priority / SeparatorAt > lastPriority / SeparatorAt)
                {
                    string path = string.Empty;
                    if (lastCategory == item.category) path = item.category;
                    menuContext.evt.menu.AppendSeparator(path);
                }

                lastPriority = priority;
                lastCategory = item.category;

                string entryName = item.category + item.menuName;

                DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal;
                if (item.state == OperateMenuActionValidity.Invalid) status = DropdownMenuAction.Status.Disabled;
                if (item.isOn) status |= DropdownMenuAction.Status.Checked;

                menuContext.evt.menu.AppendAction(entryName, _ => item.onAction?.Invoke(), status);
            }
        }

        public override void Dispose()
        {
            if (this.graphView == null) return;

            actionInfoCache.Clear();
            this.handle = null;
            base.Dispose();
        }
    }
}