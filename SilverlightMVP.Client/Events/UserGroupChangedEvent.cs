using System;
using SilverlightMVP.Client.Infrastructure.Eventing;

namespace SilverlightMVP.Client.Events
{
	public class UserGroupChangedEvent : Event
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public Guid? ParentId { get; set; }
		public bool IsNew { get; set; }
	}
}