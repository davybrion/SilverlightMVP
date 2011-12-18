using System.Linq;
using Agatha.Common;
using Agatha.ServiceLayer;
using SilverlightMVP.Common;
using SilverlightMVP.Common.RequestsAndResponses;
using SilverlightMVP.ServiceLayer.Domain;
using SilverlightMVP.ServiceLayer.Domain.Entities;

namespace SilverlightMVP.ServiceLayer.ServiceLayer
{
	public class DeleteUserGroupHandler : RequestHandler<DeleteUserGroupRequest, DeleteUserGroupResponse>
	{
		private readonly IRepository repository;
		private readonly IAuthorizationProvider authorizationProvider;

		public DeleteUserGroupHandler(IRepository repository, IAuthorizationProvider authorizationProvider)
		{
			this.repository = repository;
			this.authorizationProvider = authorizationProvider;
		}

		public override Response Handle(DeleteUserGroupRequest request)
		{
			authorizationProvider.RequirePermission(Permissions.DeleteUserGroup);

			var userGroup = repository.Get<UserGroup>(request.UserGroupId);
			DeleteChildren(userGroup);
			repository.Remove(userGroup);

			return CreateTypedResponse();
		}

		private void DeleteChildren(UserGroup userGroup)
		{
			foreach (var child in userGroup.Children)
			{
				userGroup.RemoveChildGroup(child);
				DeleteChildren(child);
				repository.Remove(child);
			}
		}
	}
}