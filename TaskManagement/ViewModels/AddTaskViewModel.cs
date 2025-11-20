using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using TaskManagement.Models;
using TaskManagement.Repositories.Tasks;
using TaskManagement.Repositories.Users;
using TaskManagement.Services;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace TaskManagement.ViewModels;

public class AddTaskViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private readonly IViewNavigator _viewNavigator;
    private readonly ITaskRepository<int> _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStatusTaskRepository _statusTaskRepository;
    
    private readonly ObservableCollection<StatusTaskInfo> _statusTasks;
    private readonly ObservableCollection<UserInfo> _users;
    
    private async Task SaveTask()
    {
        await _taskRepository.Add(Task, _userService.UserToken);
        _notificationService.Show(new Notification("Успех", "Задача успешно добавлена в БД"));
        _viewNavigator.GoBack();
    }
    private async void LoadData()
    {
        try
        {
            _users.AddRange(await _userRepository.GetAllEmployeeWorkers());
            _statusTasks.AddRange(await _statusTaskRepository.GetAll());
        }
        catch (Exception ex)
        {
            ExceptionHandling(ex);
        }
    }
    
    public AddTaskViewModel(INotificationService notificationService, ITaskRepository<int> taskRepository, IUserRepository userRepository,
        IUserService userService, IViewNavigator viewNavigator, IStatusTaskRepository statusTaskRepository) : base(notificationService)
    {
        _taskRepository = taskRepository;
        _userService = userService;
        _userRepository = userRepository;
        _viewNavigator = viewNavigator;
        _statusTaskRepository = statusTaskRepository;
        Task = new EditTask()
        {
            Date = DateOnly.FromDateTime(DateTime.Now)
        };
        _users = new ObservableCollection<UserInfo>();
        _statusTasks = new ObservableCollection<StatusTaskInfo>();
        LoadData();
        SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
        SaveCommand.ThrownExceptions.Subscribe(ExceptionHandling);
    }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ICollection<UserInfo>  Users => _users;
    public ICollection<StatusTaskInfo>  StatusTasks => _statusTasks;
    public override string Title => "Добавить новую задачу";
    public EditTask Task { get;}
}