using Mediato.Abstractions;
using Mediato.Common;

namespace Mediato.Data.Request;

internal sealed class ChangeNameRequestHandlerB : IRequestHandler<ChangeNameRequestB, bool>
{
	private readonly Owner<ChangeNameRequestB, OrderCounter> _counter;

	public ChangeNameRequestHandlerB(Owner<ChangeNameRequestB, OrderCounter> counter)
	{
		_counter = counter;
	}

	public Task<bool> HandleAsync(ChangeNameRequestB request, CancellationToken ct)
	{
		_counter.Service.Invoke();

		if (string.IsNullOrWhiteSpace(request.NewName))
		{
			return Task.FromResult(false);
		}

		return Task.FromResult(true);
	}
}
