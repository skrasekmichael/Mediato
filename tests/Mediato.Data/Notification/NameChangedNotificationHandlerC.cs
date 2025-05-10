using Mediato.Common;

namespace Mediato.Data.Notification;

internal sealed class NameChangedNotificationHandlerC : INameChangedNotificationHandler
{
	private readonly Owner<NameChangedNotificationHandlerC, OrderCounter> _counter;
	private readonly Owner<INameChangedNotificationHandler, OrderCounter> _jointCounter;
	private readonly Owner<NameChangedNotificationHandlerC, Unit<int>> _order;

	public NameChangedNotificationHandlerC(Owner<NameChangedNotificationHandlerC, OrderCounter> counter, Owner<INameChangedNotificationHandler, OrderCounter> jointCounter, Owner<NameChangedNotificationHandlerC, Unit<int>> order)
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
