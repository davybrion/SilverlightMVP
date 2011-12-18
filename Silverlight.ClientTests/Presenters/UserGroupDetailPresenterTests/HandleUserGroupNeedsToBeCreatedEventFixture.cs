using System;
using System.Linq;
using Agatha.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Presenters;
using SilverlightMVP.Client.Views;
using SilverlightMVP.Common;
using SilverlightMVP.Common.Dtos;
using SilverlightMVP.Common.RequestsAndResponses;

namespace Silverlight.ClientTests.Presenters.UserGroupDetailPresenterTests
{
	[TestClass]
	public class HandleUserGroupNeedsToBeCreatedEventFixture : PresenterFixture<UserGroupDetailPresenter, IUserGroupDetailsView>
	{
		protected override UserGroupDetailPresenter CreatePresenter()
		{
			return new UserGroupDetailPresenter(ViewMock.Object, EventAggregatorStub, RequestDispatcherFactoryStub);
		}

		[TestMethod]
		public void ClearsBindingModel()
		{
			Presenter.BindingModel.Id = Guid.NewGuid();
			Presenter.Handle(new UserGroupNeedsToBeCreatedEvent());
			Assert.IsNull(Presenter.BindingModel.Id);
		}

		[TestMethod]
		public void TellsViewToPreventDeletion()
		{
			Presenter.Handle(new UserGroupNeedsToBeCreatedEvent());
			ViewMock.Verify(v => v.PreventDeletion());
		}

		[TestMethod]
		public void DoesNotRetrieveUserGroupDetails()
		{
			Presenter.Handle(new UserGroupNeedsToBeCreatedEvent());
			Assert.IsFalse(RequestDispatcherStub.HasRequest<GetUserGroupRequest>());
		}

		[TestMethod]
		public void DoesNotRetrievePermissionsForEditingOrDeleting()
		{
			Presenter.Handle(new UserGroupNeedsToBeCreatedEvent());
			Assert.IsFalse(RequestDispatcherStub.HasRequest<CheckPermissionsRequest>());
		}

		[TestMethod]
		public void RetrievesSuitableParentUserGroups()
		{
			Presenter.Handle(new UserGroupNeedsToBeCreatedEvent());
			Assert.IsNull(RequestDispatcherStub.GetRequest<GetSuitableParentUserGroupsRequest>().UserGroupId);
		}

		[TestMethod]
		public void PublishesEventIfRemoteExceptionOccurred()
		{
			var exception = new ExceptionInfo();
			Presenter.Handle(new UserGroupNeedsToBeCreatedEvent());
			RequestDispatcherStub.SetResponsesToReturn(new GetSuitableParentUserGroupsResponse { Exception = exception });
			RequestDispatcherStub.ReturnResponses();
			Assert.AreEqual(exception, EventAggregatorStub.GetPublishedEvents<RemoteExceptionOccurredEvent>()[0].ExceptionInfo);
		}

		[TestMethod]
		public void ResponseReceived_PopulatesModel()
		{
			var suitableParents = new[] { new UserGroupDto { Id = Guid.NewGuid() } };

			Presenter.Handle(new UserGroupNeedsToBeCreatedEvent());
			RequestDispatcherStub.SetResponsesToReturn(new GetSuitableParentUserGroupsResponse { SuitableParentUserGroups = suitableParents });
			RequestDispatcherStub.ReturnResponses();

			Assert.AreEqual(suitableParents[0].Id, Presenter.BindingModel.SuitableParentUserGroups[1].Id);
		}

		[TestMethod]
		public void ResponsesReceived_ShowsTheView()
		{
			var suitableParents = new[] { new UserGroupDto { Id = Guid.NewGuid() } };

			Presenter.Handle(new UserGroupNeedsToBeCreatedEvent());
			RequestDispatcherStub.SetResponsesToReturn(new GetSuitableParentUserGroupsResponse { SuitableParentUserGroups = suitableParents });
			RequestDispatcherStub.ReturnResponses();

			ViewMock.Verify(v => v.Show());
		}
	}
}