using Microsoft.Extensions.DependencyInjection;

namespace Mediato.MicrosoftDependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMediato(this IServiceCollection services, Action<MediatorConfiguration> applyConfiguration)
	{
		var configuration = new MediatorConfiguration(services);
		applyConfiguration(configuration);

		return services;
	}

	public static IServiceCollection AddService<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime) where TImplementation : TService
	{
		var descriptor = new ServiceDescriptor(
			serviceType: typeof(TService),
			implementationType: typeof(TImplementation),
			lifetime: lifetime);

		services.Add(descriptor);
		return services;
	}

	internal static IServiceCollection AddService(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime)
	{
		var descriptor = new ServiceDescriptor(
			serviceType: serviceType,
			implementationType: implementationType,
			lifetime: lifetime);

		services.Add(descriptor);
		return services;
	}

	internal static bool IsTypeAlreadyRegistered(this IServiceCollection services, Type serviceType, Type implementationType) => services.Any(x => x.ServiceType == serviceType && x.ImplementationType == implementationType);

	internal static bool IsTypeAlreadyRegistered(this IServiceCollection services, Type serviceType) => services.Any(x => x.ServiceType == serviceType);
}
