using Avalonia.Controls.Notifications;
namespace TaskManagement.Services;

public interface INotificationService
{
    public void Register(INotificationManager notificationManager);
    public void Show(Notification notification);
}