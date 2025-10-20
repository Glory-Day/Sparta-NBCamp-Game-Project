using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Animation
{
    [CustomAttribute(
        AnimationEvent.EventType.SetSpeed)]
    public class SpeedStateMachine : StateMachineBase
    {
        private float _speed;
        public override void InitializeComponents(Animator animator)
        {
            base.InitializeComponents(animator);
        }
        public override void InitializeEventHandlers()
        {
            _eventHandlers.Add(AnimationEvent.EventType.SetSpeed, SpeedChangeHandler);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            _speed = animator.speed;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            animator.speed = _speed;
        }

        public void SpeedChangeHandler(AnimationEvent e)
        {
            _animator.speed = e.Value;
        }
    }
}
