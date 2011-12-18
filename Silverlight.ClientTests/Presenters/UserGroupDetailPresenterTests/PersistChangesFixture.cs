using System;
using Agatha.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Presenters;
using SilverlightMVP.Client.Views;
using SilverlightMVP.Common.Dtos;
using SilverlightMVP.Common.RequestsAndResponses;

namespace Silverlight.ClientTests.Presenters.UserGroupDetailPresenterTests
{
	[TestClass]
	public class PersistChangesFixture : PresenterFixture<UserGroupDetailPresenter, IUserGroupDetailsView>
	{
		protected override UserGroupDetailPresenter CreatePresenter()
		{
			return new UserGroupDetailPresenter(ViewMock.Object, EventAggregatorStub, RequestDispatcherFactoryStub);
		}

		[TestMethod]
		public void DoesNotProceedIfModelIsInvalid()
		{
			Presenter.BindingModel.Name = null;
			Presenter.PersistChanges();
			Assert.IsFalse(RequestDispatcherStub.HasRequest<SaveUserGroupRequest>());
		}

		[TestMethod]
		public void SendsCorrectRequestIfModelIsValid()
		{
			var suitableParents = new[] {new UserGroupDto {Id = Guid.NewGuid()}};
			var userGroup = new UserGroupDto {Id = Guid.NewGuid(), Name = "some group", ParentId = suitableParents[0].Id};
			Presenter.BindingModel.Populate(suitableParents, userGroup);

			Presenter.PersistChanges();

			var request = RequestDispatcherStub.GetRequest<SaveUserGroupRequest>();
			Assert.AreEqual(userGroup.Id, request.Id);
			Assert.AreEqual(userGroup.Name, request.Name);
			Assert.AreEqual(userGroup.ParentId.Value, request.ParentId.Value);
		}

		[TestMethod]
		public void PublishesEventIfRemoteExceptionOccurred()
		{
			var userGroup = new UserGroupDto {Id = Guid.NewGuid(), Name = "some group"};
			Presenter.BindingModel.Populate(new UserGroupDto[0], userGroup);
			var exception = new ExceptionInfo();
			RequestDispatcherStub.SetResponsesToReturn(new SaveUserGroupResponse {Exception = exception});

			Presenter.PersistChanges();
			RequestDispatcherStub.ReturnResponses();

			Assert.AreEqual(exception, EventAggregatorStub.GetPublishedEvents<RemoteExceptionOccurredEvent>()[0].ExceptionInfo);
		}

		[TestMethod]
		public void ResponseReceivedForNewUserGroup_UpdatesUserGroupIdInBindingModel()
		{
			var userGroup = new UserGroupDto { Id = Guid.NewGuid(), Name = "some group" };
			Presenter.BindingModel.Populate(new UserGroupDto[0], userGroup);
			var newId = Guid.NewGuid();
			RequestDispatcherStub.SetResponsesToReturn(new SaveUserGroupResponse {NewUserGroupId = newId});

			Presenter.PersistChanges();
			RequestDispatcherStub.ReturnResponses();

			Assert.AreEqual(newId, Presenter.BindingModel.Id.Value);
		}

		[TestMethod]
		public void ResponseReceivedForNewUserGroup_PublishesCorrectEvent()
		{
			var suitableParents = new[] { new UserGroupDto { Id = Guid.NewGuid() } };
			var userGroup = new UserGroupDto { Name = "some group", ParentId = suitableParents[0].Id };
			Presenter.BindingModel.Populate(suitableParents, userGroup);
			var newId = Guid.NewGuid();
			RequestDispatcherStub.SetResponsesToReturn(new SaveUserGroupResponse { NewUserGroupId = newId });
			
			Presenter.PersistChanges();
			RequestDispatcherStub.ReturnResponses();

			var publishedEvent = EventAggregatorStub.GetPublishedEvents<UserGroupChangedEvent>()[0];
			Assert.AreEqual(newId, publishedEvent.Id);
			Assert.AreEqual(userGroup.Name, publishedEvent.Name);
			Assert.AreEqual(userGroup.ParentId.Value, publishedEvent.ParentId.Value);
			Assert.IsTrue(publishedEvent.IsNew);
		}

		[TestMethod]
		public void ResponseReceivedForExistingUserGroup_PublishesCorrectEvent()
		{
			var suitableParents = new[] { new UserGroupDto { Id = Guid.NewGuid() } };
			var userGroup = new UserGroupDto { Id = Guid.NewGuid(), Name = "some group", ParentId = suitableParents[0].Id };
			Presenter.BindingModel.Populate(suitableParents, userGroup);
			RequestDispatcherStub.SetResponsesToReturn(new SaveUserGroupResponse());

			Presenter.PersistChanges();
			RequestDispatcherStub.ReturnResponses();

			var publishedEvent = EventAggregatorStub.GetPublishedEvents<UserGroupChangedEvent>()[0];
			Assert.AreEqual(userGroup.Id, publishedEvent.Id);
			Assert.AreEqual(userGroup.Name, publishedEvent.Name);
			Assert.AreEqual(userGroup.ParentId.Value, publishedEvent.ParentId.Value);
			Assert.IsFalse(publishedEvent.IsNew);
		}
	}
}