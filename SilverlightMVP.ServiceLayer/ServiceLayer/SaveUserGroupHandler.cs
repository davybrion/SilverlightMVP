using System;
using Agatha.Common;
using Agatha.ServiceLayer;
using SilverlightMVP.Common;
using SilverlightMVP.Common.RequestsAndResponses;
using SilverlightMVP.ServiceLayer.Domain;
using SilverlightMVP.ServiceLayer.Domain.Entities;

namespace SilverlightMVP.ServiceLayer.ServiceLayer
{
	public class SaveUserGroupHandler : RequestHandler<SaveUserGroupRequest, SaveUserGroupResponse>
	{
		private readonly IRepository repository;
		private readonly IAuthorizationProvider authorizationProvider;

		public SaveUserGroupHandler(IRepository repository, IAuthorizationProvider authorizationProvider)
		{
			this.repository = repository;
			this.authorizationProvider = authorizationProvider;
		}

		public override Response Handle(SaveUserGroupRequest request)
		{
			var response = CreateTypedResponse();
			UserGroup userGroup = null;

			if (request.Id.HasValue)
			{
				authorizationProvider.RequirePermission(Permissions.EditUserGroup);
				userGroup = repository.Get<UserGroup>(request.Id.Value);
				userGroup.Name = request.Name;
			}
			else
			{
				authorizationProvider.RequirePermission(Permissions.CreateUserGroup);
				userGroup = new UserGroup(request.Name);
				repository.Attach(userGroup);
				response.NewUserGroupId = userGroup.Id;
			}

			if (userGroup.Parent == null)
			{
				if (request.ParentId.HasValue)
				{
					repository.Get<UserGroup>(request.ParentId.Value).AddChildGroup(userGroup);
				}
			}
			else
			{
				if (request.ParentId.HasValue && userGroup.Parent.Id != request.ParentId.Value)
				{
					userGroup.Parent.RemoveChildGroup(userGroup);
					repository.Get<UserGroup>(request.ParentId.Value).AddChildGroup(userGroup);
				}
				else if (!request.ParentId.HasValue)
				{
					userGroup.Parent.RemoveChildGroup(userGroup);
				}
			}

			return response;
		}
	}
}