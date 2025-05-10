using Mediato.Abstractions;
using Mediato.Data.Notification;
using Mediato.MicrosoftDependencyInjection.Tests.Mocks;
using Mediato.MicrosoftDependencyInjection.Tests.RegistrationTests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Internal;

namespace Mediato.MicrosoftDependencyInjection.Tests.RegistrationTests;

public sealed class NotificationHandlersRegistrationTests
{
	[Theory]
	[InlineData(ServiceLifetime.Singleton)]
	[InlineData(ServiceLifetime.Scoped)]
	[InlineData(ServiceLifetime.Transient)]
	public void RegisterNotificationHandler_Should_CorrectlyRegisterGivenHandler(ServiceLifetime serviceLifetime)
	{
		//arrange
		var services = new ServiceCollection();

		var expectedDescriptor = new ServiceDescriptor(
			serviceType: typeof(INotificationHandler<NameChangedNotification>),
			implementationType: typeof(NameChangedNotificationHandler),
			lifetime: serviceLifetime);

		//act
		services.AddMediato(cfg =>
		{
			cfg.RegisterNotificationHandler<NameChangedNotificationHandler, NameChangedNotification>(serviceLifetime);
		});

		//assert
		services.ShouldHaveSingleItem();
		services.First().ShouldMatch(expectedDescriptor);
	}

	[Theory]
	[InlineData(ServiceLifetime.Singleton)]
	[InlineData(ServiceLifetime.Scoped)]
	[InlineData(ServiceLifetime.Transient)]
	public void RegisterNotificationHandlersFromAssembly_Should_CorrectlyRegisterAllNotificationHandlersFromGivenAssemblies(ServiceLifetime serviceLifetime)
	{
		//arrange
		var services = new ServiceCollection();
		var expectedServices = new ServiceCollection();
		expectedServices.AddService<INotificationHandler<NameChangedNotification>, NameChangedNotificationHandler>(serviceLifetime);
		expectedServices.AddService<INotificationHandler<NameChangedNotification>, NameChangedNotificationHandlerA>(serviceLifetime);
		expectedServices.AddService<INotificationHandler<NameChangedNotification>, NameChangedNotificationHandlerB>(serviceLifetime);
		expectedServices.AddService<INotificationHandler<NameChangedNotification>, NameChangedNotificationHandlerC>(serviceLifetime);

		//act
		services.AddMediato(cfg =>
		{
			cfg.RegisterNotificationHandlersFromAssembly<AssemblyReference>(serviceLifetime);
			cfg.RegisterNotificationHandlersFromAssembly<Data.AssemblyReference>(serviceLifetime);
		});

		//assert
		expectedServices.ForEach(x => services.ShouldContain(y => y.Matches(x)));
	}

	[Fact]
	public void RegisterNotificationHandler_ShouldNot_RegisterSameNotificationHandlerTwice()
	{
		//arrange
		var services = new ServiceCollection();
		services.AddSingleton<INotificationHandler<NameChangedNotification>, NameChangedNotificationHandler>();

		var expectedDescriptor = new ServiceDescriptor(
			serviceType: typeof(INotificationHandler<NameChangedNotification>),
			implementationType: typeof(NameChangedNotificationHandler),
			lifetime: ServiceLifetime.Singleton);

		//act
		services.AddMediato(cfg =>
		{
			cfg.RegisterNotificationHandler<NameChangedNotificationHandler, NameChangedNotification>();
		});

		//assert
		services.Where(x => x.Matches(expectedDescriptor)).ShouldHaveSingleItem();
	}

	[Fact]
	public void RegisterNotificationHandlersFromAssembly_ShouldNot_RegisterSameNotificationHandlerTwice()
	{
		//arrange
		var services = new ServiceCollection();
		services.AddSingleton<INotificationHandler<NameChangedNotification>, NameChangedNotificationHandler>();

		var expectedDescriptor = new ServiceDescriptor(
			serviceType: typeof(INotificationHandler<NameChangedNotification>),
			implementationType: typeof(NameChangedNotificationHandler),
			lifetime: ServiceLifetime.Singleton);

		//act
		services.AddMediato(cfg =>
		{
			cfg.RegisterNotificationHandlersFromAssembly<AssemblyReference>();
		});

		//assert
		services.Where(x => x.Matches(expectedDescriptor)).ShouldHaveSingleItem();
	}
}
