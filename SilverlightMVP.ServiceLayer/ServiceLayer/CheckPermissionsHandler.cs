using System;
using System.Collections.Generic;
using Agatha.Common;
using Agatha.ServiceLayer;
using SilverlightMVP.Common.RequestsAndResponses;

namespace SilverlightMVP.ServiceLayer.ServiceLayer
{
	public class CheckPermissionsHandler : RequestHandler<CheckPermissionsRequest, CheckPermissionsResponse>
	{
		private readonly IAuthorizationProvider authorizationProvider;

		public CheckPermissionsHandler(IAuthorizationProvider authorizationProvider)
		{
			this.authorizationProvider = authorizationProvider;
		}

		public override Response Handle(CheckPermissionsRequest request)
		{
			var response = CreateTypedResponse();

			response.AuthorizationResults = new Dictionary<Guid, bool>();

			foreach (var permission in request.PermissionsToCheck)
			{
				response.AuthorizationResults[permission] = authorizationProvider.HasPermission(permission);
			}

			return response;
		}
	}
}