using Mediato.Common;
using Mediato.Data.Notification;

namespace Mediato.MicrosoftDependencyInjection.Tests.Mocks;

internal sealed class NameChangedNotificationHandler : INameChangedNotificationHandler
{
	private readonly Owner<NameChangedNotificationHandler, OrderCounter> _counter;
	private readonly Owner<INameChangedNotificationHandler, OrderCounter> _jointCounter;
	private readonly Owner<NameChangedNotificationHandler, Unit<int>> _order;

	public NameChangedNotificationHandler(Owner<NameChangedNotificationHandler, OrderCounter> counter, Owner<INameChangedNotificationHandler, OrderCounter> jointCounter, Owner<NameChangedNotificationHandler, Unit<int>> order)
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
