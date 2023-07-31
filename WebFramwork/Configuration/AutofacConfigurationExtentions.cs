using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Data;
using Data.Contracts;
using Data.Repositories;
using Entities.Common;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Services;

namespace WebFramework.Configuration;

public static class AutofacConfigurationExtentions
{
    public static void AddService(this ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

        // property injection
        // assemblyScanning + auto/conventional Register
        // interception

        var commonAssembly = typeof(SiteSettings).Assembly;
        var entitiesAssembly = typeof(IEntity).Assembly;
        var dataAssembly = typeof(ApplicationDbContext).Assembly;
        var servicesAssembly = typeof(JwtService).Assembly;

        containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
            .AssignableTo<IScopedDependency>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
        containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
            .AssignableTo<ITransientDependency>()
            .AsImplementedInterfaces()
            .InstancePerDependency();
        containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
            .AssignableTo<ISingletonDependency>()
            .AsImplementedInterfaces()
            .SingleInstance();
    }

    public static IServiceProvider BuildAutofacServiceProvider(this IServiceCollection services)
    {
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(services);

        containerBuilder.AddService();

        var container = containerBuilder.Build();
        return new AutofacServiceProvider(container);
    }
}