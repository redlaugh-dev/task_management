using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using DynamicData;
using ReactiveUI;
using TaskManagement.Models;
using TaskManagement.Repositories.Users;
using TaskManagement.Services;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace TaskManagement.ViewModels;

public class AdminPanelViewModel : ViewModelBase
{
    
    private readonly ITypeUserRepository  _typeUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDialogService _dialogService;
    private readonly ObservableCollection<UserInfo> _users;
    private readonly ObservableCollection<TypeUserInfo> _typeUsers;
    private string _searchString;
    private readonly IViewNavigator _viewNavigator;
    private IDisposable _canceledReloadCommand;
    public AdminPanelViewModel(INotificationService notificationService, ITypeUserRepository typeUserRepository
        , IUserRepository userRepository, IDialogService dialogService, IViewNavigator viewNavigator) : base(notificationService)
    {
        _typeUserRepository = typeUserRepository;
        _userRepository = userRepository;
        _users = new ObservableCollection<UserInfo>();
        _viewNavigator = viewNavigator;
        _dialogService = dialogService;
        _typeUsers = new ObservableCollection<TypeUserInfo>();
        ReloadUsersCommand = ReactiveCommand.CreateFromTask(ct => ReloadUsers(ct));
        ReloadUsersCommand.ThrownExceptions.Subscribe(ExceptionHandling);
        LoadData();
        
        this.WhenAnyValue(x => x.SearchString).Subscribe((_)=> CancelAndReloadUsers());

        RemoveUserCommand = ReactiveCommand.CreateFromTask<int>(RemoveUser);
        RemoveUserCommand.ThrownExceptions.Subscribe(ExceptionHandling);
        
    }

    private void CancelAndReloadUsers()
    {
        if (_canceledReloadCommand != null)
        {
            _canceledReloadCommand.Dispose();
        }
        _canceledReloadCommand = ReloadUsersCommand.Execute().Subscribe();
    }
    
    private async Task RemoveUser(int id)
    {
        if (await _dialogService.ShowDialog("Подтверждение", "Вы действительно хотите удалить запись?",
                DialogButtons.YesNo) == DialogResult.Yes)
        {
            await _userRepository.RemoveUser(id);
            _notificationService.Show(new Notification("Успех","Запись успешно удалена",NotificationType.Success));
            CancelAndReloadUsers();
        }
    }

    public void GoToAddUserView()
    {
        _viewNavigator.GoTo<AddUserViewModel>(()=> CancelAndReloadUsers());
    }
    public void GoToEditUserView(int id)
    {
        _viewNavigator.GoTo<EditUserViewModel>(()=> CancelAndReloadUsers(), id);
    }
    private async void LoadData()
    {
        try
        {
            _typeUsers.AddRange(await _typeUserRepository.GetAll());
            _users.AddRange(await _userRepository.GetAll());
        }
        catch (Exception ex)
        {
            ExceptionHandling(ex);
        }
    }

    private async Task ReloadUsers(CancellationToken ct)
    {
        ICollection<UserInfo> users = null;
        if (String.IsNullOrEmpty(SearchString))
        {
            users = await _userRepository.GetAll();
        }
        else
        {
            users = await _userRepository.FindUsers(SearchString);
        }
        if (ct.IsCancellationRequested)
        {
            return;
        }
        _users.Clear();
        _users.AddRange(users);
    }
    public ReactiveCommand<Unit, Unit> ReloadUsersCommand { get; }
    public ReactiveCommand<int, Unit> RemoveUserCommand { get; }
    public override string Title => "Панель администратора";
    public ICollection<UserInfo> Users => _users;
    public ICollection<TypeUserInfo> TypeUsers => _typeUsers;

    public string SearchString
    {
        get => _searchString;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchString, value);
        }
    }
}