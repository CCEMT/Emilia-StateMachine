#if !UNITY_EDITOR
using Emilia.Reference;
#endif

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Emilia.StateMachine
{
    public static class StateMachineRunnerUtility
    {
        private static int maxId = 0;
        private static Queue<int> idPool = new Queue<int>();

        public static int GetId()
        {
            if (idPool.Count == 0) return ++maxId;
            return idPool.Dequeue();
        }

        public static void RecycleId(int id)
        {
            idPool.Enqueue(id);
        }

        public static IStateMachineRunner CreateRunner()
        {
#if UNITY_EDITOR
            Type type = Assembly.Load("Emilia.StateMachine.Editor").GetType("Emilia.StateMachine.Editor.EditorStateMachineRunner");
            return Activator.CreateInstance(type) as IStateMachineRunner;
#else
            return ReferencePool.Acquire<RuntimeStateMachineRunner>();
#endif
        }
    }
}