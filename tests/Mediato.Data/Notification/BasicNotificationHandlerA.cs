using Mediato.Abstractions;
using Mediato.Common;

namespace Mediato.Data.Notification;

internal sealed class BasicNotificationHandlerA : INotificationHandler<BasicNotification>
{
	private readonly Owner<BasicNotificationHandlerA, OrderCounter> _counter;
	private readonly Owner<IBasicNotificationHandler, OrderCounter> _jointCounter;
	private readonly Owner<BasicNotificationHandlerA, Unit<int>> _order;

	public BasicNotificationHandlerA(Owner<BasicNotificationHandlerA, OrderCounter> counter, Owner<IBasicNotificationHandler, OrderCounter> jointCounter, Owner<BasicNotificationHandlerA, Unit<int>> order)
	{
		_counter = counter;
		_jointCounter = jointCounter;
		_order = order;
	}

	public Task HandleAsync(BasicNotification notification, CancellationToken ct = default)
	{
		var order = _jointCounter.Service.Invoke();
		_counter.Service.Invoke();
		_order.Service.Value = order;
		return Task.CompletedTask;
	}
}
