using UnityEngine;

namespace GloryDay.BehaviourTree
{
    public class ProbabilityNode : DecoratorNode
    {
        public float probability;

        private bool _isSuccess;

        protected override void Start()
        {
            var number = Random.Range(0, 100);

            _isSuccess = 100 - (100 * probability) <= number;
        }

        protected override void Stop() { }

        protected override State OnUpdate()
        {
            return _isSuccess ? Child.Update() : State.Failure;
        }
    }
}
