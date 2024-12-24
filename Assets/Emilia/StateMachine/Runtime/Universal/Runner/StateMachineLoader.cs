using System;
using Object = UnityEngine.Object;

namespace Emilia.StateMachine
{
    public class StateMachineLoader : IStateMachineLoader
    {
        public string runtimeFilePath { get; set; }
        public string editorFilePath { get; set; }
        public Func<string, Object> onLoadAsset { get; set; }
        public Func<byte[], StateMachineAsset> onLoadStateMachineAsset { get; set; }

        public Object LoadAsset(string path)
        {
            return onLoadAsset?.Invoke(path);
        }

        public StateMachineAsset LoadStateMachineAsset(byte[] bytes)
        {
            return onLoadStateMachineAsset?.Invoke(bytes);
        }
    }
}