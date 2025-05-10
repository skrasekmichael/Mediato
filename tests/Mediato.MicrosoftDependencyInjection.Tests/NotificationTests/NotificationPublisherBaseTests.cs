using Mediato.Abstractions;
using Mediato.Common;
using Mediato.Data.Notification;
using Mediato.MicrosoftDependencyInjection.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Mediato.MicrosoftDependencyInjection.Tests.NotificationTests;

public abstract class NotificationPublisherBaseTests
{
	private readonly IServiceCollection _services;

	protected IServiceProvider ServiceProvider { get; }

	public NotificationPublisherBaseTests()
	{
		_services = new ServiceCollection();
		_services.AddMediato(Configure);
		_services.AddTransient<OrderCounter>();
		_services.AddTransient<CallbackCounter>();
		_services.AddTransient(typeof(Unit<>));
		_services.AddSingleton(typeof(Owner<,>));
		_services.AddSingleton(typeof(Owner<,,>));

		ServiceProvider = _services.BuildServiceProvider();
	}

	protected abstract void Configure(MediatorConfiguration config);

	protected async Task PublishAsyncCalledNTimes_Should_NotifyAllHandlersRegisteredToTheNotificationNTimes(int count)
	{
		//arrange
		var publisher = ServiceProvider.GetRequiredService<INotificationPublisher>();
		var notification = new NameChangedNotification("Darth Vader");

		//act
		for (var i = 0; i < count; i++)
		{
			await publisher.PublishAsync(notification, TestContext.Current.CancellationToken);
		}

		//assert
		CounterShouldBe<NameChangedNotificationHandler>(count);
		CounterShouldBe<NameChangedNotificationHandlerA>(count);
		CounterShouldBe<NameChangedNotificationHandlerB>(count);
		CounterShouldBe<NameChangedNotificationHandlerC>(count);
	}

	protected void CounterShouldBe<TOwner>(int expectedCount)
	{
		var counter = ServiceProvider.GetRequiredService<Owner<TOwner, OrderCounter>>();
		counter.Service.Count.ShouldBe(expectedCount);
	}

	protected void OrderShouldBe<TOwner>(int expectedCount)
	{
		var counter = ServiceProvider.GetRequiredService<Owner<TOwner, Unit<int>>>();
		counter.Service.Value.ShouldBe(expectedCount);
	}
}
