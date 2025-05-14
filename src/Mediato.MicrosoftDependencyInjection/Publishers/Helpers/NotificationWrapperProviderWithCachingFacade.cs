using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Mediato.Publishers.Helpers;

internal sealed class NotificationWrapperProviderWithCachingFacade : INotificationWrapperProvider
{
	private static readonly Type NotificationHandlerWrapperTypeDefinition = typeof(NotificationHandlerWrapper<>);

	private readonly ConcurrentDictionary<Type, INotificationHandlerWrapper> _notificationToWrapperCache = [];

	public INotificationHandlerWrapper GetWrapper(Type notificationType)
	{
		return _notificationToWrapperCache.GetOrAdd(notificationType, localNotificationType =>
		{
			var wrapperType = NotificationHandlerWrapperTypeDefinition.MakeGenericType(localNotificationType);
			return Unsafe.As<INotificationHandlerWrapper>(Activator.CreateInstance(wrapperType))!;
		});
	}
}
