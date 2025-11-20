using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Splat;
using TaskManagement.ViewModels;

namespace TaskManagement.Services;

public class WindowNavigationService : IWindowNavigationService
{
    public void GoTo<VM>() where VM : ViewModelBase
    {
        string windowFullName = $"{typeof(VM).Namespace.Replace("ViewModel","View")}.{typeof(VM).Name.Replace("ViewModel", "Window")}";
        Window window = (Window)Activator.CreateInstance(Type.GetType(windowFullName));
        Locator.Current.GetService<INotificationService>().Register(
            new WindowNotificationManager(window));
        window.DataContext = Locator.Current.GetService<VM>();
        window.Show();
    }

    public void CloseByViewModel<VM>() where VM : ViewModelBase
    {
        ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).Windows
            .Where(w => w.DataContext is VM)
            .ToList()
            .ForEach(w => w.Close());
    }
}