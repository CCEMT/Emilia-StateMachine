using Emilia.StateMachine;

namespace Emilia.SpineAnimator
{
    public interface ISpineAnimatorConditionAsset { }

    public abstract class SpineAnimatorConditionAsset<T> : ConditionAsset<T>, ISpineAnimatorConditionAsset where T : class, ICondition, new() { }

    public abstract class SpineAnimatorCondition<T> : Condition<T> where T : class, IConditionAsset { }
}