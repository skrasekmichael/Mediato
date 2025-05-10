using Mediato.Abstractions;
using Mediato.Common;

namespace Mediato.Data.Notification;

internal sealed class NameChangedNotificationHandlerB : INotificationHandler<NameChangedNotification>
{
	private readonly Owner<NameChangedNotificationHandlerB, OrderCounter> _counter;
	private readonly Owner<INameChangedNotificationHandler, OrderCounter> _jointCounter;
	private readonly Owner<NameChangedNotificationHandlerB, Unit<int>> _order;

	public NameChangedNotificationHandlerB(Owner<NameChangedNotificationHandlerB, OrderCounter> counter, Owner<INameChangedNotificationHandler, OrderCounter> jointCounter, Owner<NameChangedNotificationHandlerB, Unit<int>> order)
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
