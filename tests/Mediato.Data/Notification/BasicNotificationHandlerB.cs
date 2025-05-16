using Mediato.Abstractions;
using Mediato.Common;

namespace Mediato.Data.Notification;

internal sealed class BasicNotificationHandlerB : INotificationHandler<BasicNotification>
{
	private readonly Owner<BasicNotificationHandlerB, OrderCounter> _counter;
	private readonly Owner<IBasicNotificationHandler, OrderCounter> _jointCounter;
	private readonly Owner<BasicNotificationHandlerB, Unit<int>> _order;

	public BasicNotificationHandlerB(Owner<BasicNotificationHandlerB, OrderCounter> counter, Owner<IBasicNotificationHandler, OrderCounter> jointCounter, Owner<BasicNotificationHandlerB, Unit<int>> order)
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
