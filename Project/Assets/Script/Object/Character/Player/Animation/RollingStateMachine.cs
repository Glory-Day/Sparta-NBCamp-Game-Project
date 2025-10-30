using UnityEngine;

namespace Backend.Object.Character.Player.Animation
{
    public class RollingStateMachine : StateMachineBehaviour
    {
        private AdvancedActionController _controller;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller = animator.GetComponentInParent<AdvancedActionController>();

            _controller?.OnRollingStateEntered();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller.OnRollingStateExited();

            _controller.IsRollButtonBufferable = false;
        }
    }
}
