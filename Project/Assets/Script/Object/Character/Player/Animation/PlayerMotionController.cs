using UnityEngine;

namespace Backend.Object.Character.Player.Animation
{
    public class PlayerMotionController : StateMachineBehaviour
    {
        public AnimationCurve speed = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        private AdvancedActionController _controller;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller = animator.GetComponentInParent<AdvancedActionController>();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var normalized = stateInfo.normalizedTime % 1f;

            _controller.deltaSpeed = speed.Evaluate(normalized);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller.deltaSpeed = 1f;
        }
    }
}
