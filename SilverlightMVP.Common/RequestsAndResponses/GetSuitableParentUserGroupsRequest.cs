using System;
using Agatha.Common;
using SilverlightMVP.Common.Dtos;

namespace SilverlightMVP.Common.RequestsAndResponses
{
	public class GetSuitableParentUserGroupsRequest : Request
	{
		public Guid? UserGroupId { get; set; }
	}

	public class GetSuitableParentUserGroupsResponse : Response
	{
		public UserGroupDto[] SuitableParentUserGroups { get; set; }
	}
}