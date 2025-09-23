using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class IsAttackEndNormalize : ActionNode
    {
        [Range(0, 1)] public float TransitionPoint = 0.8f;
        public int LayerNumber = 0;
        //전환 최소 시간
        public float MinimTime = 0.1f;

        private float currentAnimStartTime = 0.1f;

        protected override void Start()
        {
            currentAnimStartTime = Time.time;
        }
        protected override void Stop()
        {
        }
        protected override State OnUpdate()
        {
            if (Time.time - currentAnimStartTime < MinimTime)
            {
                return State.Running;
            }

            //if(agent.AnimationController.GetCurrentNameHash(0) == blackboard.currentAnimationHash)
            //{
            //    if(agent.AnimationController.GetAnimationNormalize(0) >= TransitionPoint && agent.AnimationController.GetAnimationNormalize(0) < 1.0f)
            //    {
            //        return State.Success;
            //    }
            //    else if(agent.AnimationController.GetAnimationNormalize(0) >= 1.0f)
            //    {
            //        return State.Success;
            //    }
            //}
            return State.Running;
        }
    }
}
