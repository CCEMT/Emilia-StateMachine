using System;
using Emilia.StateMachine;
using Emilia.StateMachine.Editor;
using Spine.Unity;
using UnityEngine;

namespace Emilia.SpineAnimator.Editor
{
    [CreateAssetMenu(menuName = "Emilia/SpineAnimator/EditorSpineAnimatorAsset", fileName = "EditorSpineAnimatorAsset")]
    public class EditorSpineAnimatorAsset : EditorStateMachineAsset
    {
        public SkeletonDataAsset skeletonDataAsset;
        
        public override string outputFilePath => "Assets/Emilia/SpineAnimator/Resource/Config";
        public override Type[] stateComponentSubTypes => new[] {typeof(ISpineAnimatorComponentAsset), typeof(IUniversalComponentAsset)};
        public override Type[] conditionSubTypes => new[] {typeof(ISpineAnimatorConditionAsset), typeof(IUniversalConditionAsset)};
    }
}