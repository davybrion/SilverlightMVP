using System;
using Agatha.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Presenters;
using SilverlightMVP.Client.Views;
using SilverlightMVP.Common.RequestsAndResponses;

namespace Silverlight.ClientTests.Presenters.UserGroupDetailPresenterTests
{
	[TestClass]
	public class DeleteFixture : PresenterFixture<UserGroupDetailPresenter, IUserGroupDetailsView>
	{
		protected override UserGroupDetailPresenter CreatePresenter()
		{
			return new UserGroupDetailPresenter(ViewMock.Object, EventAggregatorStub, RequestDispatcherFactoryStub);
		}

		[TestMethod]
		public void SendsCorrectRequest()
		{
			Presenter.BindingModel.Id = Guid.NewGuid();
			Presenter.Delete();
			Assert.AreEqual(Presenter.BindingModel.Id.Value, RequestDispatcherStub.GetRequest<DeleteUserGroupRequest>().UserGroupId);
		}

		[TestMethod]
		public void PublishesEventIfRemoteExceptionOccurred()
		{
			Presenter.BindingModel.Id = Guid.NewGuid();
			var exception = new ExceptionInfo();
			RequestDispatcherStub.SetResponsesToReturn(new DeleteUserGroupResponse {Exception = exception});

			Presenter.Delete();
			RequestDispatcherStub.ReturnResponses();

			Assert.AreEqual(exception, EventAggregatorStub.GetPublishedEvents<RemoteExceptionOccurredEvent>()[0].ExceptionInfo);
		}

		[TestMethod]
		public void PublishesUserGroupDeletedEventWhenResponseIsReturned()
		{
			Presenter.BindingModel.Id = Guid.NewGuid();
			RequestDispatcherStub.SetResponsesToReturn(new DeleteUserGroupResponse());

			Presenter.Delete();
			RequestDispatcherStub.ReturnResponses();

			Assert.AreEqual(Presenter.BindingModel.Id.Value, EventAggregatorStub.GetPublishedEvents<UserGroupDeletedEvent>()[0].UserGroupId);
		}
	}
}