using System;
using Agatha.Common;
using SilverlightMVP.Common.Dtos;

namespace SilverlightMVP.Common.RequestsAndResponses
{
	public class GetUserGroupRequest : Request
	{
		public Guid UserGroupId { get; set; }
	}

	public class GetUserGroupResponse : Response
	{
		public UserGroupDto UserGroup { get; set; }
	}
}