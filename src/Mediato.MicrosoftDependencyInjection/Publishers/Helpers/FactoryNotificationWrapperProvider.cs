using System.Runtime.CompilerServices;

namespace Mediato.Publishers.Helpers;

internal sealed class FactoryNotificationWrapperProvider : INotificationWrapperProvider
{
	private static readonly Type NotificationHandlerWrapperTypeDefinition = typeof(NotificationHandlerWrapper<>);

	public INotificationHandlerWrapper GetWrapper(Type notificationType)
	{
		var wrapperType = NotificationHandlerWrapperTypeDefinition.MakeGenericType(notificationType);
		return Unsafe.As<INotificationHandlerWrapper>(Activator.CreateInstance(wrapperType))!;
	}
}
