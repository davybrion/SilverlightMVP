using System;
using System.Collections.Generic;
using Agatha.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Presenters;
using SilverlightMVP.Client.Views;
using SilverlightMVP.Common;
using SilverlightMVP.Common.Dtos;
using SilverlightMVP.Common.RequestsAndResponses;

namespace Silverlight.ClientTests.Presenters.UserGroupsPresenterTests
{
	[TestClass]
	public class InitializationFixture : PresenterFixture<UserGroupsPresenter, IUserGroupsView>
	{
		protected override UserGroupsPresenter CreatePresenter()
		{
			return new UserGroupsPresenter(ViewMock.Object, RequestDispatcherFactoryStub, EventAggregatorStub);
		}

		[TestMethod]
		public void SubscribesToUserGroupChangedEvent()
		{
			Presenter.Initialize();
			Assert.IsTrue(EventAggregatorStub.IsListenerSubscribedTo<UserGroupChangedEvent>(Presenter));
		}

		[TestMethod]
		public void SubscribesToUserGroupDeletedEvent()
		{
			Presenter.Initialize();
			Assert.IsTrue(EventAggregatorStub.IsListenerSubscribedTo<UserGroupDeletedEvent>(Presenter));
		}

		[TestMethod]
		public void PublishesEventIfRemoteExceptionOccurred()
		{
			var exception = new ExceptionInfo();
			Presenter.Initialize();
			RequestDispatcherStub.SetResponsesToReturn(new GetAllUserGroupsResponse { Exception = exception });
			RequestDispatcherStub.ReturnResponses();
			Assert.AreEqual(exception, EventAggregatorStub.GetPublishedEvents<RemoteExceptionOccurredEvent>()[0].ExceptionInfo);
		}

		[TestMethod]
		public void LoadsAllUserGroups()
		{
			Presenter.Initialize();
			Assert.IsTrue(RequestDispatcherStub.HasRequest<GetAllUserGroupsRequest>());
		}

		[TestMethod]
		public void PopulatesModelWithReceivedUserGroups()
		{
			RequestDispatcherStub.SetResponsesToReturn(new GetAllUserGroupsResponse { UserGroups = new[] { new UserGroupDto() } });
			Presenter.Initialize();
			RequestDispatcherStub.ReturnResponses();
			Assert.AreEqual(1, Presenter.BindingModel.UserGroups.Count);
		}

		[TestMethod]
		public void InstructsViewToExpandTreeViewWhenUserGroupsHaveBeenReceived()
		{
			RequestDispatcherStub.SetResponsesToReturn(new GetAllUserGroupsResponse { UserGroups = new[] { new UserGroupDto() } });
			Presenter.Initialize();
			RequestDispatcherStub.ReturnResponses();
			ViewMock.Verify(v => v.ExpandTreeView());
		}

		[TestMethod]
		public void ChecksPermission()
		{
			Presenter.Initialize();

			var request = RequestDispatcherStub.GetRequest<CheckPermissionsRequest>();

			Assert.AreEqual(1, request.PermissionsToCheck.Length);
			Assert.AreEqual(Permissions.CreateUserGroup, request.PermissionsToCheck[0]);
		}

		[TestMethod]
		public void HidesAddNewButtonOnViewIfUserDoesntHaveRequiredPermission()
		{
			RequestDispatcherStub.SetResponsesToReturn(new CheckPermissionsResponse
			{
				AuthorizationResults = new Dictionary<Guid, bool> { { Permissions.CreateUserGroup, false } }
			});
			Presenter.Initialize();
			RequestDispatcherStub.ReturnResponses();
			ViewMock.Verify(v => v.HideAddNewButton());
		}
	}
}