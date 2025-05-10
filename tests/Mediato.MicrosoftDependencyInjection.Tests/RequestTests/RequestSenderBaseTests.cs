using Mediato.Abstractions;
using Mediato.Common;
using Mediato.Common.Extensions;
using Mediato.Data.Request;
using Mediato.MicrosoftDependencyInjection.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Mediato.MicrosoftDependencyInjection.Tests.RequestTests;

public abstract class RequestSenderBaseTests
{
	private readonly IServiceCollection _services;

	protected IServiceProvider ServiceProvider { get; }

	public RequestSenderBaseTests()
	{
		_services = new ServiceCollection();
		_services.AddMediato(Configure);
		_services.AddSingleton<IRequestHandler<ChangeNameRequest, bool>, ChangeNameRequestHandlerDuplicate>();

		_services.AddTransient<OrderCounter>();
		_services.AddSingleton(typeof(Owner<,>));
		_services.AddSingleton(typeof(Owner<,,>));

		ServiceProvider = _services.BuildServiceProvider();
	}

	protected abstract void Configure(MediatorConfiguration config);

	protected async Task SendAsyncCalledNTimes_Should_InvokeSingleHandlerRegisteredToTheRequestNTimes(int count)
	{
		//arrange
		var sender = ServiceProvider.GetRequiredService<IRequestSender>();
		var oddRequest = new ChangeNameRequest("Darth Vader");
		var evenRequest = new ChangeNameRequest(" ");
		var responses = new bool[count];

		//act
		for (var i = 0; i < count; i++)
		{
			var request = i % 2 == 0 ? evenRequest : oddRequest;
			responses[i] = await sender.SendAsync<ChangeNameRequest, bool>(request, TestContext.Current.CancellationToken);
		}

		//assert
		responses.ShouldAllSatisfy((x, index) => index % 2 == 1 == x);
		CounterShouldBe<ChangeNameRequestHandler>(0);
		CounterShouldBe<ChangeNameRequestHandlerDuplicate>(count);
	}

	protected void CounterShouldBe<TOwner>(int expectedCount)
	{
		var counter = ServiceProvider.GetRequiredService<Owner<TOwner, OrderCounter>>();
		counter.Service.Count.ShouldBe(expectedCount);
	}
}
