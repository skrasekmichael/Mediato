using Mediato.Abstractions;

namespace Mediato.Publishers.Helpers;

internal interface INotificationHandlerWrapper
{
	Task HandleAsync(object? handler, INotification notification, CancellationToken ct = default);
}
