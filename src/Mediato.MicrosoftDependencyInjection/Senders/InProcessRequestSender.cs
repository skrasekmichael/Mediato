using Mediato.Abstractions;
using Mediato.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Mediato.Senders;

public sealed class InProcessRequestSender(IServiceProvider serviceProvider) : IRequestSender
{
	private readonly IServiceProvider _serviceProvider = serviceProvider;

	public Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken ct) where TRequest : IRequest<TResponse>
	{
		var handler = _serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>();
		if (handler is null)
		{
			throw new RequestHandlerNotFoundException(typeof(TRequest));
		}

		return handler.HandleAsync(request, ct);
	}
}
