using System;
using Emilia.BehaviorTree.Attributes;
using Emilia.Node.Attributes;
using Emilia.Variables;
using Sirenix.OdinInspector;

namespace Emilia.StateMachine
{
    [StateMachineTitle("通用/参数比较"), Serializable]
    public class VariableCompareConditionAsset : UniversalConditionAsset<VariableCompareCondition>
    {
        [LabelText("比较值"), VariableKeySelector]
        public string leftKey;

        [LabelText("比较操作符")]
        public VariableCompareOperator compareOperator;

        [HideLabel, HorizontalGroup(20)]
        public bool useDefine = true;

        [HorizontalGroup, VariableTypeFilter(nameof(leftKey)), ShowIf(nameof(useDefine))]
        public Variable rightDefineValue = new VariableObject();

        [LabelText("比较值"), VariableKeySelector, HorizontalGroup, HideIf(nameof(useDefine))]
        public string rightKey;
    }

    public class VariableCompareCondition : UniversalCondition<VariableCompareConditionAsset>
    {
        public override bool IsSuccess(StateMachine stateMachine)
        {
            Variable leftValue = stateMachine.userVariablesManage.GetThisValue(this.asset.leftKey);
            Variable rightValue = this.asset.useDefine ? this.asset.rightDefineValue : stateMachine.userVariablesManage.GetThisValue(this.asset.rightKey);
            if (leftValue == null || rightValue == null) return false;
            return VariableUtility.Compare(leftValue, rightValue, this.asset.compareOperator);
        }
    }
}