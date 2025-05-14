namespace Mediato.Abstractions;

public interface INotificationPublisher
{
	ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken ct = default) where TNotification : INotification;

	ValueTask PublishAsync(INotification notification, CancellationToken ct = default);
}
