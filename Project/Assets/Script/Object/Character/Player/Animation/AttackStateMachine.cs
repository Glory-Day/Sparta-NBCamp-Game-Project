using UnityEngine;

namespace Backend.Object.Character.Player.Animation
{
    public class AttackStateMachine : StateMachineBehaviour
    {
        public AnimationCurve speed;

        private AdvancedActionController _controller;
        private DamageSender _sender;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller = animator.GetComponentInParent<AdvancedActionController>();
            _sender = animator.GetComponentInChildren<DamageSender>();

            _controller?.OnAttackingStateEntered();
            _sender?.StartDetection();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var normalized = stateInfo.normalizedTime % 1f;

            _controller.deltaSpeed = speed.Evaluate(normalized);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller.deltaSpeed = 1f;
            _controller.OnAttackingStateExited();

            _sender.StopDetection();
        }
    }
}
