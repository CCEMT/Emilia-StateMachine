using Emilia.StateMachine;
using Spine.Unity;
using UnityEngine;

namespace Emilia.SpineAnimator
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class SpineAnimatorMono : MonoBehaviour
    {
        public const string EditorPath = "Assets/Example/Resource/Asset";

        public string fileName;

        [StateMachineKeySelector(nameof(path))]
        public string moveKey;

        [StateMachineKeySelector(nameof(path))]
        public string jumpKey;

        private SkeletonAnimation _skeletonAnimation;

        private IStateMachineRunner runner;

        public SkeletonAnimation skeletonAnimation => _skeletonAnimation;

        public string path => $"{EditorPath}/{fileName}.asset";

        private void Awake()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(fileName)) return;

            StateMachineLoader stateMachineLoader = new StateMachineLoader();
            stateMachineLoader.editorFilePath = EditorPath;

            runner = StateMachineRunnerUtility.CreateRunner();
            runner.Init(this.fileName, stateMachineLoader, skeletonAnimation);
            this.runner.Start();
        }

        private void Update()
        {
            this.runner?.Update();

            if (this.runner?.isActive == false) return;

            bool isRun = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
            this.runner.stateMachine.userVariablesManage.SetValue(moveKey, isRun);
            bool isJump = Input.GetKeyDown(KeyCode.Space);
            this.runner.stateMachine.userVariablesManage.SetValue(jumpKey, isJump);
        }

        private void OnDisable()
        {
            this.runner?.Dispose();
            runner = null;
        }
    }
}