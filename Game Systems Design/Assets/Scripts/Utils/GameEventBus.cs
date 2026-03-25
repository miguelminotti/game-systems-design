using System;
using System.Collections.Generic;

namespace MMStdLib.Utils
{
    public static class GameEventBus
    {
        /// <summary>
        /// Internal map of event type to combined delegates subscribed for that type.
        /// </summary>
        private static readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Subscribe to an event that does not carry any data.
        /// </summary>
        /// <typeparam name="T">The event type used as the key for subscription.</typeparam>
        /// <param name="listener">Action to invoke when the event is published.</param>
        public static void Subscribe<T>(Action listener)
        {
            Type type = typeof(T);
            if (_events.TryGetValue(type, out var existing))
            {
                _events[type] = Delegate.Combine(existing, listener);
            }
            else
            {
                _events.Add(type, listener);
            }
        }

        /// <summary>
        /// Subscribe to an event that carries data of type <typeparamref name="TData"/>.
        /// </summary>
        /// <typeparam name="TEvent">The event type used as the key for subscription.</typeparam>
        /// <typeparam name="TData">The data payload type passed to listeners.</typeparam>
        /// <param name="listener">Action to invoke with the event data when the event is published.</param>
        public static void Subscribe<TEvent, TData>(Action<TData> listener)
        {
            Type type = typeof(TEvent);
            if (_events.TryGetValue(type, out var existing))
            {
                _events[type] = Delegate.Combine(existing, listener);
            }
            else
            {
                _events.Add(type, listener);
            }
        }

        /// <summary>
        /// Subscribe to an event where the event type is also the payload type.
        /// This is a convenience overload equivalent to <c>Subscribe&lt;TEvent, TEvent&gt;(listener)</c>.
        /// </summary>
        /// <typeparam name="TEvent">The event and payload type.</typeparam>
        /// <param name="listener">Action to invoke with the event value.</param>
        public static void Subscribe<TEvent>(Action<TEvent> listener)
        {
            Subscribe<TEvent, TEvent>(listener);
        }

        /// <summary>
        /// Unsubscribe a previously subscribed no-data listener for the given event key type.
        /// </summary>
        /// <typeparam name="T">The event type used as the key for subscription.</typeparam>
        /// <param name="listener">The Action previously registered via <see cref="Subscribe{T}(Action)"/>.</param>
        public static void Unsubscribe<T>(Action listener)
        {
            Type type = typeof(T);
            if (_events.TryGetValue(type, out var existing))
            {
                var newDelegate = Delegate.Remove(existing, listener);
                if (newDelegate == null)
                {
                    _events.Remove(type);
                }
                else
                {
                    _events[type] = newDelegate;
                }
            }
        }

        /// <summary>
        /// Unsubscribe a data-carrying listener for the given event key type.
        /// </summary>
        /// <typeparam name="TEvent">The event type used as the key for subscription.</typeparam>
        /// <typeparam name="TData">The payload type that was used when subscribing.</typeparam>
        /// <param name="listener">The Action previously registered via <see cref="Subscribe{TEvent, TData}(Action{TData})"/>.</param>
        public static void Unsubscribe<TEvent, TData>(Action<TData> listener)
        {
            Type type = typeof(TEvent);
            if (_events.TryGetValue(type, out var existing))
            {
                var newDelegate = Delegate.Remove(existing, listener);
                if (newDelegate == null)
                {
                    _events.Remove(type);
                }
                else
                {
                    _events[type] = newDelegate;
                }
            }
        }

        /// <summary>
        /// Unsubscribe a listener where the event type is also the payload type.
        /// Convenience overload equivalent to <c>Unsubscribe&lt;TEvent, TEvent&gt;(listener)</c>.
        /// </summary>
        /// <typeparam name="TEvent">The event and payload type.</typeparam>
        /// <param name="listener">The Action previously registered.</param>
        public static void Unsubscribe<TEvent>(Action<TEvent> listener)
        {
            Unsubscribe<TEvent, TEvent>(listener);
        }

        /// <summary>
        /// Publish an event that carries no data. All subscribed <see cref="Action"/> listeners
        /// registered under the event key <typeparamref name="T"/> will be invoked.
        /// </summary>
        /// <typeparam name="T">The event type used as the key for publishing.</typeparam>
        /// <exception cref="Exception">Thrown when the registered delegate is not an <see cref="Action"/>.</exception>
        public static void Publish<T>()
        {
            Type type = typeof(T);
            if (_events.TryGetValue(type, out var existing))
            {
                if (existing is Action action)
                {
                    action.Invoke();
                }
                else
                {
                    throw new Exception($"Event of type {type} is not an Action.");
                }
            }
        }

        /// <summary>
        /// Publish an event that carries a payload of type <typeparamref name="TData"/>.
        /// All subscribed <see cref="Action{TData}"/> listeners registered under the
        /// event key <typeparamref name="TEvent"/> will be invoked with <paramref name="data"/>.
        /// </summary>
        /// <typeparam name="TEvent">The event type used as the key for publishing.</typeparam>
        /// <typeparam name="TData">The payload type passed to listeners.</typeparam>
        /// <param name="data">The data to send to listeners.</param>
        /// <exception cref="Exception">Thrown when the registered delegate is not an <see cref="Action{TData}"/>.</exception>
        public static void Publish<TEvent, TData>(TData data)
        {
            Type type = typeof(TEvent);
            if (_events.TryGetValue(type, out var existing))
            {
                if (existing is Action<TData> action)
                {
                    action.Invoke(data);
                }
                else
                {
                    throw new Exception($"Event of type {type} is not an Action<{typeof(TData)}>.");
                }
            }
        }

        /// <summary>
        /// Publish an event where the event type and payload type are the same.
        /// Convenience overload that forwards to <see cref="Publish{TEvent, TData}(TData)"/>.
        /// </summary>
        /// <typeparam name="TEvent">The event and payload type.</typeparam>
        /// <param name="data">The payload to send to listeners.</param>
        public static void Publish<TEvent>(TEvent data)
        {
            Publish<TEvent, TEvent>(data);
        }

        /// <summary>
        /// Clear all subscriptions from the event bus. After calling this, no listeners
        /// remain registered until new subscriptions are added.
        /// </summary>
        public static void Clear()
        {
            _events.Clear();
        }
    }
}