using System;
using Avalonia.Controls.Notifications;

namespace SampleApp.Services;

public class NotificationService : INotificationService
{
  private int _notificationTimeout = 5;
  private WindowNotificationManager? _notificationManager;

  /// <inheritdoc/>
  public int MaxItems { get; set; } = 4;

  /// <inheritdoc/>
  public int NotificationTimeout { get => _notificationTimeout; set => _notificationTimeout = (value < 0) ? 0 : value; }

  /// <inheritdoc/>
  public void SetHostWindow(TopLevel hostWindow)
  {
    var notificationManager = new WindowNotificationManager(hostWindow)
    {
        Position = NotificationPosition.BottomRight,
        MaxItems = MaxItems,
        Margin = new Thickness(0, 0, 15, 40)
    };

    _notificationManager = notificationManager;
  }

  /// <inheritdoc/>
  public void Show(string title,
                   string message,
                   NotificationType notificationType = NotificationType.Information,
                   Action? onClick = null,
                   Action? onClose = null)
  {
    if (_notificationManager is { } nm)
    {
        nm.Show(
            new Notification(
            title,
            message,
            notificationType,
            TimeSpan.FromSeconds(_notificationTimeout),
            onClick,
            onClose));
    }
  }
}
