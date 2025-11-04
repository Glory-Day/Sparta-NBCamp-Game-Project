using UnityEngine;

namespace Backend.Object.Character.Player.Animation
{
    public class PlayerActionController : StateMachineBehaviour
    {
        [SerializeField] private State state;

        private AdvancedActionController _controller;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller = animator.GetComponentInParent<AdvancedActionController>();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller.State = state;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller.State = State.Grounded;
            _controller.IsButtonBufferable = false;
        }
    }
}
