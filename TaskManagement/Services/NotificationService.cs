using System;
using Avalonia.Controls.Notifications;


namespace TaskManagement.Services;

public class NotificationService : INotificationService
{
    private INotificationManager _notificationManager;
    public void Register(INotificationManager notificationManager)
    {
        _notificationManager = notificationManager;
    }

    public void Show(Notification notification)
    {
        if (_notificationManager == null)
        {
            throw new NullReferenceException("Notification Manager is null");
        }
        _notificationManager.Show(notification);
    }
}