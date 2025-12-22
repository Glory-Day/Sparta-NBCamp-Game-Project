namespace GloryDay.BehaviourTree
{
    public class SucceedNode : DecoratorNode
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

            return state == State.Failure ? State.Success : state;
        }
    }
}
