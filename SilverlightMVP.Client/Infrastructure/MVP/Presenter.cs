using Agatha.Common;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Infrastructure.Eventing;

namespace SilverlightMVP.Client.Infrastructure.MVP
{
	public abstract class Presenter<TView, TBindingModel>
		where TView : IView
		where TBindingModel : BindingModel<TBindingModel>, new()
	{
		protected Presenter(TView view, IEventAggregator eventAggregator, IAsyncRequestDispatcherFactory requestDispatcherFactory)
		{
			View = view;
			EventAggregator = eventAggregator;
			RequestDispatcherFactory = requestDispatcherFactory;
			BindingModel = new TBindingModel();
		}

		protected IEventAggregator EventAggregator { get; private set; }
		protected IAsyncRequestDispatcherFactory RequestDispatcherFactory { get; private set; } 
		protected TView View { get; private set; }

		public TBindingModel BindingModel { get; private set; }

		public virtual void Initialize() {}

		protected void PublishRemoteException(ExceptionInfo exceptionInfo)
		{
			EventAggregator.Publish(new RemoteExceptionOccurredEvent(exceptionInfo));
		}
	}
}