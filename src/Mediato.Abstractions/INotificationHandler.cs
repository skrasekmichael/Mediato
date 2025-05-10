namespace Mediato.Abstractions;

public interface INotificationHandler<TNotification> where TNotification : INotification
{
	Task HandleAsync(TNotification notification, CancellationToken ct = default);
}
