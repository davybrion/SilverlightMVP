using Agatha.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Silverlight.ClientTests
{
	public abstract class PresenterFixture<TPresenter, TView> where TView : class
	{
		protected TPresenter Presenter { get; private set; }
		protected Mock<TView> ViewMock { get; private set; }
		protected AsyncRequestDispatcherFactoryStub RequestDispatcherFactoryStub { get; private set; }
		protected AsyncRequestDispatcherStub RequestDispatcherStub { get; private set; }
		protected EventAggregatorStub EventAggregatorStub { get; private set; }

		[TestInitialize]
		public void SetUp()
		{
			RequestDispatcherStub = new AsyncRequestDispatcherStub();
			RequestDispatcherFactoryStub = new AsyncRequestDispatcherFactoryStub(RequestDispatcherStub);
			EventAggregatorStub = new EventAggregatorStub();
			ViewMock = new Mock<TView>();
			Presenter = CreatePresenter();
		}

		protected abstract TPresenter CreatePresenter();
	}
}