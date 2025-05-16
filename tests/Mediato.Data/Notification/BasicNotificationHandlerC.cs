using Mediato.Common;

namespace Mediato.Data.Notification;

internal sealed class BasicNotificationHandlerC : IBasicNotificationHandler
{
	private readonly Owner<BasicNotificationHandlerC, OrderCounter> _counter;
	private readonly Owner<IBasicNotificationHandler, OrderCounter> _jointCounter;
	private readonly Owner<BasicNotificationHandlerC, Unit<int>> _order;

	public BasicNotificationHandlerC(Owner<BasicNotificationHandlerC, OrderCounter> counter, Owner<IBasicNotificationHandler, OrderCounter> jointCounter, Owner<BasicNotificationHandlerC, Unit<int>> order)
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
