using Mediato.Abstractions;
using Mediato.Common;
using Mediato.Data.Notification;
using Mediato.MicrosoftDependencyInjection.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Mediato.MicrosoftDependencyInjection.Tests.NotificationTests;

public abstract class NotificationPublisherBaseTests
{
	private static readonly Type OwnerType = typeof(Owner<,>);
	private static readonly Type OrderCounterType = typeof(OrderCounter);

	protected static readonly Type[] BasicNotificationHandlerType =
	[
		typeof(BasicNotificationHandler),
		typeof(BasicNotificationHandlerA),
		typeof(BasicNotificationHandlerB),
		typeof(BasicNotificationHandlerC),
	];

	protected static readonly Type[] NestedNotificationHandlerType =
	[
		typeof(NestedNotificationHandler)
	];

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

	protected async Task CallPublishAsyncNTimesAndVerifyThatAllHandlersRegisteredToTheNotificationWereNotifiedNTimes<TBaseType, T>(int count, params Type[] notificationHandlers)
		where TBaseType : INotification where T : TBaseType, new()
	{
		//arrange
		var publisher = ServiceProvider.GetRequiredService<INotificationPublisher>();
		TBaseType notification = new T();

		//act
		for (var i = 0; i < count; i++)
		{
			await publisher.PublishAsync(notification, TestContext.Current.CancellationToken);
		}

		//assert
		foreach (var handlerType in notificationHandlers)
		{
			CounterShouldBe(handlerType, count);
		}
	}

	protected void CounterShouldBe(Type counterOwner, int expectedCount)
	{
		var counterServiceType = OwnerType.MakeGenericType(counterOwner, OrderCounterType);
		var counter = ServiceProvider.GetRequiredService(counterServiceType) as IOwner<OrderCounter>;
		counter!.Service.Count.ShouldBe(expectedCount);
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
