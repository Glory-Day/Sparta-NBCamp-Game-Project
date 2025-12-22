using Backend.Util.Debug;

namespace GloryDay.BehaviourTree
{
    public class DebugLogNode : ActionNode
    {
        public string Message;

        protected override void Start()
        {
            Debugger.LogMessage(Message);
        }

        protected override void Stop()
        {
            Debugger.LogMessage(Message);
        }

        protected override State OnUpdate()
        {
            Debugger.LogMessage(Message);

            return State.Success;
        }
    }
}
