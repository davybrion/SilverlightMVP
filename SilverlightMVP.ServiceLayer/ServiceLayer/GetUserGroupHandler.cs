using Agatha.Common;
using Agatha.ServiceLayer;
using SilverlightMVP.Common.RequestsAndResponses;
using SilverlightMVP.ServiceLayer.Domain;
using SilverlightMVP.ServiceLayer.Domain.Entities;

namespace SilverlightMVP.ServiceLayer.ServiceLayer
{
	public class GetUserGroupHandler : RequestHandler<GetUserGroupRequest, GetUserGroupResponse>
	{
		private readonly IRepository repository;

		public GetUserGroupHandler(IRepository repository)
		{
			this.repository = repository;
		}

		public override Response Handle(GetUserGroupRequest request)
		{
			var response = CreateTypedResponse();
			response.UserGroup = repository.Get<UserGroup>(request.UserGroupId).ToDto();
			return response;
		}
	}
}