using System;
using SilverlightMVP.Common.Dtos;
using SilverlightMVP.ServiceLayer.Domain.Entities;

namespace SilverlightMVP.ServiceLayer.ServiceLayer
{
	public static class ExtensionMethods
	{
		public static UserGroupDto ToDto(this UserGroup userGroup)
		{
			return new UserGroupDto
			       {
			       	Id = userGroup.Id,
			       	Name = userGroup.Name,
			       	ParentId = userGroup.Parent == null ? (Guid?)null : userGroup.Parent.Id
			       };
		}
	}
}