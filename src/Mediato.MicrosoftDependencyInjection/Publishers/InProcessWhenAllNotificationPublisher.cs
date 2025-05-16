using Mediato.Abstractions;
using Mediato.Publishers.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Mediato.Publishers;

public sealed class InProcessWhenAllNotificationPublisher(IServiceProvider serviceProvider) : INotificationPublisher
{
	private static readonly Type NotificationHandlerTypeDefinition = typeof(INotificationHandler<>);

	private readonly IServiceProvider _serviceProvider = serviceProvider;
	private readonly INotificationWrapperProvider _wrapperProvider = serviceProvider.GetRequiredService<INotificationWrapperProvider>();

	public ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken ct = default) where TNotification : INotification
	{
		if (typeof(TNotification).IsInterface)
		{
			return DefaultInternalPublishAsync(notification, ct);
		}

		return GenericInternalPublishAsync(notification, ct);
	}

	private async ValueTask GenericInternalPublishAsync<TNotification>(TNotification notification, CancellationToken ct = default) where TNotification : INotification
	{
		var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();
		if (!handlers.Any())
		{
			return;
		}

		var tasks = handlers.Select(x => x.HandleAsync(notification, ct));
		await Task.WhenAll(tasks);
	}

	private async ValueTask DefaultInternalPublishAsync(INotification notification, CancellationToken ct = default)
	{
		var notificationType = notification.GetType();
		var handlerType = NotificationHandlerTypeDefinition.MakeGenericType(notificationType);

		var wrapper = _wrapperProvider.GetWrapper(notificationType);
		var handlers = _serviceProvider.GetServices(handlerType);
		if (!handlers.Any())
		{
			return;
		}

		var tasks = handlers.Select(x => wrapper.HandleAsync(x, notification, ct));
		await Task.WhenAll(tasks);
	}
}
