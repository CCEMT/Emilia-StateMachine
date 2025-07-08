﻿using System.Collections.Generic;

namespace Emilia.Node.Editor
{
    public interface ICreateNodeCollect
    {
        /// <summary>
        /// 收集节点信息
        /// </summary>
        /// <param name="allNodeInfos">所有节点</param>
        /// <returns>可创建的节点</returns>
        List<CreateNodeInfo> Collect(List<MenuNodeInfo> allNodeInfos);
    }
}