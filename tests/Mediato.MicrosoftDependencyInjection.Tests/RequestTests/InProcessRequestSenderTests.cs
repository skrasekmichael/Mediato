using Mediato.Data.Request;
using Mediato.MicrosoftDependencyInjection.Tests.Mocks;

namespace Mediato.MicrosoftDependencyInjection.Tests.RequestTests;

public sealed class InProcessRequestSenderTests : RequestSenderBaseTests
{
	protected override void Configure(MediatorConfiguration config)
	{
		config.RegisterRequestHandlersFromAssembly<Data.AssemblyReference>();
		config.RegisterRequestHandlersFromAssembly<AssemblyReference>();
		config.RegisterRequestHandler<ChangeNameRequestHandler, ChangeNameRequest, bool>();
		config.UseDefaultRequestSender();
	}

	[Fact]
	public Task SendAsync_Should_InvokeSingleHandlerRegisteredToTheRequest()
	{
		return SendAsyncCalledNTimes_Should_InvokeSingleHandlerRegisteredToTheRequestNTimes(1);
	}

	[Fact]
	public Task SendAsyncCalledTwice_Should_InvokeSingleHandlerRegisteredToTheRequestTwice()
	{
		return SendAsyncCalledNTimes_Should_InvokeSingleHandlerRegisteredToTheRequestNTimes(2);
	}
}
