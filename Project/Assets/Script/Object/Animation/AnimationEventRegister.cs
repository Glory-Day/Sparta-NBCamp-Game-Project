using System;
using System.Collections.Generic;

namespace Backend.Object.Animation
{
    public class AnimationEventRegister
    {
        private readonly List<Action> _actions = new ();

        public void Invoke()
        {
            var count = _actions.Count;

            for (var i = 0; i < count; i++)
            {
                _actions[i]?.Invoke();
            }
        }

        public void Invoke(int index)
        {
            _actions[index]?.Invoke();
        }

        public void Register(Action action)
        {
            _actions.Add(action);
        }

        public void Unregister(Action action)
        {
            _actions.Remove(action);
        }

        public void Clear()
        {
            _actions.Clear();
        }

        public int Length => _actions.Count;
    }
}
