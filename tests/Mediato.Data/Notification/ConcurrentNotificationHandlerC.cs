using System.Diagnostics;
using Mediato.Abstractions;
using Mediato.Common;

namespace Mediato.Data.Notification;

public sealed class ConcurrentNotificationHandlerC : INotificationHandler<ConcurrentNotification>
{
	private readonly Owner<ConcurrentNotificationHandlerC, CallbackCounter> _callback;
	private readonly Owner<INotificationHandler<ConcurrentNotification>, OrderCounter> _jointCounter;
	private readonly Owner<ConcurrentNotificationHandlerC, Unit<int>> _order;
	private readonly Owner<ConcurrentNotificationHandlerC, Unit<RuntimeStatistics>> _runtime;
	private readonly Owner<ConcurrentNotificationHandlerB, CallbackCounter> _handlerCallbackB;

	public ConcurrentNotificationHandlerC(Owner<ConcurrentNotificationHandlerC, CallbackCounter> callback, Owner<INotificationHandler<ConcurrentNotification>, OrderCounter> jointCounter, Owner<ConcurrentNotificationHandlerC, Unit<int>> order, Owner<ConcurrentNotificationHandlerC, Unit<RuntimeStatistics>> runtime, Owner<ConcurrentNotificationHandlerB, CallbackCounter> handlerCallbackB)
	{
		_callback = callback;
		_jointCounter = jointCounter;
		_order = order;
		_runtime = runtime;
		_handlerCallbackB = handlerCallbackB;
	}

	public async Task HandleAsync(ConcurrentNotification notification, CancellationToken ct = default)
	{
		await _handlerCallbackB.Service.WaitForFirstCallbackAsync();

		await _callback.Service.InvokeAsync();
		var start = Stopwatch.GetTimestamp();

		var order = _jointCounter.Service.Invoke();
		_order.Service.Value = order;

		_runtime.Service.Value = RuntimeStatistics.Get(start);
		await _callback.Service.InvokeAsync();
	}
}
