using System;
using System.Windows;
using System.Windows.Threading;

namespace SilverlightMVP.Client.Infrastructure
{
	public interface IDispatcher
	{
		void BeginInvoke(Action action);
		void BeginInvoke(Delegate action, params object[] arguments);
		bool CheckAccess();
	}

	public class DispatcherWrapper : IDispatcher
	{
		private static Dispatcher RealDispatcher
		{
			get { return Deployment.Current.Dispatcher; }
		}

		public void BeginInvoke(Action action)
		{
			RealDispatcher.BeginInvoke(action);
		}

		public void BeginInvoke(Delegate action, params object[] arguments)
		{
			RealDispatcher.BeginInvoke(action, arguments);
		}

		public bool CheckAccess()
		{
			return RealDispatcher.CheckAccess();
		}
	}
}