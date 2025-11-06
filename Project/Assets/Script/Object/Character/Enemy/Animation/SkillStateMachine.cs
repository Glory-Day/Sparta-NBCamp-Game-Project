using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Enemy.Boss.Skill;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Animation
{
    [CustomAttribute(AnimationEvent.EventType.PlaySkill, AnimationEvent.EventType.MovePos)]
    public class SkillStateMachine : StateMachineBase
    {
        private EnemySkillsController _nineTaiedBeastSkills;
        public override void InitializeComponents(Animator animator)
        {
            base.InitializeComponents(animator);
            _nineTaiedBeastSkills = animator.GetComponent<EnemySkillsController>();
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void InitializeEventHandlers()
        {
            _eventHandlers.Add(AnimationEvent.EventType.PlaySkill, HandlePlaySkill);
            _eventHandlers.Add(AnimationEvent.EventType.MovePos, HandleMoveToPos);
        }

        private void HandlePlaySkill(AnimationEvent e)
        {
            _nineTaiedBeastSkills.SpawnProjectile(e.Descript);
        }

        private void HandleMoveToPos(AnimationEvent e)
        {
            _nineTaiedBeastSkills.MoveToPosition(e.Descript);
        }
    }
}
