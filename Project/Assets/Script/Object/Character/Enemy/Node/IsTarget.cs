using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class IsTaget : ActionNode
    {


        protected override void Start()
        {
        }
        // 노드가 중지될 때 호출됩니다.
        protected override void Stop()
        {
        }
        // 노드가 실행되는 동안 매 프레임 호출됩니다.
        protected override State OnUpdate()
        {
            if(agent.MovementController.Target != null)
            {
                return State.Success;
            }
            return State.Failure;
        }
    }
}
