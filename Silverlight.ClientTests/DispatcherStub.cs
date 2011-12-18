using System;
using SilverlightMVP.Client.Infrastructure;

namespace Silverlight.ClientTests
{
	public class DispatcherStub : IDispatcher
	{
		public void BeginInvoke(Action action)
		{
			action();
		}

		public void BeginInvoke(Delegate action, params object[] arguments)
		{
			action.DynamicInvoke(arguments);
		}

		public bool CheckAccess()
		{
			return true;
		}
	}
}