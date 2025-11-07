using GloryDay.BehaviourTree;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class RandomIdle : ActionNode
    {
        public float MinTime = 1f;
        public float MaxTime = 3f;
        private float _waitTime;
        private float _startTime;
        protected override void Start()
        {
            agent.NavMeshAgent.ResetPath();
            _startTime = Time.time;
            _waitTime = Random.Range(MinTime, MaxTime);
        }

        protected override void Stop()
        {

        }

        protected override State OnUpdate()
        {
            if (Time.time - _startTime >= _waitTime)
            {
                return State.Success;
            }

            return State.Running;
        }
    }
}
