using Mediato.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Mediato.Publishers;

public sealed class InProcessForEachNotificationPublisher(IServiceProvider serviceProvider) : INotificationPublisher
{
	private readonly IServiceProvider _serviceProvider = serviceProvider;

	public async ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken ct = default) where TNotification : INotification
	{
		var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();
		foreach (var handler in handlers)
		{
			await handler.HandleAsync(notification, ct);
		}
	}
}
