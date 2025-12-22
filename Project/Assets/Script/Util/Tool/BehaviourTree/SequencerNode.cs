using System;

namespace GloryDay.BehaviourTree
{
    public class SequencerNode : CompositeNode
    {
        private int _index;

        protected override void Start()
        {
            _index = 0;
        }

        protected override void Stop() { }

        protected override State OnUpdate()
        {
            var child = Children[_index];
            switch (child.Update())
            {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    return State.Failure;
                case State.Success:
                    _index++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return _index == Children.Count ? State.Success : State.Running;
        }
    }
}
