using System.IdentityModel.Configuration;
using System.Reflection;
using System.Web.Mvc;
using MvcSTSApplication.App_Start;
using MvcSTSApplication.IdentityProviders;
using MvcSTSApplication.IdentityProviders.Twitter;
using MvcSTSApplication.Infrastructure;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof (SimpleInjectorInitializer), "Initialize")]

namespace MvcSTSApplication.App_Start
{
    public static class SimpleInjectorInitializer
    {
        /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
        public static void Initialize()
        {
            // Did you know the container can diagnose your configuration? Go to: http://bit.ly/YE8OJj.
            var container = new Container();

            InitializeContainer(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.RegisterMvcAttributeFilterProvider();

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        private static void InitializeContainer(Container container)
        {
            container.Register<IConfigurationProvider, AppSettingsConfigurationProvider>();
            container.Register<IMetadataProvider, TwitterSecurityTokenServiceConfiguration>();
            container.Register<SecurityTokenServiceConfiguration, TwitterSecurityTokenServiceConfiguration>();
            container.Register<IIdentityProvider, TwitterSecurityTokenService>();
        }
    }
}