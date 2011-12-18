using System;
using SilverlightMVP.Client.Infrastructure.Eventing;

namespace SilverlightMVP.Client.Events
{
	public class UserGroupDeletedEvent : Event
	{
		public UserGroupDeletedEvent(Guid userGroupId)
		{
			UserGroupId = userGroupId;
		}

		public Guid UserGroupId { get; private set; }
	}
}