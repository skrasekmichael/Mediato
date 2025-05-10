using Mediato.Abstractions;
using Mediato.Common;

namespace Mediato.Data.Notification;

internal sealed class NameChangedNotificationHandlerA : INotificationHandler<NameChangedNotification>
{
	private readonly Owner<NameChangedNotificationHandlerA, OrderCounter> _counter;
	private readonly Owner<INameChangedNotificationHandler, OrderCounter> _jointCounter;
	private readonly Owner<NameChangedNotificationHandlerA, Unit<int>> _order;

	public NameChangedNotificationHandlerA(Owner<NameChangedNotificationHandlerA, OrderCounter> counter, Owner<INameChangedNotificationHandler, OrderCounter> jointCounter, Owner<NameChangedNotificationHandlerA, Unit<int>> order)
	{
		_counter = counter;
		_jointCounter = jointCounter;
		_order = order;
	}

	public Task HandleAsync(NameChangedNotification notification, CancellationToken ct = default)
	{
		var order = _jointCounter.Service.Invoke();
		_counter.Service.Invoke();
		_order.Service.Value = order;
		return Task.CompletedTask;
	}
}
