using System;
using SilverlightMVP.Client.Infrastructure.Eventing;

namespace SilverlightMVP.Client.Events
{
	public class UserGroupSelectedEvent : Event
	{
		public UserGroupSelectedEvent(Guid selectedUserGroupId)
		{
			SelectedUserGroupId = selectedUserGroupId;
		}

		public Guid SelectedUserGroupId { get; private set; }
	}
}