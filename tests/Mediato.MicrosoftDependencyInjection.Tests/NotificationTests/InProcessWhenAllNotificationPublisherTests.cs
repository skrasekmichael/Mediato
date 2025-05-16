using System.Diagnostics;
using Mediato.Abstractions;
using Mediato.Common;
using Mediato.Data.Notification;
using Mediato.MicrosoftDependencyInjection.Tests.Mocks;
using Mediato.Publishers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Mediato.MicrosoftDependencyInjection.Tests.NotificationTests;

public sealed class InProcessWhenAllNotificationPublisherTests : NotificationPublisherBaseTests
{
	protected override void Configure(MediatorConfiguration config)
	{
		config.RegisterNotificationHandlersFromAssembly<Data.AssemblyReference>();
		config.RegisterNotificationHandlersFromAssembly<AssemblyReference>();
		config.RegisterNotificationHandler<BasicNotificationHandler, BasicNotification>();
		config.UseCachingLayer(false);
		config.UseNotificationPublisher<InProcessWhenAllNotificationPublisher>();
	}

	[Theory]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(10)]
	public Task PublishAsyncCalledNTimes_Should_NotifyAllHandlersRegisteredToTheNotificationNTimes(int count)
	{
		return CallPublishAsyncNTimesAndVerifyThatAllHandlersRegisteredToTheNotificationWereNotifiedNTimes<BasicNotification, BasicNotification>(count, BasicNotificationHandlerType);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(10)]
	public Task PublishAsyncCalledNTimes_WithBoxedNotification_Should_NotifyAllHandlersRegisteredToTheNotificationNTimes(int count)
	{
		return CallPublishAsyncNTimesAndVerifyThatAllHandlersRegisteredToTheNotificationWereNotifiedNTimes<INotification, BasicNotification>(count, BasicNotificationHandlerType);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(10)]
	public Task PublishAsyncCalledNTimes_WithNestedNotification_Should_NotifyAllHandlersRegisteredToTheNotificationNTimes(int count)
	{
		return CallPublishAsyncNTimesAndVerifyThatAllHandlersRegisteredToTheNotificationWereNotifiedNTimes<NestedNotification, NestedNotification>(count, NestedNotificationHandlerType);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(10)]
	public Task PublishAsyncCalledNTimes_WithBoxedNestedNotification_Should_NotifyAllHandlersRegisteredToTheNotificationNTimes(int count)
	{
		return CallPublishAsyncNTimesAndVerifyThatAllHandlersRegisteredToTheNotificationWereNotifiedNTimes<INotification, NestedNotification>(count, NestedNotificationHandlerType);
	}

	[Fact]
	public async Task PublishAsync_Should_NotifyAllHandlersSimultaneously()
	{
		//arrange
		var publisher = ServiceProvider.GetRequiredService<INotificationPublisher>();
		var notification = new ConcurrentNotification();

		//act
		await publisher.PublishAsync(notification, TestContext.Current.CancellationToken);

		//assert
		OrderShouldBe<ConcurrentNotificationHandlerC>(1);
		OrderShouldBe<ConcurrentNotificationHandlerB>(2);
		OrderShouldBe<ConcurrentNotificationHandlerA>(3);

		var runtimeA = GetRuntime<ConcurrentNotificationHandlerA>();
		var runtimeB = GetRuntime<ConcurrentNotificationHandlerB>();
		var runtimeC = GetRuntime<ConcurrentNotificationHandlerC>();

		runtimeC.ShouldBeInRange(runtimeB);
		runtimeB.ShouldBeInRange(runtimeA);
	}

	[Fact]
	public async Task BoxedPublishAsync_Should_NotifyAllHandlersSimultaneously()
	{
		//arrange
		var publisher = ServiceProvider.GetRequiredService<INotificationPublisher>();
		INotification notification = new ConcurrentNotification();

		//act
		await publisher.PublishAsync(notification, TestContext.Current.CancellationToken);

		//assert
		OrderShouldBe<ConcurrentNotificationHandlerC>(1);
		OrderShouldBe<ConcurrentNotificationHandlerB>(2);
		OrderShouldBe<ConcurrentNotificationHandlerA>(3);

		var runtimeA = GetRuntime<ConcurrentNotificationHandlerA>();
		var runtimeB = GetRuntime<ConcurrentNotificationHandlerB>();
		var runtimeC = GetRuntime<ConcurrentNotificationHandlerC>();

		runtimeC.ShouldBeInRange(runtimeB);
		runtimeB.ShouldBeInRange(runtimeA);
	}

	private RuntimeStatistics GetRuntime<TOwner>()
	{
		var runtime = ServiceProvider.GetRequiredService<Owner<TOwner, Unit<RuntimeStatistics>>>().Service.Value;
		runtime.ShouldNotBeNull();
		Debug.WriteLine(runtime);
		return runtime;
	}
}
