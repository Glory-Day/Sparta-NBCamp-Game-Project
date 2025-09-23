using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class SetMoveToDestination : ActionNode
    {
        //목표 설정
        public Vector3 DestinationPosition;
        public bool PlayerDestination;
        public float Offset = 2.0f;

        protected override void Start() { }
        protected override void Stop() { }

        protected override State OnUpdate()
        {
            if (PlayerDestination)
            {
                Transform agentTransform = agent.MovementController.transform;
                Transform playerTransform = agent.MovementController.Target.transform;

                if (playerTransform == null)
                {
                    return State.Failure;
                }

                Vector3 dir = agent.MovementController.GetDirection();
                Vector3 destination = playerTransform.position - (dir * Offset);
                destination.y = agentTransform.position.y;
                blackboard.moveDestination = destination;

                return State.Success;
            }
            else
            {
                Vector3 worldDestination = agent.MovementController.transform.TransformPoint(DestinationPosition);
                blackboard.moveDestination = worldDestination;

                return State.Success;
            }
        }
    }
}
