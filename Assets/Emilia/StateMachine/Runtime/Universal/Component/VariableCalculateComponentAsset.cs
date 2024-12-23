using System;
using Emilia.BehaviorTree.Attributes;
using Emilia.Node.Attributes;
using Emilia.Variables;
using Sirenix.OdinInspector;

namespace Emilia.StateMachine
{
    [StateMachineTitle("通用/参数运算"), Serializable]
    public class VariableCalculateComponentAsset : UniversalComponentAsset<VariableCalculateComponent>
    {
        [LabelText("参数"), VariableKeySelector]
        public string leftKey;

        [LabelText("运算操作符")]
        public VariableCalculateOperator calculateOperator;

        [HideLabel, HorizontalGroup(20)]
        public bool useDefine = true;

        [HorizontalGroup, VariableTypeFilter(nameof(leftKey)), ShowIf(nameof(useDefine))]
        public Variable rightDefineValue = new VariableObject();

        [LabelText("参数"), VariableKeySelector, HorizontalGroup, HideIf(nameof(useDefine))]
        public string rightKey;
    }

    public class VariableCalculateComponent : UniversalComponent<VariableCalculateComponentAsset>
    {
        public override void Enter(StateMachine stateMachine)
        {
            base.Enter(stateMachine);
            Variable leftValue = stateMachine.userVariablesManage.GetThisValue(this.asset.leftKey);
            Variable rightValue = this.asset.useDefine ? this.asset.rightDefineValue : stateMachine.userVariablesManage.GetThisValue(this.asset.rightKey);

            if (leftValue == null || rightValue == null) return;

            VariableUtility.Calculate(leftValue, rightValue, this.asset.calculateOperator);
        }
    }
}