using Mediato.Common;

namespace Mediato.Data.Request;

internal sealed class ChangeNameRequestHandlerC : IChangeNameRequestHandlerC
{
	private readonly Owner<ChangeNameRequestC, OrderCounter> _counter;

	public ChangeNameRequestHandlerC(Owner<ChangeNameRequestC, OrderCounter> counter)
	{
		_counter = counter;
	}

	public Task<bool> HandleAsync(ChangeNameRequestC request, CancellationToken ct)
	{
		_counter.Service.Invoke();

		if (string.IsNullOrWhiteSpace(request.NewName))
		{
			return Task.FromResult(false);
		}

		return Task.FromResult(true);
	}
}
