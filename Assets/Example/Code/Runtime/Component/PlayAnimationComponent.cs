using System;
using Emilia.Node.Attributes;
using Emilia.StateMachine;
using Sirenix.OdinInspector;
using Spine;

namespace Emilia.SpineAnimator
{
    [StateMachineTitle("播放动画"), Serializable]
    public class PlayAnimationComponentAsset : SpineAnimatorComponentAsset<PlayAnimationComponent>
    {
        [LabelText("动画名称"), SpineAnimationNameSelector]
        public string animationName;

        [LabelText("轨道")]
        public int trackIndex;

        [LabelText("是否循环")]
        public bool isLoop;

        [LabelText("状态"), VariableKeySelector]
        public string stateKey;
    }

    public class PlayAnimationComponent : SpineAnimatorComponent<PlayAnimationComponentAsset>
    {
        private int state;
        private TrackEntry trackEntry;

        public override void Enter(StateMachine.StateMachine stateMachine)
        {
            base.Enter(stateMachine);

            this.state = 0;
            if (string.IsNullOrEmpty(this.asset.stateKey) == false) stateMachine.userVariablesManage.SetValue(this.asset.stateKey, state);
            trackEntry = this.skeletonAnimation.AnimationState.SetAnimation(this.asset.trackIndex, this.asset.animationName, this.asset.isLoop);
        }

        public override void Update(StateMachine.StateMachine stateMachine)
        {
            base.Update(stateMachine);

            if (trackEntry == null) return;
            this.state = trackEntry.IsComplete ? 1 : 0;
            if (string.IsNullOrEmpty(this.asset.stateKey) == false) stateMachine.userVariablesManage.SetValue(this.asset.stateKey, state);
        }

        public override void Exit(StateMachine.StateMachine stateMachine)
        {
            base.Exit(stateMachine);
            this.state = 0;
            if (string.IsNullOrEmpty(this.asset.stateKey) == false) stateMachine.userVariablesManage.SetValue(this.asset.stateKey, state);
            trackEntry = null;
        }

        protected override void OnDispose(StateMachine.StateMachine stateMachine)
        {
            base.OnDispose(stateMachine);
            this.state = 0;
            if (string.IsNullOrEmpty(this.asset.stateKey) == false) stateMachine.userVariablesManage.SetValue(this.asset.stateKey, state);
            trackEntry = null;
        }
    }
}