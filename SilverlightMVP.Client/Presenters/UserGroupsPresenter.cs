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
	public class UserGroupsPresenter : Presenter<IUserGroupsView, UserGroupsBindingModel>,
		IListenTo<UserGroupChangedEvent>, IListenTo<UserGroupDeletedEvent>
	{
		public UserGroupsPresenter(IUserGroupsView view, IAsyncRequestDispatcherFactory requestDispatcherFactory, IEventAggregator eventAggregator) 
			: base(view, eventAggregator, requestDispatcherFactory) {}

		public override void Initialize()
		{
			EventAggregator.Subscribe(this);

			var requestDispatcher = RequestDispatcherFactory.CreateAsyncRequestDispatcher();
			requestDispatcher.Add(new CheckPermissionsRequest { PermissionsToCheck = new[] { Permissions.CreateUserGroup } });
			requestDispatcher.Add(new GetAllUserGroupsRequest());
			requestDispatcher.ProcessRequests(ResponsesReceived, PublishRemoteException);
		}

		private void ResponsesReceived(ReceivedResponses receivedResponses)
		{
			if (receivedResponses.HasResponse<GetAllUserGroupsResponse>())
			{
				BindingModel.PopulateFrom(receivedResponses.Get<GetAllUserGroupsResponse>().UserGroups);
				View.ExpandTreeView();
			}

			if (receivedResponses.HasResponse<CheckPermissionsResponse>() &&
			    !receivedResponses.Get<CheckPermissionsResponse>().AuthorizationResults[Permissions.CreateUserGroup])
			{
				View.HideAddNewButton();
			}
		}

		public void DealWithSelectedUserGroup(HierarchicalUserGroupBindingModel selectedUserGroup)
		{
			EventAggregator.Publish(new UserGroupSelectedEvent(selectedUserGroup.Id));	
		}

		public void PrepareUserGroupCreation()
		{
			EventAggregator.Publish(new UserGroupNeedsToBeCreatedEvent());
		}

		public void Handle(UserGroupChangedEvent receivedEvent)
		{
			if (receivedEvent.IsNew)
			{
				View.SelectItemInTreeView(BindingModel.AddUserGroup(receivedEvent.Id, receivedEvent.Name, receivedEvent.ParentId));
			}
			else
			{
				BindingModel.UpdateUserGroup(receivedEvent.Id, receivedEvent.Name, receivedEvent.ParentId);
			}
		}

		public void Handle(UserGroupDeletedEvent receivedEvent)
		{
			BindingModel.RemoveUserGroup(receivedEvent.UserGroupId);
		}
	}
}