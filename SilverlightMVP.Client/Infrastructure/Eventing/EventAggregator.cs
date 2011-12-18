using System;
using System.Linq;
using System.Collections.Generic;

namespace SilverlightMVP.Client.Infrastructure.Eventing
{
	public interface IEventAggregator
	{
		void Subscribe(IListener listener);
		void Subscribe<TEvent>(IListenTo<TEvent> listener) where TEvent : Event;

		void Unsubscribe(IListener listener);
		void Unsubscribe<TEvent>(IListenTo<TEvent> listener) where TEvent : Event;

		void Publish<TEvent>(TEvent message) where TEvent : Event;
		void Publish<TEvent>() where TEvent : Event, new();
		void Publish<TEvent>(TEvent message, bool asynchronously) where TEvent : Event;
		void Publish<TEvent>(bool asynchronously) where TEvent : Event, new();
	}

	public class EventAggregator : IEventAggregator
	{
		private readonly object listenerLock = new object();
		protected readonly Dictionary<Type, List<IListener>> listeners = new Dictionary<Type, List<IListener>>();
		private readonly IDispatcher dispatcher;

		public EventAggregator(IDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}

		public void Subscribe(IListener listener)
		{
			ForEachListenerInterfaceImplementedBy(listener, Subscribe);
		}

		public void Subscribe<TEvent>(IListenTo<TEvent> listener) where TEvent : Event
		{
			Subscribe(typeof(TEvent), listener);
		}

		private void Subscribe(Type typeOfEvent, IListener listener)
		{
			lock (listenerLock)
			{
				if (!listeners.ContainsKey(typeOfEvent))
				{
					listeners.Add(typeOfEvent, new List<IListener>());
				}

				if (listeners[typeOfEvent].Contains(listener))
				{
					throw new InvalidOperationException("You're not supposed to register to the same event twice");
				}

				listeners[typeOfEvent].Add(listener);
			}
		}

		public void Unsubscribe(IListener listener)
		{
			ForEachListenerInterfaceImplementedBy(listener, Unsubscribe);
		}

		public void Unsubscribe<TEvent>(IListenTo<TEvent> listener) where TEvent : Event
		{
			Unsubscribe(typeof(TEvent), listener);
		}

		private void Unsubscribe(Type typeOfEvent, IListener listener)
		{
			lock (listenerLock)
			{
				if (listeners.ContainsKey(typeOfEvent))
				{
					listeners[typeOfEvent].Remove(listener);
				}
			}
		}

		public virtual void Publish<TEvent>(TEvent message, bool asynchronously) where TEvent : Event
		{
			var typeOfEvent = typeof(TEvent);

			Action action = () =>
			{
				lock (listenerLock)
				{
					if (!listeners.ContainsKey(typeOfEvent)) return;

					foreach (var listener in listeners[typeOfEvent])
					{
						var typedReference = (IListenTo<TEvent>)listener;
						typedReference.Handle(message);
					}
				}
			};

			if (asynchronously)
			{
				dispatcher.BeginInvoke(action);
			}
			else
			{
				action();
			}
		}

		public virtual void Publish<TEvent>(TEvent message) where TEvent : Event
		{
			Publish(message, true);
		}

		public virtual void Publish<TEvent>() where TEvent : Event, new()
		{
			Publish(new TEvent(), true);
		}

		public virtual void Publish<TEvent>(bool asynchronously) where TEvent : Event, new()
		{
			Publish(new TEvent(), asynchronously);
		}

		private static void ForEachListenerInterfaceImplementedBy(IListener listener, Action<Type, IListener> action)
		{
			var listenerTypeName = typeof(IListenTo<>).Name;

			foreach (var interfaceType in listener.GetType().GetInterfaces().Where(i => i.Name.StartsWith(listenerTypeName)))
			{
				Type typeOfEvent = interfaceType.GetGenericArguments()[0];

				if (typeOfEvent != null)
				{
					action(typeOfEvent, listener);
				}
			}
		}
	}
}