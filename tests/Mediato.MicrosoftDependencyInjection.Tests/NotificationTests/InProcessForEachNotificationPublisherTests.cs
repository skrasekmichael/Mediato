using Mediato.Abstractions;
using Mediato.Common.Extensions;
using Mediato.Data.Notification;
using Mediato.MicrosoftDependencyInjection.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Mediato.MicrosoftDependencyInjection.Tests.NotificationTests;

public sealed class InProcessForEachNotificationPublisherTests : NotificationPublisherBaseTests
{
	protected override void Configure(MediatorConfiguration config)
	{
		config.RegisterNotificationHandlersFromAssembly<Data.AssemblyReference>();
		config.RegisterNotificationHandlersFromAssembly<AssemblyReference>();
		config.RegisterNotificationHandler<BasicNotificationHandler, BasicNotification>();
		config.UseCachingLayer(false);
		config.UseDefaultNotificationPublisher();
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
	public async Task PublishAsync_Should_NotifyAllHandlersRegisteredToTheNotificationInSequentialOrder()
	{
		//arrange
		var publisher = ServiceProvider.GetRequiredService<INotificationPublisher>();
		var notification = new BasicNotification();

		//act
		await publisher.PublishAsync(notification, TestContext.Current.CancellationToken);

		//assert
		OrderShouldBe<BasicNotificationHandlerA>(1);
		OrderShouldBe<BasicNotificationHandlerB>(2);
		OrderShouldBe<BasicNotificationHandlerC>(3);
		OrderShouldBe<BasicNotificationHandler>(4);
	}

	[Fact]
	public async Task BoxedPublishAsync_Should_NotifyAllHandlersRegisteredToTheNotificationInSequentialOrder()
	{
		//arrange
		var publisher = ServiceProvider.GetRequiredService<INotificationPublisher>();
		INotification notification = new BasicNotification();

		//act
		await publisher.PublishAsync(notification, TestContext.Current.CancellationToken);

		//assert
		OrderShouldBe<BasicNotificationHandlerA>(1);
		OrderShouldBe<BasicNotificationHandlerB>(2);
		OrderShouldBe<BasicNotificationHandlerC>(3);
		OrderShouldBe<BasicNotificationHandler>(4);
	}

	[Fact]
	public async Task PublishAsync_ForConcurrentHandler_Should_TimeoutDueToDeadlock()
	{
		//arrange
		var publisher = ServiceProvider.GetRequiredService<INotificationPublisher>();
		var notification = new ConcurrentNotification();
		var ct = TestContext.Current.CancellationToken;

		//act
		var hasTimedOut = await publisher.PublishAsync(notification, ct).WithTimeout(2000, ct);

		//assert
		hasTimedOut.ShouldBeTrue();
	}

	[Fact]
	public async Task BoxedPublishAsync_ForConcurrentHandler_Should_TimeoutDueToDeadlock()
	{
		//arrange
		var publisher = ServiceProvider.GetRequiredService<INotificationPublisher>();
		INotification notification = new ConcurrentNotification();
		var ct = TestContext.Current.CancellationToken;

		//act
		var hasTimedOut = await publisher.PublishAsync(notification, ct).WithTimeout(2000, ct);

		//assert
		hasTimedOut.ShouldBeTrue();
	}
}
