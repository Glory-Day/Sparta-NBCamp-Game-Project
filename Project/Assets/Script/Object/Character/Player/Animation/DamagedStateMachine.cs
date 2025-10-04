using UnityEngine;

namespace Backend.Object.Character.Player.Animation
{
    public class DamagedStateMachine : StateMachineBehaviour
    {
        public AnimationCurve speed;

        private AdvancedActionController _controller;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller = animator.GetComponentInParent<AdvancedActionController>();
            _controller?.OnDamagedStateEntered();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller.OnDamagedStateExited();
        }
    }
}
