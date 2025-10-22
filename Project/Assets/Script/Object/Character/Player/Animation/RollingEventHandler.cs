using System.Collections.Generic;
using Backend.Object.Animation;
using UnityEngine;

namespace Backend.Object.Character.Player.Animation
{
    public class RollingEventHandler : StateMachineBehaviour
    {
        [Header("Event Settings")]
        [SerializeField] private List<float> times;

        private AdvancedActionController _controller;

        private readonly AnimationEventRegister _register = new ();
        private readonly bool[] _triggers = new bool[1];

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //TODO: 빈도 높은 고자원 메서드 사용 중임으로 차후에 수정해야 한다.
            _controller = animator.GetComponentInParent<AdvancedActionController>();
            _register.Register(IsRollButtonBufferedValid);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var progress = stateInfo.normalizedTime % 1f;

            if (_triggers[0] == false && progress >= times[0])
            {
                _triggers[0] = true;
                _register.Invoke(0);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _register.Unregister(IsRollButtonBufferedValid);
            _controller.IsRollButtonBufferable = false;
            _triggers[0] = false;
        }

        private void IsRollButtonBufferedValid()
        {
            _controller.IsRollButtonBufferable = true;
        }
    }
}
