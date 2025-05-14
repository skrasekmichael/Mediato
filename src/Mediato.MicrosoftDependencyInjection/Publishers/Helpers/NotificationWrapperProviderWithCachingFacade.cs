using System.Runtime.CompilerServices;

namespace Mediato.Publishers.Helpers;

internal sealed class NotificationWrapperProviderWithCachingFacade : INotificationWrapperProvider
{
	private static readonly Type NotificationHandlerWrapperTypeDefinition = typeof(NotificationHandlerWrapper<>);

	private readonly Dictionary<Type, INotificationHandlerWrapper> _notificationToWrapperCache = [];

	public INotificationHandlerWrapper GetWrapper(Type notificationType)
	{
		if (_notificationToWrapperCache.TryGetValue(notificationType, out var cachedWrapper))
		{
			return cachedWrapper;
		}

		var wrapperType = NotificationHandlerWrapperTypeDefinition.MakeGenericType(notificationType);
		var wrapper = Unsafe.As<INotificationHandlerWrapper>(Activator.CreateInstance(wrapperType))!;
		_notificationToWrapperCache.Add(notificationType, wrapper);

		return wrapper;
	}
}
