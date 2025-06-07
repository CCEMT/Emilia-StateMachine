using Emilia.StateMachine;
using Spine.Unity;

namespace Emilia.SpineAnimator
{
    public interface ISpineAnimatorComponentAsset { }

    public abstract class SpineAnimatorComponentAsset<T> : StateComponentAsset<T>, ISpineAnimatorComponentAsset where T : class, IStateComponent, new() { }

    public abstract class SpineAnimatorComponent<T> : StateComponent<T> where T : class, IStateComponentAsset
    {
        protected SkeletonAnimation skeletonAnimation;

        protected override void OnInit()
        {
            base.OnInit();
            skeletonAnimation = this.owner as SkeletonAnimation;
        }
    }
}