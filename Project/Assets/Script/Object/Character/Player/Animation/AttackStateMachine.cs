using UnityEngine;

namespace Backend.Object.Character.Player.Animation
{
    public class AttackStateMachine : StateMachineBehaviour
    {
        private AdvancedActionController _controller;

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            _controller = animator.GetComponentInParent<AdvancedActionController>();
            _controller?.OnAttackingStateEntered();
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            _controller.OnAttackingStateExited();
        }
    }
}
