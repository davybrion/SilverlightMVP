using System.Reflection;
using Agatha.Common;
using Castle.MicroKernel.Registration;
using SilverlightMVP.Client.Infrastructure;
using SilverlightMVP.Client.Infrastructure.Eventing;
using SilverlightMVP.Client.Presenters;

namespace SilverlightMVP.Client
{
	public static class Components
	{
		public static void Register()
		{
			new ClientConfiguration(Assembly.GetExecutingAssembly(), new Agatha.Castle.Container(IoC.Container)).Initialize();

			IoC.Container.Register(Component.For<UserGroupsPresenter>().LifeStyle.Transient);
			IoC.Container.Register(Component.For<UserGroupDetailPresenter>().LifeStyle.Transient);

			IoC.Container.Register(Component.For<IDispatcher>().ImplementedBy<DispatcherWrapper>().LifeStyle.Singleton);
			IoC.Container.Register(Component.For<IEventAggregator>().ImplementedBy<EventAggregator>().LifeStyle.Singleton);
		}
	}
}