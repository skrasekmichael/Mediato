using System.Runtime.CompilerServices;
using Mediato.Abstractions;

namespace Mediato.Publishers.Helpers;

internal sealed class NotificationHandlerWrapper<T> : INotificationHandlerWrapper where T : INotification
{
	public Task HandleAsync(object? handler, INotification notification, CancellationToken ct = default)
	{
		var typedHandler = Unsafe.As<INotificationHandler<T>>(handler)!;
		return typedHandler.HandleAsync((T)notification, ct);
	}
}
