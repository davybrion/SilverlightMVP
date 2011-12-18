using System;
using System.Collections.Generic;
using Agatha.Common;

namespace SilverlightMVP.Common.RequestsAndResponses
{
	public class CheckPermissionsRequest : Request
	{
		public Guid[] PermissionsToCheck { get; set; }
	}

	public class CheckPermissionsResponse : Response
	{
		public Dictionary<Guid, bool> AuthorizationResults { get; set; }
	}
}