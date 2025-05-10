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
		config.RegisterNotificationHandler<NameChangedNotificationHandler, NameChangedNotification>();
		config.UseDefaultNotificationPublisher();
	}

	[Fact]
	public Task PublishAsync_Should_NotifyAllHandlersRegisteredToTheNotification()
	{
		return PublishAsyncCalledNTimes_Should_NotifyAllHandlersRegisteredToTheNotificationNTimes(1);
	}

	[Fact]
	public Task PublishAsyncCalledTwice_Should_NotifyAllHandlersRegisteredToTheNotificationTwice()
	{
		return PublishAsyncCalledNTimes_Should_NotifyAllHandlersRegisteredToTheNotificationNTimes(2);
	}

	[Fact]
	public async Task PublishAsync_Should_NotifyAllHandlersRegisteredToTheNotificationInSequentialOrder()
	{
		//arrange
		var publisher = ServiceProvider.GetRequiredService<INotificationPublisher>();
		var notification = new NameChangedNotification("Darth Vader");

		//act
		await publisher.PublishAsync(notification, TestContext.Current.CancellationToken);

		//assert
		OrderShouldBe<NameChangedNotificationHandlerA>(1);
		OrderShouldBe<NameChangedNotificationHandlerB>(2);
		OrderShouldBe<NameChangedNotificationHandlerC>(3);
		OrderShouldBe<NameChangedNotificationHandler>(4);
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
}
