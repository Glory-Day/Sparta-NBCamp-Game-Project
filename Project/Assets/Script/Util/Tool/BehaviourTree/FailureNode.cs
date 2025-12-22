namespace GloryDay.BehaviourTree
{
    public class FailureNode : DecoratorNode
    {
        protected override void Start() { }

        protected override void Stop() { }

        protected override State OnUpdate()
        {
            if (Child is null)
            {
                return State.Failure;
            }

            var state = Child.Update();

            return state == State.Success ? State.Failure : state;
        }
    }
}
