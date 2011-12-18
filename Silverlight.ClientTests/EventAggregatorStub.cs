using System;
using System.Collections.Generic;
using System.Linq;
using SilverlightMVP.Client.Infrastructure;
using SilverlightMVP.Client.Infrastructure.Eventing;

namespace Silverlight.ClientTests
{
	public class EventAggregatorStub : EventAggregator
	{
		private List<Event> publishedEvents = new List<Event>();

		public EventAggregatorStub() : base(new DispatcherStub()) { }

		public EventAggregatorStub(IDispatcher dispatcher) : base(dispatcher) { }

		public override void Publish<TEvent>(TEvent message, bool asynchronously)
		{
			publishedEvents.Add(message);
			base.Publish(message, asynchronously);
		}

		public bool IsListenerSubscribedTo<TEvent>(IListener listener)
		{
			Type typeOfEvent = typeof(TEvent);

			if (!listeners.ContainsKey(typeOfEvent))
			{
				return false;
			}

			return listeners[typeOfEvent].Contains(listener);
		}

		public TEvent[] GetPublishedEvents<TEvent>()
		{
			return publishedEvents.OfType<TEvent>().ToArray();
		}

		public Event[] GetPublishedEvents()
		{
			return publishedEvents.ToArray();
		}

		public int GetSubscriptionCount()
		{
			return listeners.SelectMany(l => l.Value).Count();
		}

		public void Clear()
		{
			listeners.Clear();
		}

		public void ClearPublishedEvents()
		{
			publishedEvents.Clear();
		}

		public void ClearPublishedEvents<TEvent>()
		{
			publishedEvents = publishedEvents.Where(e => !e.GetType().Equals(typeof(TEvent))).ToList();
		}
	}
}