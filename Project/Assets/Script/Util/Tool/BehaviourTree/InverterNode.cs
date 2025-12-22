namespace GloryDay.BehaviourTree
{
    public class InverterNode : DecoratorNode
    {
        protected override void Start() { }

        protected override void Stop() { }

        protected override State OnUpdate()
        {
            if (Child is null)
            {
                return State.Failure;
            }

            return Child.Update() switch
            {
                State.Running => State.Running,
                State.Failure => State.Success,
                State.Success => State.Failure,
                _ => State.Failure
            };
        }
    }
}
