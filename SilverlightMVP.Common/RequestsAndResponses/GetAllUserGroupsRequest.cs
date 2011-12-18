using Agatha.Common;
using SilverlightMVP.Common.Dtos;

namespace SilverlightMVP.Common.RequestsAndResponses
{
	public class GetAllUserGroupsRequest : Request {}

	public class GetAllUserGroupsResponse : Response
	{
		public UserGroupDto[] UserGroups { get; set; }
	}
}