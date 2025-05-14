namespace Mediato.Exceptions;

public sealed class RequestHandlerNotFoundException : Exception
{
	public Type RequestType { get; }

	public RequestHandlerNotFoundException(Type requestType) : base($"Handler for request {requestType.Name} has not been registered.")
	{
		RequestType = requestType;
	}
}
