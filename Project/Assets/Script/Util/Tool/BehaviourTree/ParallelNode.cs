using System;
using System.Collections.Generic;

namespace GloryDay.BehaviourTree
{
    public class ParallelNode : CompositeNode
    {
        private readonly List<State> _states = new ();

        protected override void Start()
        {
            _states.Clear();

            Children.ForEach(_ => { _states.Add(State.Running); });
        }

        protected override void Stop() { }

        protected override State OnUpdate()
        {
            var isRunning = false;

            var count = _states.Count;
            for (var i = 0; i < count; ++i)
            {
                if (_states[i] != State.Running)
                {
                    continue;
                }

                var status = Children[i].Update();
                switch (status)
                {
                    case State.Failure:
                        AbortRunningChildren();
                        return State.Failure;
                    case State.Running:
                        isRunning = true;
                        break;
                    case State.Success:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _states[i] = status;
            }

            return isRunning ? State.Running : State.Success;
        }

        private void AbortRunningChildren()
        {
            var count = _states.Count;
            for (int i = 0; i < count; ++i)
            {
                if (_states[i] == State.Running)
                {
                    Children[i].Abort();
                }
            }
        }
    }
}
