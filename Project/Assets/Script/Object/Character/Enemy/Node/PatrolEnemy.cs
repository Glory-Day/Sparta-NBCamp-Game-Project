using GloryDay.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Backend.Object.Character.Enemy.Node
{
    public class PatrolEnemy : ActionNode
    {
        public enum PatrolMode { Walk, Run }

        [Tooltip("순찰 이동 방식을 선택합니다 (걷기 또는 달리기).")]
        public PatrolMode Mode = PatrolMode.Walk;
        [Tooltip("걷기 속도입니다.")]
        public float WalkSpeed = 2.0f;
        [Tooltip("달리기 속도입니다.")]
        public float RunSpeed = 5.0f;


        private bool _pathFound = false;

        protected override void Start()
        {
            // 선택된 모드에 따라 NavMeshAgent의 속도를 설정합니다.
            float targetSpeed = (Mode == PatrolMode.Run) ? RunSpeed : WalkSpeed;
            agent.NavMeshAgent.speed = targetSpeed;

            // 이동할 새로운 경로를 찾습니다.
            _pathFound = MoveToRandomPositionInPatrolArea();
        }

        protected override State OnUpdate()
        {
            // 1. 경로를 찾지 못했다면 즉시 실패 처리
            if (!_pathFound)
            {
                return State.Failure;
            }
            // EnemyMovementController의 타겟이 null이 아닐 때 (플레이어를 감지했을 때)
            if (agent.MovementController.Target != null)
            {
                Debug.Log("IsNormalEnemyMoveStop: Target detected, stopping movement.");
                agent.NavMeshAgent.ResetPath();
                return State.Success;
            }
            // 2. NavMeshAgent의 현재 속도를 기반으로 애니메이션 블렌드 값을 계산합니다.
            float normalizedSpeed = agent.NavMeshAgent.velocity.magnitude / agent.NavMeshAgent.speed;

            // 걷기 모드일 경우, 애니메이션 파라미터의 최대값을 0.5로 제한합니다.
            if (Mode == PatrolMode.Walk)
            {
                normalizedSpeed *= 0.5f;
            }


            // 3. 목적지에 도착했는지 확인
            if (!agent.NavMeshAgent.pathPending && agent.NavMeshAgent.remainingDistance <= agent.NavMeshAgent.stoppingDistance)
            {
                // 도착했으므로 애니메이션을 멈추고 Success 반환
                return State.Success;
            }

            // 4. 아직 목적지로 이동 중이면 Running 상태 유지
            return State.Running;
        }

        protected override void Stop()
        {
            // 노드가 중단될 때 애니메이션을 멈추고 경로를 초기화합니다.
            if (agent.NavMeshAgent.hasPath)
            {
                agent.NavMeshAgent.ResetPath();
            }
        }

        private bool MoveToRandomPositionInPatrolArea()
        {
            Vector3 initialPosition = agent.MovementController.InitialPosition;
            float moveRange = agent.MovementController.MoveRange;

            Vector3 randomDirection = Random.insideUnitSphere * moveRange;
            randomDirection += initialPosition;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, moveRange, NavMesh.AllAreas))
            {
                agent.NavMeshAgent.SetDestination(hit.position);
                return true;
            }

            return false;
        }
    }
}
