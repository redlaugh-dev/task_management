using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using Splat;
using TaskManagement.DataBase;
using TaskManagement.Repositories.Tasks;
using TaskManagement.Repositories.Users;
using TaskManagement.Services;
using TaskManagement.ViewModels;
using TaskManagement.Views;

namespace TaskManagement;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Locator.CurrentMutable.Register(()=> new DataBaseContext());
            SplatRegistrations.RegisterLazySingleton<IUserService,UserService>();
            SplatRegistrations.RegisterLazySingleton<INotificationService, NotificationService>();
            SplatRegistrations.RegisterLazySingleton<IWindowNavigationService, WindowNavigationService>();
            SplatRegistrations.Register<AuthorizationViewModel>();
            SplatRegistrations.Register<MainViewModel>();
            SplatRegistrations.Register<AdminPanelViewModel>();
            SplatRegistrations.Register<AddUserViewModel>();
            SplatRegistrations.Register<EditUserViewModel>();
            SplatRegistrations.Register<ITaskRepository<int>, TaskSqLiteRepository>();
            SplatRegistrations.Register<IStatusTaskRepository, StatusTaskSqLiteRepository>();
            SplatRegistrations.Register<TaskPanelViewModel>();
            SplatRegistrations.Register<AddTaskViewModel>();
            SplatRegistrations.Register<EditTaskViewModel>();
            SplatRegistrations.RegisterLazySingleton<IViewNavigator, ObservableViewNavigator>();
            SplatRegistrations.Register<ITypeUserRepository, TypeUserSqLiteRepository>();
            SplatRegistrations.Register<IUserRepository, UserSqLiteRepository>();
            SplatRegistrations.Register<IDialogService, DialogService>();
            SplatRegistrations.SetupIOC();
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new AuthorizationWindow()
            {
                DataContext = Locator.Current.GetService<AuthorizationViewModel>()
            };
            Locator.Current.GetService<INotificationService>().Register(new WindowNotificationManager(desktop.MainWindow));
            
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}