using System;

namespace SilverlightMVP.Common.Dtos
{
	public class UserGroupDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public Guid? ParentId { get; set; }
	}
}