namespace Backend.Object.Character.Enemy.Node
{
    public class IsPlayerInRange : ActionNode
    {
        public float minDistance = 0f;
        public float maxDistance = 5f;
        protected override void Start()
        {
        }
        protected override void Stop()
        {
        }
        protected override State OnUpdate()
        {
            float distance = agent.MovementController.Distance; // 플레이어와의 거리 계산
            if (distance >= minDistance && distance < maxDistance)
            {
                return State.Success;
            }
            else
            {
                return State.Failure;
            }
        }
    }
}
