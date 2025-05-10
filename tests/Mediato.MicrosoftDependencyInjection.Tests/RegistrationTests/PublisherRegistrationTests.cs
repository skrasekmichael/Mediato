using Mediato.Abstractions;
using Mediato.MicrosoftDependencyInjection.Publishers;
using Mediato.MicrosoftDependencyInjection.Tests.RegistrationTests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Mediato.MicrosoftDependencyInjection.Tests.RegistrationTests;

public sealed class PublisherRegistrationTests
{
	[Fact]
	public void UseDefaultNotificationPublisher_Should_RegisterInProcessForEachNotificationPublisher()
	{
		//arrange
		var services = new ServiceCollection();
		var expectedServices = new ServiceCollection();
		expectedServices.AddSingleton<INotificationPublisher, InProcessForEachNotificationPublisher>();

		//act
		services.AddMediato(cfg =>
		{
			cfg.UseDefaultNotificationPublisher();
		});

		//assert
		services.ShouldHaveSingleItem();
		expectedServices.First().ShouldMatch(expectedServices.First());
	}

	[Fact]
	public void UseNotificationPublisher_Should_RegisterGivenNotificationPublisher()
	{
		//arrange
		var services = new ServiceCollection();
		var expectedServices = new ServiceCollection();
		expectedServices.AddSingleton<INotificationPublisher, InProcessWhenAllNotificationPublisher>();

		//act
		services.AddMediato(cfg =>
		{
			cfg.UseNotificationPublisher<InProcessWhenAllNotificationPublisher>();
		});

		//assert
		services.ShouldHaveSingleItem();
		expectedServices.First().ShouldMatch(expectedServices.First());
	}
}
