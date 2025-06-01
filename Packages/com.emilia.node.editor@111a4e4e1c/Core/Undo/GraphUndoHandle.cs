﻿using Emilia.Kit;

namespace Emilia.Node.Editor
{
    [EditorHandleGenerate]
    public class GraphUndoHandle
    {
        /// <summary>
        /// 在撤销操作之前调用
        /// </summary>
        public virtual void OnUndoBefore(EditorGraphView graphView, bool isSilent) { }

        /// <summary>
        /// 在撤销操作之后调用
        /// </summary>
        public virtual void OnUndoAfter(EditorGraphView graphView, bool isSilent) { }
    }
}