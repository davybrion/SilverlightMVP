using System.Collections.Generic;
using System.Linq;
using Agatha.Common;
using Agatha.ServiceLayer;
using SilverlightMVP.Common.RequestsAndResponses;
using SilverlightMVP.ServiceLayer.Domain;
using SilverlightMVP.ServiceLayer.Domain.Entities;

namespace SilverlightMVP.ServiceLayer.ServiceLayer
{
	public class GetSuitableParentUserGroupsHandler : RequestHandler<GetSuitableParentUserGroupsRequest, GetSuitableParentUserGroupsResponse>
	{
		private readonly IRepository repository;

		public GetSuitableParentUserGroupsHandler(IRepository repository)
		{
			this.repository = repository;
		}

		public override Response Handle(GetSuitableParentUserGroupsRequest request)
		{
			var response = CreateTypedResponse();

			var allUserGroups = repository.GetAll<UserGroup>();

			if (request.UserGroupId.HasValue)
			{
				var childrenToExclude = GetAllChildrenOf(repository.Get<UserGroup>(request.UserGroupId.Value));

				response.SuitableParentUserGroups = allUserGroups
					.Where(u => u.Id != request.UserGroupId.Value)
					.Except(childrenToExclude)
					.Select(t => t.ToDto())
					.ToArray();
			}
			else
			{
				response.SuitableParentUserGroups = allUserGroups.Select(u => u.ToDto()).ToArray();
			}

			return response;
		}

		private IEnumerable<UserGroup> GetAllChildrenOf(UserGroup userGroup)
		{
			var list = new List<UserGroup>();
			AddAllChildrenToList(userGroup, list);
			return list;
		}

		private void AddAllChildrenToList(UserGroup userGroup, List<UserGroup> list)
		{
			foreach (var child in userGroup.Children)
			{
				list.Add(child);
				AddAllChildrenToList(child, list);
			}
		}
	}
}