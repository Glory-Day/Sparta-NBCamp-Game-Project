using UnityEngine;

namespace GloryDay.BehaviourTree
{
    public class RandomWaitNode : ActionNode
    {
        public float minWaitTime = 0f;
        public float maxWaitTime = 2f;

        private float _waitTime;
        private float _startTime;

        protected override void Start()
        {
            _waitTime = Random.Range(minWaitTime, maxWaitTime);
            _startTime = Time.time;
        }

        protected override void Stop() { }

        protected override State OnUpdate()
        {
            return Time.time - _startTime >= _waitTime ? State.Success : State.Running;
        }
    }
}
