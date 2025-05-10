using Mediato.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Mediato.MicrosoftDependencyInjection.Publishers;

public sealed class InProcessWhenAllNotificationPublisher(IServiceProvider serviceProvider) : INotificationPublisher
{
	private readonly IServiceProvider _serviceProvider = serviceProvider;

	public async ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken ct = default) where TNotification : INotification
	{
		var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();
		if (!handlers.Any())
		{
			return;
		}

		var tasks = handlers.Select(x => x.HandleAsync(notification, ct));
		await Task.WhenAll(tasks);
	}
}
