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

public class EditUserViewModel : ViewModelBase
{
    private readonly ITypeUserRepository  _typeUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly ObservableCollection<TypeUserInfo> _typeUsers;
    private readonly IViewNavigator _viewNavigator;
    private EditUser _editUser;
    private async Task UpdateUser()
    {
        await _userRepository.UpdateUser(User);
        _notificationService.Show(new Notification("Успех", "Пользователь успешно изменен в БД"));
        _viewNavigator.GoBack();
    }
    private async void LoadData()
    {
        try
        {
            if (_viewNavigator.Message == null)
            {
                throw new Exception("Не найдена информация о сотруднике");
            }
            int id = (int)_viewNavigator.Message;
            _typeUsers.AddRange(await _typeUserRepository.GetAll());
            User = await _userRepository.GetUserById(id);
        }
        catch (Exception ex)
        {
            ExceptionHandling(ex);
        }
    }
    public EditUserViewModel(INotificationService notificationService, ITypeUserRepository typeUserRepository, 
        IUserRepository userRepository, IViewNavigator viewNavigator) : base(notificationService)
    {
        User = new EditUser();
        _typeUserRepository = typeUserRepository;
        _userRepository = userRepository;
        _viewNavigator = viewNavigator;
        _typeUsers = new ObservableCollection<TypeUserInfo>();
        UpdateCommand = ReactiveCommand.CreateFromTask(UpdateUser);
        UpdateCommand.ThrownExceptions.Subscribe(ExceptionHandling);
        LoadData();
    }
    public ICollection<TypeUserInfo> TypeUsers => _typeUsers;
    public ReactiveCommand<Unit, Unit> UpdateCommand { get; }

    public EditUser User
    {
        get => _editUser;
        set
        {
            this.RaiseAndSetIfChanged(ref _editUser, value);  
        }
    }
    public override string Title => "Редактировать пользователя";
}