using Backend.Object.Character.Player;
using UnityEngine;

namespace Backend.Object.Animation
{
    public class AnimationMovementController : StateMachineBehaviour
    {
        public AnimationCurve speed;

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
