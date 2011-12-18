using System;
using SilverlightMVP.ServiceLayer;

namespace SilverlightMVP.Web
{
	public class Global : System.Web.HttpApplication
	{
		void Application_Start(object sender, EventArgs e)
		{
			Components.Register();
		}
	}
}
