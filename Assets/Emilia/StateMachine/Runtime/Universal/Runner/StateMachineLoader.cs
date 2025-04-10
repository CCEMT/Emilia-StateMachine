using System;
using Object = UnityEngine.Object;

namespace Emilia.StateMachine
{
    public class StateMachineLoader : IStateMachineLoader
    {
        public string runtimeFilePath { get; set; }
        public string editorFilePath { get; set; }
        public Func<string, Object> onLoadAsset { get; set; }
        public Action<string> onReleaseAsset { get; set; }
        public Func<byte[], StateMachineAsset> onLoadStateMachineAsset { get; set; }

        public Object LoadAsset(string path) => onLoadAsset?.Invoke(path);
        public void ReleaseAsset(string path) => onReleaseAsset?.Invoke(path);
        public StateMachineAsset LoadStateMachineAsset(byte[] bytes) => onLoadStateMachineAsset?.Invoke(bytes);
    }
}