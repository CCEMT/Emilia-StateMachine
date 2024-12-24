using System;
using Sirenix.OdinInspector;

namespace Emilia.StateMachine
{
    [StateMachineTitle("通用/判断串行组件是否结束"), Serializable]
    public class JudgmentSerialEndConditionAsset : UniversalConditionAsset<JudgmentSerialEndCondition>
    {
        [LabelText("ID")]
        public int id;

        [EnumToggleButtons, HideLabel]
        public IsEnd isEnd;

        public enum IsEnd
        {
            [LabelText("判断结束")]
            End,

            [LabelText("判断未结束")]
            NoEnd,
        }
    }

    public class JudgmentSerialEndCondition : UniversalCondition<JudgmentSerialEndConditionAsset>
    {
        public override bool IsSuccess(StateMachine stateMachine)
        {
            string key = RunSerialComponent.RunSerialComponentEndIdentifier + this.asset.id;
            bool isEnd = stateMachine.stateVariablesManage.GetValue<bool>(key);
            bool result = this.asset.isEnd == JudgmentSerialEndConditionAsset.IsEnd.End ? isEnd : ! isEnd;
            return result;
        }
    }
}