using System;
using System.Collections.Generic;
using System.Security;
using SilverlightMVP.Common;

namespace SilverlightMVP.ServiceLayer.ServiceLayer
{
	public interface IAuthorizationProvider
	{
		void RequirePermission(Guid permission);
		bool HasPermission(Guid permission);
	}

	public class AuthorizationProvider : IAuthorizationProvider
	{
		// change these boolean values to change the permissions for the 'current user' (which we don't have in this sample, but you get the idea)
		private static Dictionary<Guid, bool> permissions = new Dictionary<Guid, bool>
		                                                    {
		                                                    	{Permissions.CreateUserGroup, true},
		                                                    	{Permissions.EditUserGroup, true},
		                                                    	{Permissions.DeleteUserGroup, true},
		                                                    	{Permissions.ModifyUserGroupComposition, true}
		                                                    };

		public void RequirePermission(Guid permission)
		{
			if (!permissions[permission])
			{
				throw new SecurityException("You're not authorized to perform this operation");
			}
		}

		public bool HasPermission(Guid permission)
		{
			return permissions[permission];
		}
	}
}