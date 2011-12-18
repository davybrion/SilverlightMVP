using System;
using Agatha.Common;
using SilverlightMVP.Common.Dtos;

namespace SilverlightMVP.Common.RequestsAndResponses
{
	public class SaveUserGroupRequest : Request
	{
		public Guid? Id { get; set; }
		public string Name { get; set; }
		public Guid? ParentId { get; set; }
	}
	public class SaveUserGroupResponse : Response
	{
		public Guid? NewUserGroupId { get; set; }
	}
}