using GloryDay.BehaviourTree;

namespace Backend.Object.Character.Enemy.Node
{
    public class SetRotationToPlayerFalse : ActionNode
    {
        public bool faceToPlayer = false;
        protected override void Start()
        {
            agent.MovementController.FaceToPlayer = faceToPlayer;
        }
        protected override void Stop()
        {

        }
        protected override State OnUpdate()
        {
            return State.Success;
        }
    }
}
