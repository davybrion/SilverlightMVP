using System;
using System.Collections.Generic;
using System.Linq;
using Agatha.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Presenters;
using SilverlightMVP.Client.Views;
using SilverlightMVP.Common;
using SilverlightMVP.Common.Dtos;
using SilverlightMVP.Common.RequestsAndResponses;

namespace Silverlight.ClientTests.Presenters.UserGroupDetailPresenterTests
{
	[TestClass]
	public class HandleUserGroupSelectedEventFixture : PresenterFixture<UserGroupDetailPresenter, IUserGroupDetailsView>
	{
		protected override UserGroupDetailPresenter CreatePresenter()
		{
			return new UserGroupDetailPresenter(ViewMock.Object, EventAggregatorStub, RequestDispatcherFactoryStub);
		}

		[TestMethod]
		public void ClearsBindingModel()
		{
			Presenter.BindingModel.Id = Guid.NewGuid();
			Presenter.Handle(new UserGroupSelectedEvent(Guid.NewGuid()));
			Assert.IsNull(Presenter.BindingModel.Id);
		}

		[TestMethod]
		public void TellsViewToEnableEverything()
		{
			Presenter.Handle(new UserGroupSelectedEvent(Guid.NewGuid()));
			ViewMock.Verify(v => v.EnableEverything());
		}

		[TestMethod]
		public void ChecksPermissions()
		{
			Presenter.Handle(new UserGroupSelectedEvent(Guid.NewGuid()));
			var request = RequestDispatcherStub.GetRequest<CheckPermissionsRequest>();
			Assert.IsTrue(request.PermissionsToCheck.Contains(Permissions.EditUserGroup));
			Assert.IsTrue(request.PermissionsToCheck.Contains(Permissions.DeleteUserGroup));
		}

		[TestMethod]
		public void RetrievesUserGroupDetails()
		{
			var userGroupId = Guid.NewGuid();
			Presenter.Handle(new UserGroupSelectedEvent(userGroupId));
			Assert.AreEqual(userGroupId, RequestDispatcherStub.GetRequest<GetUserGroupRequest>().UserGroupId);
		}

		[TestMethod]
		public void RetrievesSuitableParentUserGroups()
		{
			var userGroupId = Guid.NewGuid();
			Presenter.Handle(new UserGroupSelectedEvent(userGroupId));
			Assert.AreEqual(userGroupId, RequestDispatcherStub.GetRequest<GetSuitableParentUserGroupsRequest>().UserGroupId.Value);
		}

		[TestMethod]
		public void PublishesEventIfRemoteExceptionOccurred()
		{
			var exception = new ExceptionInfo();
			Presenter.Handle(new UserGroupSelectedEvent(Guid.NewGuid()));
			RequestDispatcherStub.SetResponsesToReturn(new GetUserGroupResponse { Exception = exception });
			RequestDispatcherStub.ReturnResponses();
			Assert.AreEqual(exception, EventAggregatorStub.GetPublishedEvents<RemoteExceptionOccurredEvent>()[0].ExceptionInfo);
		}

		[TestMethod]
		public void ResponsesReceived_PopulatesModel()
		{
			var userGroup = new UserGroupDto { Id = Guid.NewGuid() };
			var suitableParents = new[] { new UserGroupDto { Id = Guid.NewGuid() } };

			Presenter.Handle(new UserGroupSelectedEvent(Guid.NewGuid()));
			RequestDispatcherStub.SetResponsesToReturn(new GetUserGroupResponse { UserGroup = userGroup },
													   new GetSuitableParentUserGroupsResponse { SuitableParentUserGroups = suitableParents });
			RequestDispatcherStub.ReturnResponses();

			Assert.AreEqual(userGroup.Id, Presenter.BindingModel.Id);
			Assert.AreEqual(suitableParents[0].Id, Presenter.BindingModel.SuitableParentUserGroups[1].Id);
		}

		[TestMethod]
		public void ResponsesReceived_TellsViewToPreventDeletionIfUserDoesntHavePermission()
		{
			Presenter.Handle(new UserGroupSelectedEvent(Guid.NewGuid()));
			RequestDispatcherStub.SetResponsesToReturn(new GetUserGroupResponse(),
			                                           new GetSuitableParentUserGroupsResponse {SuitableParentUserGroups = new UserGroupDto[0]},
			                                           new CheckPermissionsResponse
			                                           {
			                                           		AuthorizationResults = new Dictionary<Guid, bool> {{Permissions.DeleteUserGroup, false}, {Permissions.EditUserGroup, true}}
			                                           });
			RequestDispatcherStub.ReturnResponses();

			ViewMock.Verify(v => v.PreventDeletion());
		}

		[TestMethod]
		public void ResponsesReceived_DoesNotTellViewToPreventDeletionIfUserHasPermission()
		{
			Presenter.Handle(new UserGroupSelectedEvent(Guid.NewGuid()));
			RequestDispatcherStub.SetResponsesToReturn(new GetUserGroupResponse(),
			                                           new GetSuitableParentUserGroupsResponse {SuitableParentUserGroups = new UserGroupDto[0]},
			                                           new CheckPermissionsResponse
			                                           {
			                                           		AuthorizationResults = new Dictionary<Guid, bool> {{Permissions.DeleteUserGroup, true}, {Permissions.EditUserGroup, true}}
			                                           });
			RequestDispatcherStub.ReturnResponses();

			ViewMock.Verify(v => v.PreventDeletion(), Times.Never());
		}

		[TestMethod]
		public void ResponsesReceived_TellsViewToPreventModificationIfUserDoesntHavePermission()
		{
			Presenter.Handle(new UserGroupSelectedEvent(Guid.NewGuid()));
			RequestDispatcherStub.SetResponsesToReturn(new GetUserGroupResponse(),
			                                           new GetSuitableParentUserGroupsResponse {SuitableParentUserGroups = new UserGroupDto[0]},
			                                           new CheckPermissionsResponse
			                                           {
			                                           		AuthorizationResults = new Dictionary<Guid, bool> {{Permissions.DeleteUserGroup, true}, {Permissions.EditUserGroup, false}}
			                                           });
			RequestDispatcherStub.ReturnResponses();

			ViewMock.Verify(v => v.PreventModification());
		}

		[TestMethod]
		public void ResponsesReceived_DoesNotTellViewToPreventModificationIfUserHasPermission()
		{
			Presenter.Handle(new UserGroupSelectedEvent(Guid.NewGuid()));
			RequestDispatcherStub.SetResponsesToReturn(new GetUserGroupResponse(),
			                                           new GetSuitableParentUserGroupsResponse {SuitableParentUserGroups = new UserGroupDto[0]},
			                                           new CheckPermissionsResponse
			                                           {
			                                           		AuthorizationResults = new Dictionary<Guid, bool> {{Permissions.DeleteUserGroup, true}, {Permissions.EditUserGroup, true}}
			                                           });
			RequestDispatcherStub.ReturnResponses();

			ViewMock.Verify(v => v.PreventModification(), Times.Never());
		}

		[TestMethod]
		public void ResponsesReceived_ShowsTheView()
		{
			var userGroup = new UserGroupDto { Id = Guid.NewGuid() };
			var suitableParents = new[] { new UserGroupDto { Id = Guid.NewGuid() } };

			Presenter.Handle(new UserGroupSelectedEvent(Guid.NewGuid()));
			RequestDispatcherStub.SetResponsesToReturn(new GetUserGroupResponse { UserGroup = userGroup },
													   new GetSuitableParentUserGroupsResponse { SuitableParentUserGroups = suitableParents });
			RequestDispatcherStub.ReturnResponses();

			ViewMock.Verify(v => v.Show());
		}
	}
}