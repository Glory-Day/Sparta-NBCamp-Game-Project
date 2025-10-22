using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class IsNormalEnemyMoveStop : ActionNode
    {
        protected override void Start()
        {
        }

        protected override State OnUpdate()
        {


            // pathPending: 경로 계산 중인지 확인, remainingDistance: 남은 거리
            if (!agent.NavMeshAgent.hasPath || (!agent.NavMeshAgent.pathPending && agent.NavMeshAgent.remainingDistance <= agent.NavMeshAgent.stoppingDistance))
            {
                return State.Success;
            }

            return State.Running;
        }

        protected override void Stop()
        {
            agent.NavMeshAgent.ResetPath();
        }
    }
}
