using Mediato.Abstractions;
using Mediato.Common;
using Mediato.Data.Request;

namespace Mediato.MicrosoftDependencyInjection.Tests.Mocks;

internal sealed class ChangeNameRequestHandlerDuplicate : IRequestHandler<ChangeNameRequest, bool>
{
	private readonly Owner<ChangeNameRequestHandlerDuplicate, OrderCounter> _counter;

	public ChangeNameRequestHandlerDuplicate(Owner<ChangeNameRequestHandlerDuplicate, OrderCounter> counter)
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
