using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Animation
{
    public class AttackStateMachine : StateMachineBase
    {
        private bool _isAttack = false;
        private EnemyCombatController _combatController;

        public override void InitializeComponents(Animator animator)
        {
            base.InitializeComponents(animator);
            _combatController = animator.GetComponent<EnemyCombatController>();
        }

        public override void InitializeEventHandlers()
        {
            _eventHandlers.Add(AnimationEvent.EventType.SetWeapon, HandleSetWeapon);
            _eventHandlers.Add(AnimationEvent.EventType.StartAttack, HandleStartAttack);
            _eventHandlers.Add(AnimationEvent.EventType.EndAttack, HandleEndAttack);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            _isAttack = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (_isAttack && _combatController != null)
            {
                _isAttack = false;
            }
        }

        private void HandleStartAttack(AnimationEvent e)
        {
            if (_combatController != null)
            {
                _combatController.StartAttack();
                _isAttack = true;
            }
        }

        private void HandleEndAttack(AnimationEvent e)
        {
            if (_combatController != null)
            {
                _combatController.EndAttack();
                _isAttack = false;
            }
        }

        private void HandleSetWeapon(AnimationEvent e)
        {
            if (_combatController != null)
            {
                _combatController.SetWeapon(e.Index);
            }
        }
    }
}
