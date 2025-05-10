using Mediato.Abstractions;

namespace Mediato.Data.Notification;

public sealed record NameChangedNotification(string NewValue) : INotification;
