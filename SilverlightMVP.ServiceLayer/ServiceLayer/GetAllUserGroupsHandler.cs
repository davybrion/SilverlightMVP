using System;
using System.Linq;
using Agatha.Common;
using Agatha.ServiceLayer;
using SilverlightMVP.Common.Dtos;
using SilverlightMVP.Common.RequestsAndResponses;
using SilverlightMVP.ServiceLayer.Domain;
using SilverlightMVP.ServiceLayer.Domain.Entities;

namespace SilverlightMVP.ServiceLayer.ServiceLayer
{
	public class GetAllUserGroupsHandler : RequestHandler<GetAllUserGroupsRequest, GetAllUserGroupsResponse>
	{
		private readonly IRepository repository;

		public GetAllUserGroupsHandler(IRepository repository)
		{
			this.repository = repository;
		}

		public override Response Handle(GetAllUserGroupsRequest request)
		{
			var response = CreateTypedResponse();
			response.UserGroups = repository.GetAll<UserGroup>().Select(g => g.ToDto()).ToArray();
			return response;
		}
	}
}