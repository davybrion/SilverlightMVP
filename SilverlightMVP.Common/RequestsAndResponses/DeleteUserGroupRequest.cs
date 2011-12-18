using System;
using Agatha.Common;

namespace SilverlightMVP.Common.RequestsAndResponses
{
	public class DeleteUserGroupRequest : Request
	{
		public Guid UserGroupId { get; set; }
	}

	public class DeleteUserGroupResponse : Response {}
}