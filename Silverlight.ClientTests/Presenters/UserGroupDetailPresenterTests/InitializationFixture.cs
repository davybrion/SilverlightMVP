using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Presenters;
using SilverlightMVP.Client.Views;

namespace Silverlight.ClientTests.Presenters.UserGroupDetailPresenterTests
{
	[TestClass]
	public class InitializationFixture : PresenterFixture<UserGroupDetailPresenter, IUserGroupDetailsView>
	{
		protected override UserGroupDetailPresenter CreatePresenter()
		{
			return new UserGroupDetailPresenter(ViewMock.Object, EventAggregatorStub, RequestDispatcherFactoryStub);
		}

		[TestMethod]
		public void Initialize_HidesTheView()
		{
			Presenter.Initialize();
			ViewMock.Verify(v => v.Hide());
		}

		[TestMethod]
		public void Initialize_SubcribesToUserGroupSelectedEvent()
		{
			Presenter.Initialize();
			Assert.IsTrue(EventAggregatorStub.IsListenerSubscribedTo<UserGroupSelectedEvent>(Presenter));
		}

		[TestMethod]
		public void Initialize_SubscribesToUserGroupNeedsToBeCreatedEvent()
		{
			Presenter.Initialize();
			Assert.IsTrue(EventAggregatorStub.IsListenerSubscribedTo<UserGroupNeedsToBeCreatedEvent>(Presenter));
		}
	}
}