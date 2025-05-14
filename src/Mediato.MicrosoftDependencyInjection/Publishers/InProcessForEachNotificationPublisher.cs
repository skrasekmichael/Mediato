using Mediato.Abstractions;
using Mediato.Publishers.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Mediato.Publishers;

public sealed partial class InProcessForEachNotificationPublisher(IServiceProvider serviceProvider) : INotificationPublisher
{
	private static readonly Type NotificationHandlerTypeDefinition = typeof(INotificationHandler<>);

	private readonly IServiceProvider _serviceProvider = serviceProvider;
	private readonly INotificationWrapperProvider _wrapperProvider = serviceProvider.GetRequiredService<INotificationWrapperProvider>();

	public async ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken ct = default) where TNotification : INotification
	{
		var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();
		foreach (var handler in handlers)
		{
			await handler.HandleAsync(notification, ct);
		}
	}

	public async ValueTask PublishAsync(INotification notification, CancellationToken ct = default)
	{
		var notificationType = notification.GetType();
		var handlerType = NotificationHandlerTypeDefinition.MakeGenericType(notificationType);

		var wrapper = _wrapperProvider.GetWrapper(notificationType);
		var handlers = _serviceProvider.GetServices(handlerType);
		foreach (var handler in handlers)
		{
			await wrapper.HandleAsync(handler, notification, ct);
		}
	}
}
