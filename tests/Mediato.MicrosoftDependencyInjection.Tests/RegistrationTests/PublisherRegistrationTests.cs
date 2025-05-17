using Mediato.Abstractions;
using Mediato.MicrosoftDependencyInjection.Tests.RegistrationTests.Extensions;
using Mediato.Publishers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Mediato.MicrosoftDependencyInjection.Tests.RegistrationTests;

public sealed class PublisherRegistrationTests
{
	[Theory]
	[InlineData(ServiceLifetime.Singleton)]
	[InlineData(ServiceLifetime.Scoped)]
	public void UseDefaultNotificationPublisher_Should_RegisterInProcessForEachNotificationPublisher(ServiceLifetime lifetime)
	{
		//arrange
		var services = new ServiceCollection();
		var expectedServices = new ServiceCollection();
		expectedServices.AddService<INotificationPublisher, InProcessForEachNotificationPublisher>(lifetime);

		//act
		services.AddMediato(cfg =>
		{
			cfg.UseDefaultNotificationPublisher(lifetime);
		});

		//assert
		services.ShouldHaveSingleItem();
		expectedServices.First().ShouldMatch(expectedServices.First());
	}

	[Theory]
	[InlineData(ServiceLifetime.Singleton)]
	[InlineData(ServiceLifetime.Scoped)]
	public void UseNotificationPublisher_Should_RegisterGivenNotificationPublisher(ServiceLifetime lifetime)
	{
		//arrange
		var services = new ServiceCollection();
		var expectedServices = new ServiceCollection();
		expectedServices.AddService<INotificationPublisher, InProcessWhenAllNotificationPublisher>(lifetime);

		//act
		services.AddMediato(cfg =>
		{
			cfg.UseNotificationPublisher<InProcessWhenAllNotificationPublisher>(lifetime);
		});

		//assert
		services.ShouldHaveSingleItem();
		expectedServices.First().ShouldMatch(expectedServices.First());
	}
}
