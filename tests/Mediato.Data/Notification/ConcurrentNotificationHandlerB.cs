using System.Diagnostics;
using Mediato.Abstractions;
using Mediato.Common;

namespace Mediato.Data.Notification;

public sealed class ConcurrentNotificationHandlerB : INotificationHandler<ConcurrentNotification>
{
	private readonly Owner<ConcurrentNotificationHandlerC, CallbackCounter> _handlerCallbackC;
	private readonly Owner<ConcurrentNotificationHandlerB, CallbackCounter> _callback;
	private readonly Owner<INotificationHandler<ConcurrentNotification>, OrderCounter> _jointCounter;
	private readonly Owner<ConcurrentNotificationHandlerB, Unit<int>> _order;
	private readonly Owner<ConcurrentNotificationHandlerB, Unit<RuntimeStatistics>> _runtime;
	private readonly Owner<ConcurrentNotificationHandlerA, CallbackCounter> _handlerCallbackA;

	public ConcurrentNotificationHandlerB(Owner<ConcurrentNotificationHandlerC, CallbackCounter> handlerCallbackC, Owner<ConcurrentNotificationHandlerB, CallbackCounter> callback, Owner<INotificationHandler<ConcurrentNotification>, OrderCounter> jointCounter, Owner<ConcurrentNotificationHandlerB, Unit<int>> order, Owner<ConcurrentNotificationHandlerB, Unit<RuntimeStatistics>> runtime, Owner<ConcurrentNotificationHandlerA, CallbackCounter> handlerCallbackA)
	{
		_handlerCallbackC = handlerCallbackC;
		_callback = callback;
		_jointCounter = jointCounter;
		_order = order;
		_runtime = runtime;
		_handlerCallbackA = handlerCallbackA;
	}

	public async Task HandleAsync(ConcurrentNotification notification, CancellationToken ct = default)
	{
		await _handlerCallbackA.Service.WaitForFirstCallbackAsync();

		await _callback.Service.InvokeAsync();
		var start = Stopwatch.GetTimestamp();

		await _handlerCallbackC.Service.WaitForSecondCallbackAsync();
		var order = _jointCounter.Service.Invoke();
		_order.Service.Value = order;

		_runtime.Service.Value = RuntimeStatistics.Get(start);
		await _callback.Service.InvokeAsync();
	}
}
