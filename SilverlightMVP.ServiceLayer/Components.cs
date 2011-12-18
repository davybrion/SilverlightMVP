using Agatha.ServiceLayer;
using Castle.MicroKernel.Registration;
using SilverlightMVP.Common.RequestsAndResponses;
using SilverlightMVP.ServiceLayer.Domain;
using SilverlightMVP.ServiceLayer.ServiceLayer;

namespace SilverlightMVP.ServiceLayer
{
	public static class Components
	{
		public static void Register()
		{
			IoC.Container.Register(Component.For<IRepository>().ImplementedBy<Repository>().LifeStyle.Singleton);
			IoC.Container.Register(Component.For<IAuthorizationProvider>().ImplementedBy<AuthorizationProvider>().LifeStyle.Singleton);

			var containerWrapper = new Agatha.Castle.Container(IoC.Container);
			new ServiceLayerConfiguration(typeof(Components).Assembly, typeof(GetAllUserGroupsRequest).Assembly, containerWrapper).Initialize();
		}
	}
}