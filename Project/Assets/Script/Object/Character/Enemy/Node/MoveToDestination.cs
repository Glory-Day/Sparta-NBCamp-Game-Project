using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class MoveToDestination : ActionNode
    {
        public bool EnableRotation = false;
        public float RotationSpeed = 10f;
        public float Speed = 5f;
        public float StopDistance = 0.1f;

        private Vector3 _targetPos;

        protected override void Start()
        {
            _targetPos = blackboard.moveDestination;
        }

        protected override void Stop()
        {

        }
        // 노드가 실행되는 동안 매 프레임 호출됩니다.
        protected override State OnUpdate()
        {
            if(Vector3.Distance(agent.MovementController.transform.position, _targetPos) <= StopDistance)
            {
                return State.Success;
            }

            agent.MovementController.MoveToTarget(agent.MovementController.transform.position, _targetPos, Speed);

            if (EnableRotation)
            {
                agent.MovementController.SetLerpRotation(agent.MovementController.transform.position, _targetPos, Speed);
            }
            return State.Running;
        }
    }
}
