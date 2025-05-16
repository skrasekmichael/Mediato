using Mediato.Common;
using Mediato.Data.Notification;

namespace Mediato.MicrosoftDependencyInjection.Tests.Mocks;

internal sealed class BasicNotificationHandler : IBasicNotificationHandler
{
	private readonly Owner<BasicNotificationHandler, OrderCounter> _counter;
	private readonly Owner<IBasicNotificationHandler, OrderCounter> _jointCounter;
	private readonly Owner<BasicNotificationHandler, Unit<int>> _order;

	public BasicNotificationHandler(Owner<BasicNotificationHandler, OrderCounter> counter, Owner<IBasicNotificationHandler, OrderCounter> jointCounter, Owner<BasicNotificationHandler, Unit<int>> order)
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
