using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using TaskManagement.Models;
using TaskManagement.Repositories.Users;
using TaskManagement.Services;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace TaskManagement.ViewModels;

public class AddUserViewModel : ViewModelBase
{
    private readonly ITypeUserRepository  _typeUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly ObservableCollection<TypeUserInfo> _typeUsers;
    private readonly IViewNavigator _viewNavigator;
    private async Task SaveUser()
    {
        await _userRepository.AddUser(User);
        _notificationService.Show(new Notification("Успех", "Пользователь успешно добавлен в БД"));
        _viewNavigator.GoBack();
    }
    private async void LoadData()
    {
        try
        {
            _typeUsers.AddRange(await _typeUserRepository.GetAll());
        }
        catch (Exception ex)
        {
            ExceptionHandling(ex);
        }
    }
    public AddUserViewModel(INotificationService notificationService, ITypeUserRepository typeUserRepository, 
        IUserRepository userRepository, IViewNavigator viewNavigator) : base(notificationService)
    {
        User = new EditUser();
        _typeUserRepository  = typeUserRepository;
        _userRepository    = userRepository;
        _viewNavigator = viewNavigator;
        _typeUsers = new ObservableCollection<TypeUserInfo>();
        SaveCommand = ReactiveCommand.CreateFromTask(SaveUser);
        SaveCommand.ThrownExceptions.Subscribe(ExceptionHandling);
        LoadData();
    }
    public ICollection<TypeUserInfo> TypeUsers => _typeUsers;
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public EditUser User { get; }
    public override string Title => "Добавить нового пользователя";
}