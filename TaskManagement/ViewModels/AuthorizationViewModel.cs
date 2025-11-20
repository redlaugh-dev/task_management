using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using TaskManagement.Services;

namespace TaskManagement.ViewModels;

public class AuthorizationViewModel : ViewModelBase
{
    private readonly IUserService  _userService;
    private ObservableAsPropertyHelper<bool> _isAuthorized;
    private readonly IWindowNavigationService _windowNavigationService;

    public AuthorizationViewModel(INotificationService notificationService, IUserService userService, IWindowNavigationService windowNavigationService) : base(notificationService)
    {
        this._userService = userService;
        this._windowNavigationService = windowNavigationService;
        LoginCommand = ReactiveCommand.CreateFromTask(LogIn);
        LoginCommand.ThrownExceptions.Subscribe(ExceptionHandling);
        _isAuthorized = LoginCommand.CanExecute.ToProperty(this,x=>x.IsAuthorized);
    }

    private async Task LogIn()
    {
        await _userService.LogIn(Login, Password);
        _windowNavigationService.GoTo<MainViewModel>();
        _windowNavigationService.CloseByViewModel<AuthorizationViewModel>();
    }
    public bool IsAuthorized => _isAuthorized.Value;
    public string Login { get; set; }
    public string Password { get; set; }
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public override string Title => "Авторизация";
}