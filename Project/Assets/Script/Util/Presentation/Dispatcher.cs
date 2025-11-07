using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Util.Debug;
using UnityEngine.Rendering;

namespace Backend.Util.Presentation
{
    public class Dispatcher : IDispatcher
    {
        private readonly Dictionary<int, List<ISubscriber>> _subscribers = new ();

        public void Subscribe(ISubscriber subscriber)
        {
            var type = subscriber.GetType();
            var key = GetHashCode(type);

            if (!_subscribers.ContainsKey(key))
            {
                _subscribers.Add(key, new List<ISubscriber>());
            }
            _subscribers[key].Add(subscriber);

            //Debugger.LogError($"Cannot add {subscriber} because it's already subscribed.");
        }

        public void Unsubscribe(ISubscriber subscriber)
        {
            var type = subscriber.GetType();
            var key = GetHashCode(type);

            if (_subscribers.ContainsKey(key))
            {
                _subscribers[key].Remove(subscriber);
                if (_subscribers[key].Count == 0)
                {
                    _subscribers.Remove(key);
                }
            }
        }

        public void DispatchTo<TSubscriber, TMessage>(int index, TMessage message) where TSubscriber : ISubscriber
        {
            var type = typeof(TSubscriber);
            var key = GetHashCode(type);

            if (_subscribers.TryGetValue(key, out var subcribers))
            {
                subcribers[index].Receive(message);
            }
        }

        public void DispatchToAll<T>(T message)
        {
            var values = _subscribers.Values.ToArray();

            var length = values.Length;
            for (var i = 0; i < length; i++)
            {
                for(var j = 0; j < values[i].Count; j++)
                {
                    values[i][j].Receive(message);
                }
            }
        }

        private static int GetHashCode(Type type)
        {
            return type.GetHashCode();
        }
    }
}
