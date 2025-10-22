using System.Collections.Generic;
using Backend.Object.Animation;
using UnityEngine;

namespace Backend.Object.Character.Player.Animation
{
    public class AttackEventHandler : StateMachineBehaviour
    {
        [Header("Event Settings")]
        [SerializeField] private List<float> times;

        private DamageSender _sender;

        private readonly AnimationEventRegister _register = new ();
        private readonly bool[] _triggers = new bool[2];

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //TODO: 빈도 높은 고자원 메서드 사용 중임으로 차후에 수정해야 한다.
            _sender = animator.GetComponentInChildren<DamageSender>();
            _register.Register(_sender.StartDetection);
            _register.Register(_sender.StopDetection);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var progress = stateInfo.normalizedTime % 1f;

            for (var i = 0; i < _register.Length; i++)
            {
                if (_triggers[i] == false && progress >= times[i])
                {
                    _triggers[i] = true;
                    _register.Invoke(i);
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _register.Unregister(_sender.StartDetection);
            _register.Unregister(_sender.StopDetection);
            _triggers[0] = false;
            _triggers[1] = false;
        }
    }
}
