using Mediato.Abstractions;
using Mediato.Data.Request;
using Mediato.MicrosoftDependencyInjection.Tests.Mocks;
using Mediato.MicrosoftDependencyInjection.Tests.RegistrationTests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Internal;

namespace Mediato.MicrosoftDependencyInjection.Tests.RegistrationTests;

public sealed class RequestHandlersRegistrationTests
{
	[Theory]
	[InlineData(ServiceLifetime.Singleton)]
	[InlineData(ServiceLifetime.Scoped)]
	[InlineData(ServiceLifetime.Transient)]
	public void RegisterRequestHandler_Should_CorrectlyRegisterGivenHandler(ServiceLifetime serviceLifetime)
	{
		//arrange
		var services = new ServiceCollection();

		var expectedDescriptor = new ServiceDescriptor(
			serviceType: typeof(IRequestHandler<ChangeNameRequest, bool>),
			implementationType: typeof(ChangeNameRequestHandler),
			lifetime: serviceLifetime);

		//act
		services.AddMediato(cfg =>
		{
			cfg.RegisterRequestHandler<ChangeNameRequestHandler, ChangeNameRequest, bool>(serviceLifetime);
		});

		//assert
		services.ShouldHaveSingleItem();
		services.First().ShouldMatch(expectedDescriptor);
	}

	[Theory]
	[InlineData(ServiceLifetime.Singleton)]
	[InlineData(ServiceLifetime.Scoped)]
	[InlineData(ServiceLifetime.Transient)]
	public void RegisterRequestHandlersFromAssembly_Should_CorrectlyRegisterForEachRequestOneRequestHandlersFromGivenAssemblies(ServiceLifetime serviceLifetime)
	{
		//arrange
		var services = new ServiceCollection();
		var expectedServices = new ServiceCollection();
		expectedServices.AddService<IRequestHandler<ChangeNameRequest, bool>, ChangeNameRequestHandler>(serviceLifetime);
		expectedServices.AddService<IRequestHandler<ChangeNameRequestA, bool>, ChangeNameRequestHandlerA>(serviceLifetime);
		expectedServices.AddService<IRequestHandler<ChangeNameRequestB, bool>, ChangeNameRequestHandlerB>(serviceLifetime);
		expectedServices.AddService<IRequestHandler<ChangeNameRequestC, bool>, ChangeNameRequestHandlerC>(serviceLifetime);

		var duplicateHandler = new ServiceDescriptor(
			serviceType: typeof(IRequestHandler<ChangeNameRequest, bool>),
			implementationType: typeof(ChangeNameRequestHandlerDuplicate),
			lifetime: ServiceLifetime.Singleton);

		//act
		services.AddMediato(cfg =>
		{
			cfg.RegisterRequestHandlersFromAssembly<AssemblyReference>(serviceLifetime);
			cfg.RegisterRequestHandlersFromAssembly<Data.AssemblyReference>(serviceLifetime);
		});

		//assert
		expectedServices.ForEach(x => services.ShouldContain(y => y.Matches(x)));
		services.FirstOrDefault(x => x.Matches(duplicateHandler)).ShouldBeNull();
	}

	[Fact]
	public void RegisterRequestHandler_ShouldNot_RegisterSameRequestHandlerTwice()
	{
		//arrange
		var services = new ServiceCollection();
		services.AddSingleton<IRequestHandler<ChangeNameRequest, bool>, ChangeNameRequestHandler>();

		var expectedDescriptor = new ServiceDescriptor(
			serviceType: typeof(IRequestHandler<ChangeNameRequest, bool>),
			implementationType: typeof(ChangeNameRequestHandler),
			lifetime: ServiceLifetime.Singleton);

		//act
		services.AddMediato(cfg =>
		{
			cfg.RegisterRequestHandler<ChangeNameRequestHandler, ChangeNameRequest, bool>();
		});

		//assert
		services.Where(x => x.Matches(expectedDescriptor)).ShouldHaveSingleItem();
	}

	[Fact]
	public void RegisterRequestHandlersFromAssembly_ShouldNot_RegisterSameRequestHandlerTwice()
	{
		//arrange
		var services = new ServiceCollection();
		services.AddSingleton<IRequestHandler<ChangeNameRequest, bool>, ChangeNameRequestHandler>();

		var expectedDescriptor = new ServiceDescriptor(
			serviceType: typeof(IRequestHandler<ChangeNameRequest, bool>),
			implementationType: typeof(ChangeNameRequestHandler),
			lifetime: ServiceLifetime.Singleton);

		var duplicateHandler = new ServiceDescriptor(
			serviceType: typeof(IRequestHandler<ChangeNameRequest, bool>),
			implementationType: typeof(ChangeNameRequestHandlerDuplicate),
			lifetime: ServiceLifetime.Singleton);

		//act
		services.AddMediato(cfg =>
		{
			cfg.RegisterNotificationHandlersFromAssembly<AssemblyReference>();
		});

		//assert
		services.Where(x => x.Matches(expectedDescriptor)).ShouldHaveSingleItem();
		services.FirstOrDefault(x => x.Matches(duplicateHandler)).ShouldBeNull();
	}
}
