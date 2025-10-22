using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Util.Debug;

namespace Backend.Util.Presentation
{
    public class Dispatcher : IDispatcher
    {
        private readonly Dictionary<int, ISubscriber> _subscribers = new ();

        public void Subscribe(ISubscriber subscriber)
        {
            var type = subscriber.GetType();
            var key = GetHashCode(type);

            if (_subscribers.TryAdd(key, subscriber))
            {
                return;
            }

            Debugger.LogError($"Cannot add {subscriber} because it's already subscribed.");
        }

        public void Unsubscribe(ISubscriber subscriber)
        {
            var type = subscriber.GetType();
            var key = GetHashCode(type);

            _subscribers.Remove(key);
        }

        public void DispatchTo<TSubscriber, TMessage>(TMessage message) where TSubscriber : ISubscriber
        {
            var type = typeof(TSubscriber);
            var key = GetHashCode(type);

            if (_subscribers.TryGetValue(key, out var subscriber))
            {
                subscriber.Receive(message);
            }
        }

        public void DispatchToAll<T>(T message, bool isReversed)
        {
            var values = _subscribers.Values;
            var subscribers = isReversed ? values.Reverse().ToArray() : values.ToArray();

            var length = subscribers.Length;
            for (var i = 0; i < length; i++)
            {
                subscribers[i].Receive(message);
            }
        }

        private static int GetHashCode(Type type)
        {
            return type.GetHashCode();
        }
    }
}
