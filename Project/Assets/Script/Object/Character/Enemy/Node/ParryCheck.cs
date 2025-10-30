using System.Collections;
using System.Collections.Generic;
using Backend.Util.Debug;
using GloryDay.BehaviourTree;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class ParryCheck : ActionNode
    {
        public float grogyTimer = 0f;

        private float _timer = 0f;

        private readonly int parryAnim = Animator.StringToHash("IsParry");
        private readonly int Idle = Animator.StringToHash("Idle");
        protected override State OnUpdate()
        {
            //그로기 발생
            _timer += Time.deltaTime;

            if (grogyTimer <= _timer)
            {
                blackboard.IsParry = true;
                return State.Success;
            }

            return State.Running;
        }
        protected override void Start()
        {
            _timer = 0f;

            if (!blackboard.IsParry)
            {
                agent.AnimationController.SetAnimationTrigger("IsParry");
            }
        }
        protected override void Stop()
        {
            agent.AnimationController.SetCrossFadeInFixedTime(Idle, 0.8f);
            blackboard.IsParry = true;
        }
    }
}

