using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SilverlightMVP.Client.BindingModels;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Presenters;
using SilverlightMVP.Client.Views;
using SilverlightMVP.Common.Dtos;

namespace Silverlight.ClientTests.Presenters.UserGroupsPresenterTests
{
	[TestClass]
	public class HandleUserGroupChangedEventFixture : PresenterFixture<UserGroupsPresenter, IUserGroupsView>
	{
		protected override UserGroupsPresenter CreatePresenter()
		{
			return new UserGroupsPresenter(ViewMock.Object, RequestDispatcherFactoryStub, EventAggregatorStub);
		}

		[TestMethod]
		public void SelectsItemInTreeViewIfItsNewUserGroup()
		{
			var id = Guid.NewGuid();
			HierarchicalUserGroupBindingModel hierarchicalUserGroupBindingModel = null;

			ViewMock.Setup(v => v.SelectItemInTreeView(It.IsAny<HierarchicalUserGroupBindingModel>()))
				.Callback(new Action<HierarchicalUserGroupBindingModel>(m => hierarchicalUserGroupBindingModel = m));

			Presenter.Handle(new UserGroupChangedEvent {Id = id, IsNew = true});

			Assert.AreEqual(id, hierarchicalUserGroupBindingModel.Id);
		}

		[TestMethod]
		public void AddsUserGroupToBindingModelIfItsNewUserGroup()
		{
			var id = Guid.NewGuid();
			Presenter.Handle(new UserGroupChangedEvent {Id = id, IsNew = true});
			Assert.IsTrue(Presenter.BindingModel.UserGroups.Any(u => u.Id == id));
		}

		[TestMethod]
		public void UpdatesUserGroupInBindingModelIfItsExistingUserGroup()
		{
			var userGroup = new UserGroupDto {Id = Guid.NewGuid(), Name = "some name"};
			Presenter.BindingModel.PopulateFrom(new[] {userGroup});

			Presenter.Handle(new UserGroupChangedEvent { Id = userGroup.Id, Name = "some other name"});

			Assert.AreEqual("some other name", Presenter.BindingModel.UserGroups[0].Name);
		}
	}
}