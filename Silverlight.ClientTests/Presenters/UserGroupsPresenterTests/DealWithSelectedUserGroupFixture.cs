using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.BindingModels;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Presenters;
using SilverlightMVP.Client.Views;

namespace Silverlight.ClientTests.Presenters.UserGroupsPresenterTests
{
	[TestClass]
	public class DealWithSelectedUserGroupFixture : PresenterFixture<UserGroupsPresenter, IUserGroupsView>
	{
		protected override UserGroupsPresenter CreatePresenter()
		{
			return new UserGroupsPresenter(ViewMock.Object, RequestDispatcherFactoryStub, EventAggregatorStub);
		}

		[TestMethod]
		public void DealWithSelectedUserGroup_PublishesEvent()
		{
			var userGroup = new HierarchicalUserGroupBindingModel { Id = Guid.NewGuid() };
			Presenter.DealWithSelectedUserGroup(userGroup);
			Assert.AreEqual(userGroup.Id, EventAggregatorStub.GetPublishedEvents<UserGroupSelectedEvent>()[0].SelectedUserGroupId);
		}
	}
}