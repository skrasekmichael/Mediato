namespace Mediato.Publishers.Helpers;

internal interface INotificationWrapperProvider
{
	INotificationHandlerWrapper GetWrapper(Type notificationType);
}
