using Mediato.Abstractions;
using Mediato.Common;
using Mediato.Data.Request;

namespace Mediato.MicrosoftDependencyInjection.Tests.Mocks;

internal sealed class ChangeNameRequestHandler : IRequestHandler<ChangeNameRequest, bool>
{
	private readonly Owner<ChangeNameRequestHandler, OrderCounter> _counter;

	public ChangeNameRequestHandler(Owner<ChangeNameRequestHandler, OrderCounter> counter)
	{
		_counter = counter;
	}

	public Task<bool> HandleAsync(ChangeNameRequest request, CancellationToken ct)
	{
		_counter.Service.Invoke();

		if (string.IsNullOrWhiteSpace(request.NewName))
		{
			return Task.FromResult(false);
		}

		return Task.FromResult(true);
	}
}
