using Mediato.Abstractions;
using Mediato.Common;

namespace Mediato.Data.Request;

public sealed class ChangeNameRequestHandlerA : IRequestHandler<ChangeNameRequestA, bool>
{
	private readonly Owner<ChangeNameRequestA, OrderCounter> _counter;

	public ChangeNameRequestHandlerA(Owner<ChangeNameRequestA, OrderCounter> counter)
	{
		_counter = counter;
	}

	public Task<bool> HandleAsync(ChangeNameRequestA request, CancellationToken ct)
	{
		_counter.Service.Invoke();

		if (string.IsNullOrWhiteSpace(request.NewName))
		{
			return Task.FromResult(false);
		}

		return Task.FromResult(true);
	}
}
