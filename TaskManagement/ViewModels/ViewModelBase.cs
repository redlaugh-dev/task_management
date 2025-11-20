using System;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ReactiveUI;
using TaskManagement.Services;

namespace TaskManagement.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
    protected INotificationService _notificationService;
    protected void ExceptionHandling(Exception ex)
    {
        _notificationService.Show(new Notification("Ошибка", ex.Message, NotificationType.Error));
    }
    public ViewModelBase(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    
    public abstract string Title { get; }
}