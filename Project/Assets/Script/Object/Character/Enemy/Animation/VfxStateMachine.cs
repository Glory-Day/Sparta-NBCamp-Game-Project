using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Animation
{
    [CustomAttribute(
        AnimationEvent.EventType.SetEffect, AnimationEvent.EventType.PlayEffect, AnimationEvent.EventType.StopEffect)]
    public class VfxStateMachine : StateMachineBase
    {
        private EnemyCombatController _combatController;

        public override void InitializeEventHandlers()
        {
            _eventHandlers.Add(AnimationEvent.EventType.SetEffect, HandleSetEffect);
            _eventHandlers.Add(AnimationEvent.EventType.PlayEffect, HandlePlayEffect);
            _eventHandlers.Add(AnimationEvent.EventType.StopEffect, HandleStopEffect);
        }

        public override void InitializeComponents(Animator animator)
        {
            base.InitializeComponents(animator);
            _combatController = animator.GetComponent<EnemyCombatController>();
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        private void HandleSetEffect(AnimationEvent e)
        {
            if (_combatController != null)
            {
                _combatController.SetEffect(e.Index);
            }
        }

        private void HandlePlayEffect(AnimationEvent e)
        {
            if (_combatController != null)
            {
                _combatController.StartEffect();
            }
        }

        private void HandleStopEffect(AnimationEvent e)
        {
            if (_combatController != null)
            {
                _combatController.EndEffect();
            }
        }
    }
}

