using Agatha.Common;
using SilverlightMVP.Client.Infrastructure.Eventing;

namespace SilverlightMVP.Client.Events
{
	// NOTE: right now, nobody actually subscribes to this event... but it would be useful if there
	// was some kind of central component that subscribes to it so it can display the error messages
	// or log them or whatever...
	public class RemoteExceptionOccurredEvent : Event
	{
		public RemoteExceptionOccurredEvent(ExceptionInfo exceptionInfo)
		{
			ExceptionInfo = exceptionInfo;
		}

		public ExceptionInfo ExceptionInfo { get; private set; }
	}
}