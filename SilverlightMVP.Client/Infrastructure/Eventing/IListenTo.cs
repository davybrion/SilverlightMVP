namespace SilverlightMVP.Client.Infrastructure.Eventing
{
	public interface IListener {} // just a marker interface

	public interface IListenTo<TEvent> : IListener
		where TEvent : Event
	{
		void Handle(TEvent receivedEvent);
	}
}