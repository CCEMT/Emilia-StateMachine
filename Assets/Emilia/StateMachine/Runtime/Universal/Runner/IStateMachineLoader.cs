using UnityEngine;

namespace Emilia.StateMachine
{
    public interface IStateMachineLoader
    {
        /// <summary>
        /// 运行时文件路径
        /// </summary>
        string runtimeFilePath { get; }

        /// <summary>
        /// 编辑器文件路径
        /// </summary>
        string editorFilePath { get; }

        /// <summary>
        /// 加载资源
        /// </summary>
        Object LoadAsset(string path);
        
        
        /// <summary>
        /// 释放资源
        /// </summary>
        void ReleaseAsset(string path);

        /// <summary>
        /// 加载状态机资源（反序列化）
        /// </summary>
        StateMachineAsset LoadStateMachineAsset(byte[] bytes);
    }
}