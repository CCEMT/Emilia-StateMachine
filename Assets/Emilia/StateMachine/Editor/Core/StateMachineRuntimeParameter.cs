using System;
using System.Collections.Generic;
using Emilia.Node.Universal.Editor;
using Emilia.Variables;
using Sirenix.OdinInspector;

namespace Emilia.StateMachine.Editor
{
    [Serializable]
    public class StateMachineRuntimeParameter
    {
        private EditorStateMachineRunner runner;

        private Dictionary<string, Variable> _runtimeUserVariables = new Dictionary<string, Variable>();

        [LabelText("参数"), ShowInInspector]
        public Dictionary<string, Variable> runtimeUserVariables
        {
            get
            {
                _runtimeUserVariables.Clear();
                if (this.runner == null) return this._runtimeUserVariables;

                foreach (var variablePair in this.runner.stateMachine.userVariablesManage.variableMap)
                {
                    EditorParameter editorParameter = this.runner.editorStateMachineAsset.editorParametersManage.parameters.Find((x) => x.key == variablePair.Key);
                    if (editorParameter == null) continue;
                    _runtimeUserVariables[editorParameter.description] = variablePair.Value;
                }

                return this._runtimeUserVariables;
            }

            set { }
        }

        [LabelText("（内置）状态机参数"), ShowInInspector]
        public Dictionary<string, Variable> runtimeStateMachineVariables
        {
            get
            {
                if (this.runner == null) return new Dictionary<string, Variable>();
                return runner.stateMachine.stateMachineVariablesManage.variableMap as Dictionary<string, Variable>;
            }
            set { }
        }

        [LabelText("（内置）状态参数"), ShowInInspector]
        public Dictionary<string, Variable> runtimeStateVariables
        {
            get
            {
                if (this.runner == null) return new Dictionary<string, Variable>();
                return runner.stateMachine.stateVariablesManage.variableMap as Dictionary<string, Variable>;
            }
            set { }
        }

        public StateMachineRuntimeParameter(EditorStateMachineRunner runner)
        {
            this.runner = runner;
        }
    }
}