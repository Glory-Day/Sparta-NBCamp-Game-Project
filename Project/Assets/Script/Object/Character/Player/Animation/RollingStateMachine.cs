using UnityEngine;

namespace Backend.Object.Character.Player.Animation
{
    public class RollingStateMachine : StateMachineBehaviour
    {
        public AnimationCurve speed;

        private AdvancedActionController _controller;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller = animator.GetComponentInParent<AdvancedActionController>();

            _controller?.OnRollingStateEntered();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var normalized = stateInfo.normalizedTime % 1f;

            _controller.rollingSpeed = speed.Evaluate(normalized);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller.rollingSpeed = 1f;
            _controller.OnRollingStateExited();
        }
    }
}
