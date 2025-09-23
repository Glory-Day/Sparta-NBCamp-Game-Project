namespace Backend.Object.Character.Enemy.Node
{
    public class MoveToDest : ActionNode
    {
        public bool EnableRotation = false;
        // 기본 이동 속도입니다.
        public float Speed = 5f;
        // 회전 속도입니다.
        public float RotationSpeed = 3f;
        // 목적지까지의 거리에 따라 속도를 동적으로 변경할지 여부입니다.
        public bool VariableSpeedForDist;

        protected override void Start()
        {
        }

        protected override void Stop()
        {
        }
        // 노드가 실행되는 동안 매 프레임 호출됩니다.
        protected override State OnUpdate()
        {
            if (VariableSpeedForDist)
            {
                float speedFactor = 0f;
                // 목적지까지의 거리를 계산합니다.
                float distance = agent.MovementController.Distance;
                // 거리에 따라 속도 계수(speedFactor)를 다르게 설정합니다.
                // 멀리 있을수록 더 빠르게 움직입니다.
                if (distance < 5)
                {
                    speedFactor = 1.5f;
                }
                else if (distance < 10)
                {
                    speedFactor = 2f;
                }
                else if (distance < 17)
                {
                    speedFactor = 3f;
                }
                else if (distance < 27)
                {
                    speedFactor = 4.7f;
                }
                else if (distance < 45)
                {
                    speedFactor = 6f;
                }
                else if (distance < 75)
                {
                    speedFactor = 9f;
                }
                // NavMeshAgent가 계산한 경로(desiredVelocity)를 사용하여 CharacterController로 실제 이동을 처리합니다.
                // 이를 통해 NavMesh의 경로 탐색 기능과 CharacterController의 물리 기반 이동을 함께 사용할 수 있습니다.
                agent.MovementController.MoveToTarget(Speed, speedFactor);
            }
            else // 고정 속도를 사용하는 경우
            {
                agent.MovementController.MoveToTarget(Speed);
            }

            if (EnableRotation)
            {
                agent.MovementController.SetLerpRotation(Speed);
            }
            // 이 노드는 이동을 '시도'하는 역할만 하므로, 항상 Success를 반환하여 시퀀스의 다음 노드가 실행되도록 합니다.
            return State.Success;
        }
    }
}
