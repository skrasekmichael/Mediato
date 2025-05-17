using System.Reflection;
using Mediato.Abstractions;
using Mediato.Publishers;
using Mediato.Publishers.Helpers;
using Mediato.Senders;
using Microsoft.Extensions.DependencyInjection;

namespace Mediato;

public sealed class MediatorConfiguration(IServiceCollection services)
{
	private static readonly Type NotificationHandlerTypeDefinition = typeof(INotificationHandler<>);
	private static readonly Type RequestHandlerTypeDefinition = typeof(IRequestHandler<,>);

	private readonly IServiceCollection _services = services;

	public bool IsUsingCachingLayer { get; private set; }

	public MediatorConfiguration UseCachingLayer(bool enableCaching)
	{
		if (enableCaching)
		{
			IsUsingCachingLayer = true;
			_services.AddSingleton<INotificationWrapperProvider, NotificationWrapperProviderWithCachingFacade>();
		}
		else
		{
			IsUsingCachingLayer = false;
			_services.AddSingleton<INotificationWrapperProvider, FactoryNotificationWrapperProvider>();
		}

		return this;
	}

	public MediatorConfiguration UseDefaultRequestSender(ServiceLifetime lifetime = ServiceLifetime.Singleton) => UseRequestSender<InProcessRequestSender>(lifetime);

	public MediatorConfiguration UseRequestSender<TRequestSender>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TRequestSender : class, IRequestSender
	{
		_services.AddService<IRequestSender, TRequestSender>(lifetime);
		return this;
	}

	public MediatorConfiguration UseDefaultNotificationPublisher(ServiceLifetime lifetime = ServiceLifetime.Singleton) => UseNotificationPublisher<InProcessForEachNotificationPublisher>(lifetime);

	public MediatorConfiguration UseNotificationPublisher<TPublisher>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TPublisher : class, INotificationPublisher
	{
		_services.AddService<INotificationPublisher, TPublisher>(lifetime);
		return this;
	}

	public MediatorConfiguration RegisterRequestHandler<TRequestHandler, TRequest, TResponse>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
		where TRequestHandler : class, IRequestHandler<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
	{
		RegisterRequestHandlerService(typeof(IRequestHandler<TRequest, TResponse>), typeof(TRequestHandler), lifetime);
		return this;
	}

	public MediatorConfiguration RegisterNotificationHandler<TNotificationHandler, TNotification>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
		where TNotificationHandler : class, INotificationHandler<TNotification>
		where TNotification : INotification
	{
		RegisterNotificationHandlerService(typeof(INotificationHandler<TNotification>), typeof(TNotificationHandler), lifetime);
		return this;
	}

	public MediatorConfiguration RegisterRequestHandlersFromAssembly<TTypeInAssembly>(ServiceLifetime lifetime = ServiceLifetime.Singleton) => RegisterRequestHandlersFromAssembly(typeof(TTypeInAssembly).Assembly, lifetime);

	public MediatorConfiguration RegisterRequestHandlersFromAssembly(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Singleton)
	{
		RegisterHandlersFromAssembly(assembly, lifetime, RequestHandlerTypeDefinition, RegisterRequestHandlerService);
		return this;
	}

	public MediatorConfiguration RegisterNotificationHandlersFromAssembly<TTypeInAssembly>(ServiceLifetime lifetime = ServiceLifetime.Singleton) => RegisterNotificationHandlersFromAssembly(typeof(TTypeInAssembly).Assembly, lifetime);

	public MediatorConfiguration RegisterNotificationHandlersFromAssembly(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Singleton)
	{
		RegisterHandlersFromAssembly(assembly, lifetime, NotificationHandlerTypeDefinition, RegisterNotificationHandlerService);
		return this;
	}

	private void RegisterNotificationHandlerService(Type serviceType, Type implementationType, ServiceLifetime lifetime)
	{
		if (_services.IsTypeAlreadyRegistered(serviceType, implementationType))
		{
			return;
		}

		_services.AddService(serviceType, implementationType, lifetime);
	}

	private void RegisterRequestHandlerService(Type serviceType, Type implementationType, ServiceLifetime lifetime)
	{
		if (_services.IsTypeAlreadyRegistered(serviceType))
		{
			return;
		}

		_services.AddService(serviceType, implementationType, lifetime);
	}

	private static void RegisterHandlersFromAssembly(Assembly assembly, ServiceLifetime lifetime, Type handlerTypedDefinition, Action<Type, Type, ServiceLifetime> register)
	{
		var types = assembly
			.GetTypes()
			.Select(type => (type, type.GetGenericInterfaceType(handlerTypedDefinition)));

		foreach (var (implementationType, interfaceType) in types)
		{
			if (interfaceType is null)
			{
				continue;
			}

			var serviceType = handlerTypedDefinition.MakeGenericType(interfaceType.GetGenericArguments());
			register(serviceType, implementationType, lifetime);
		}
	}
}
