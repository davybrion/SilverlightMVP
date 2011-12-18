using System;
using Agatha.Common;
using SilverlightMVP.Client.BindingModels;
using SilverlightMVP.Client.Events;
using SilverlightMVP.Client.Infrastructure.Eventing;
using SilverlightMVP.Client.Infrastructure.MVP;
using SilverlightMVP.Client.Views;
using SilverlightMVP.Common;
using SilverlightMVP.Common.RequestsAndResponses;

namespace SilverlightMVP.Client.Presenters
{
	public class UserGroupDetailPresenter : Presenter<IUserGroupDetailsView, UserGroupDetailBindingModel>, 
		IListenTo<UserGroupSelectedEvent>, IListenTo<UserGroupNeedsToBeCreatedEvent>
	{
		public UserGroupDetailPresenter(IUserGroupDetailsView view, IEventAggregator eventAggregator, IAsyncRequestDispatcherFactory requestDispatcherFactory) 
			: base(view, eventAggregator, requestDispatcherFactory) {}

		public override void Initialize()
		{
			View.Hide();
			EventAggregator.Subscribe(this);
		}

		public void Handle(UserGroupNeedsToBeCreatedEvent receivedEvent)
		{
			View.PreventDeletion();
			LoadData();
		}

		public void Handle(UserGroupSelectedEvent receivedEvent)
		{
			View.EnableEverything();
			LoadData(receivedEvent.SelectedUserGroupId);
		}

		private void LoadData(Guid? userGroupId = null) 
		{
			BindingModel.Clear();

			var requestDispatcher = RequestDispatcherFactory.CreateAsyncRequestDispatcher();

			if (userGroupId.HasValue)
			{
				requestDispatcher.Add(new CheckPermissionsRequest {PermissionsToCheck = new[] {Permissions.DeleteUserGroup, Permissions.EditUserGroup}});
				requestDispatcher.Add(new GetUserGroupRequest { UserGroupId = userGroupId.Value });
			}
			requestDispatcher.Add(new GetSuitableParentUserGroupsRequest {UserGroupId = userGroupId});
			requestDispatcher.ProcessRequests(ResponsesReceived, PublishRemoteException);
		}

		private void ResponsesReceived(ReceivedResponses receivedResponses)
		{
			if (receivedResponses.HasResponse<GetUserGroupResponse>())
			{
				BindingModel.Populate(receivedResponses.Get<GetSuitableParentUserGroupsResponse>().SuitableParentUserGroups, 
					receivedResponses.Get<GetUserGroupResponse>().UserGroup);
			}
			else
			{
				BindingModel.Populate(receivedResponses.Get<GetSuitableParentUserGroupsResponse>().SuitableParentUserGroups);
			}

			if (receivedResponses.HasResponse<CheckPermissionsResponse>())
			{
				var response = receivedResponses.Get<CheckPermissionsResponse>();
				if (!response.AuthorizationResults[Permissions.DeleteUserGroup]) View.PreventDeletion();
				if (!response.AuthorizationResults[Permissions.EditUserGroup]) View.PreventModification();
			}

			View.Show();
		}

		public void PersistChanges()
		{
			BindingModel.ValidateAll();
			if (BindingModel.HasErrors) return; 

			var dispatcher = RequestDispatcherFactory.CreateAsyncRequestDispatcher();
			dispatcher.Add(new SaveUserGroupRequest
			{
				Id = BindingModel.Id,
				Name = BindingModel.Name,
				ParentId = BindingModel.SelectedParentUserGroup.Id != Guid.Empty ? BindingModel.SelectedParentUserGroup.Id : (Guid?)null
			});
			dispatcher.ProcessRequests(PersistChanges_ResponseReceived, PublishRemoteException);
		}

		private void PersistChanges_ResponseReceived(ReceivedResponses responses)
		{
			var response = responses.Get<SaveUserGroupResponse>();

			if (response.NewUserGroupId.HasValue)
			{
				BindingModel.Id = response.NewUserGroupId.Value;
			}

			EventAggregator.Publish(new UserGroupChangedEvent
			{
				Id = BindingModel.Id.Value,
				Name = BindingModel.Name,
				ParentId = BindingModel.SelectedParentUserGroup.Id != Guid.Empty ? BindingModel.SelectedParentUserGroup.Id : (Guid?)null,
				IsNew = response.NewUserGroupId.HasValue
			});
		}

		public void Delete()
		{
			var dispatcher = RequestDispatcherFactory.CreateAsyncRequestDispatcher();
			dispatcher.Add(new DeleteUserGroupRequest { UserGroupId = BindingModel.Id.Value });
			dispatcher.ProcessRequests(DeleteUserGroup_ResponseReceived, PublishRemoteException);
		}

		private void DeleteUserGroup_ResponseReceived(ReceivedResponses responses)
		{
			EventAggregator.Publish(new UserGroupDeletedEvent(BindingModel.Id.Value));
		}

		public void Cancel()
		{
			BindingModel.RevertToOriginalValues();
		}
	}
}	