namespace Mediato.Abstractions;

public interface IRequestSender
{
	Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken ct) where TRequest : IRequest<TResponse>;
}
