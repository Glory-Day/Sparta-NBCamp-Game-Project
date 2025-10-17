using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Backend.Object.Character.Enemy.Animation
{
    [CustomAttribute(AnimationEvent.EventType.SetParry)]
    public class ParryStateMachine : StateMachineBase
    {
        private EnemyStatus _enemyStatus;
        public override void InitializeComponents(Animator animator)
        {
            base.InitializeComponents(animator);
            _enemyStatus = animator.GetComponent<EnemyStatus>();
        }
        public override void InitializeEventHandlers()
        {
            _eventHandlers.Add(AnimationEvent.EventType.SetParry, HandleSetParry);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            _enemyStatus.SetParry(false);
        }

        private void HandleSetParry(AnimationEvent e)
        {
            _enemyStatus.SetParry(e.IsBool);
        }
    }
}
