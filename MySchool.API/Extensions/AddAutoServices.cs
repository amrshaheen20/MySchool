using MySchool.API.Interfaces;
using System.Reflection;

namespace MySchool.API.Extensions
{
    public enum ServiceLifetimeType
    {
        Scoped,
        Singleton,
        Transient
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceTypeAttribute : Attribute
    {
        public ServiceLifetimeType Lifetime { get; }
        public Type? ServiceType { get; set; } = null;

        public ServiceTypeAttribute(ServiceLifetimeType lifetime)
        {
            Lifetime = lifetime;
        }

        public ServiceTypeAttribute(ServiceLifetimeType lifetime, Type serviceType)
        {
            Lifetime = lifetime;
            ServiceType = serviceType;
        }
    }

    public static partial class ApiExtensions
    {
        public static IServiceCollection AddAutoServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddAutoServices(assembly);
            return services;
        }

        public static void AddAutoServices(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    (
                        typeof(IServiceInjector).IsAssignableFrom(t) ||
                        t.GetCustomAttribute<ServiceTypeAttribute>() != null
                    )
                )
                .Select(t => new
                {
                    Implementation = t,
                    Attribute = t.GetCustomAttribute<ServiceTypeAttribute>(),
                    DefaultService = t.GetInterfaces()
                        .Where(i => i != typeof(IServiceInjector))
                        .FirstOrDefault()
                });

            foreach (var item in types)
            {
                var lifetime = item.Attribute?.Lifetime ?? ServiceLifetimeType.Scoped;
                var serviceType = item.Attribute?.ServiceType ?? item.DefaultService ?? item.Implementation;

                switch (lifetime)
                {
                    case ServiceLifetimeType.Singleton:
                        services.AddSingleton(serviceType, item.Implementation);
                        break;

                    case ServiceLifetimeType.Transient:
                        services.AddTransient(serviceType, item.Implementation);
                        break;

                    case ServiceLifetimeType.Scoped:
                        services.AddScoped(serviceType, item.Implementation);
                        break;
                }
            }
        }

    }
}
