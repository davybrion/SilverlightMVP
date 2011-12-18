using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Presenters;
using SilverlightMVP.Client.Views;

namespace Silverlight.ClientTests.Presenters.UserGroupsPresenterTests
{
	[TestClass]
	public class PrepareUserGroupCreationFixture : PresenterFixture<UserGroupsPresenter, IUserGroupsView>
	{
		protected override UserGroupsPresenter CreatePresenter()
		{
			return new UserGroupsPresenter(ViewMock.Object, RequestDispatcherFactoryStub, EventAggregatorStub);
		}

		[TestMethod]
		public void PublishesEvent()
		{
			Presenter.PrepareUserGroupCreation();
			Assert.AreEqual(1, EventAggregatorStub.GetPublishedEvents<UserGroupNeedsToBeCreatedEvent>().Length);
		}
	}
}