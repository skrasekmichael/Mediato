using Mediato.Abstractions;
using Mediato.MicrosoftDependencyInjection.Senders;
using Mediato.MicrosoftDependencyInjection.Tests.RegistrationTests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Mediato.MicrosoftDependencyInjection.Tests.RegistrationTests;

public sealed class RequestSenderRegistrationTests
{
	[Fact]
	public void UseDefaultRequestSender_Should_RegisterInProcessRequestSender()
	{
		//arrange
		var services = new ServiceCollection();
		var expectedServices = new ServiceCollection();
		expectedServices.AddSingleton<IRequestSender, InProcessRequestSender>();

		//act
		services.AddMediato(cfg =>
		{
			cfg.UseDefaultRequestSender();
		});

		//assert
		services.ShouldHaveSingleItem();
		expectedServices.First().ShouldMatch(expectedServices.First());
	}

	[Fact]
	public void UseRequestSender_Should_RegisterGivenRequestSender()
	{
		//arrange
		var services = new ServiceCollection();
		var expectedServices = new ServiceCollection();
		expectedServices.AddSingleton<IRequestSender, InProcessRequestSender>();

		//act
		services.AddMediato(cfg =>
		{
			cfg.UseRequestSender<InProcessRequestSender>();
		});

		//assert
		services.ShouldHaveSingleItem();
		expectedServices.First().ShouldMatch(expectedServices.First());
	}
}
