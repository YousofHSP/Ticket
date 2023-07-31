using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace WebFramework.CustomMapping;

public static class AutoMapperConfiguration
{
    public static void InitializeAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddAutoMapper(config => { config.AddCustomMappingProfile(); }, assemblies);
    }

    public static void AddCustomMappingProfile(this IMapperConfigurationExpression config)
    {
        config.AddCustomMappingProfile(Assembly.GetEntryAssembly()!);
    }

    public static void AddCustomMappingProfile(this IMapperConfigurationExpression config, params Assembly[] assemblies)
    {
        var allTypes = assemblies.SelectMany(a => a.ExportedTypes);

        var list = allTypes
            .Where(type => type.IsClass && !type.IsAbstract &&
                           type.GetInterfaces().Contains(typeof(IHaveCustomMapping)))
            .Select(type => (IHaveCustomMapping)Activator.CreateInstance(type)!);
        config.AddProfile(new CustomMappingProfile(list));
    }
}