using System.Diagnostics;
using Mediato.Abstractions;
using Mediato.Common;

namespace Mediato.Data.Notification;

public sealed class ConcurrentNotificationHandlerA : INotificationHandler<ConcurrentNotification>
{
	private readonly Owner<ConcurrentNotificationHandlerB, CallbackCounter> _handlerCallbackB;
	private readonly Owner<ConcurrentNotificationHandlerA, CallbackCounter> _callback;
	private readonly Owner<INotificationHandler<ConcurrentNotification>, OrderCounter> _jointCounter;
	private readonly Owner<ConcurrentNotificationHandlerA, Unit<int>> _order;
	private readonly Owner<ConcurrentNotificationHandlerA, Unit<RuntimeStatistics>> _runtime;

	public ConcurrentNotificationHandlerA(Owner<ConcurrentNotificationHandlerB, CallbackCounter> handlerCallbackB, Owner<ConcurrentNotificationHandlerA, CallbackCounter> counter, Owner<INotificationHandler<ConcurrentNotification>, OrderCounter> jointCounter, Owner<ConcurrentNotificationHandlerA, Unit<int>> order, Owner<ConcurrentNotificationHandlerA, Unit<RuntimeStatistics>> runtime)
	{
		_handlerCallbackB = handlerCallbackB;
		_callback = counter;
		_jointCounter = jointCounter;
		_order = order;
		_runtime = runtime;
	}

	public async Task HandleAsync(ConcurrentNotification notification, CancellationToken ct = default)
	{
		await _callback.Service.InvokeAsync();
		var start = Stopwatch.GetTimestamp();

		await _handlerCallbackB.Service.WaitForSecondCallbackAsync();

		var order = _jointCounter.Service.Invoke();
		_order.Service.Value = order;

		_runtime.Service.Value = RuntimeStatistics.Get(start);
		await _callback.Service.InvokeAsync();
	}
}
