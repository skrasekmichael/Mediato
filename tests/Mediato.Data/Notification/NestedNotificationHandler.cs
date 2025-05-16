using Mediato.Abstractions;
using Mediato.Common;

namespace Mediato.Data.Notification;

internal sealed class NestedNotificationHandler : INotificationHandler<NestedNotification>
{
	private readonly Owner<NestedNotificationHandler, OrderCounter> _counter;

	public NestedNotificationHandler(Owner<NestedNotificationHandler, OrderCounter> counter)
	{
		_counter = counter;
	}

	public Task HandleAsync(NestedNotification notification, CancellationToken ct = default)
	{
		_counter.Service.Invoke();
		return Task.CompletedTask;
	}
}
