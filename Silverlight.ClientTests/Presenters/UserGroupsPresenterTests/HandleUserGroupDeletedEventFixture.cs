using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Presenters;
using SilverlightMVP.Client.Views;
using SilverlightMVP.Common.Dtos;

namespace Silverlight.ClientTests.Presenters.UserGroupsPresenterTests
{
	[TestClass]
	public class HandleUserGroupDeletedEventFixture : PresenterFixture<UserGroupsPresenter, IUserGroupsView>
	{
		protected override UserGroupsPresenter CreatePresenter()
		{
			return new UserGroupsPresenter(ViewMock.Object, RequestDispatcherFactoryStub, EventAggregatorStub);
		}

		[TestMethod]
		public void RemovesUserGroupFromBindingModel()
		{
			var id = Guid.NewGuid();
			Presenter.BindingModel.PopulateFrom(new[] {new UserGroupDto {Id = Guid.NewGuid()}, new UserGroupDto {Id = id}});
			Presenter.Handle(new UserGroupDeletedEvent(id));
			Assert.IsFalse(Presenter.BindingModel.UserGroups.Any(u => u.Id == id));
		}
	}
}