using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using DynamicData;
using ReactiveUI;
using TaskManagement.Models;
using TaskManagement.Repositories.Tasks;
using TaskManagement.Repositories.Users;
using TaskManagement.Services;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace TaskManagement.ViewModels;

public class TaskPanelViewModel : ViewModelBase
{
    private readonly ITaskRepository<int> _taskRepository;
    private readonly IStatusTaskRepository _statusTaskRepository;
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IViewNavigator _viewNavigator;
    private readonly IDialogService _dialogService;
    
    private readonly ObservableCollection<StatusTaskInfo>  _statusTasks;
    private readonly ObservableCollection<UserInfo>  _users;
    private readonly ObservableCollection<TaskInfo> _tasks;

    private readonly ObservableAsPropertyHelper<bool> _clearFilterVisible;
    private int _roleId;
    private IDisposable _canceledReloadCommand;

    private async void LoadData()
    {
        try
        {
            _statusTasks.AddRange(await _statusTaskRepository.GetAll());
            _users.AddRange(await _userRepository.GetAllEmployeeWorkers());
            var userInfo = await _userService.GetUserInfo();
            RoleId = userInfo.TypeUser.Id;
            CanceledAndReloadTasks();
        }
        catch (Exception ex)
        {
            ExceptionHandling(ex);
        }
    }


    private async Task UpdateStatusTask(ReadOnlyCollection<object> p)
    {
        _taskRepository.UpdateStatusTask((int)p[0], (int)p[1]);
        _notificationService.Show(new Notification("Успех","Статус успешно изменен",NotificationType.Success));
        CanceledAndReloadTasks();
    }
    private async Task ReloadTasks(CancellationToken ct)
    {
        var tasks = await _taskRepository.GetAll(_userService.UserToken, TaskFilters);
        if (ct.IsCancellationRequested)
        {
            return;
        }
        foreach (var keyValuePair in Categories)
        {
            keyValuePair.Value.Clear();
        }
        foreach (var task in tasks)
        {
            var days = (int)Math.Ceiling((task.Date.ToDateTime(TimeOnly.MinValue) - DateTime.Now).TotalDays);
            if (days < 0)
            {
                Categories["Прошедшие"].Add(task);
            }
            else if (days == 0)
            {
                Categories["Сегодня"].Add(task);
            }
            else if (days == 1)
            {
                Categories["Завтра"].Add(task);
            }
            else
            {
                Categories["Предстоящие"].Add(task);
            }
                
        }
    }

    private void CanceledAndReloadTasks()
    {
        if (_canceledReloadCommand != null)
        {
            _canceledReloadCommand.Dispose();
        }
        _canceledReloadCommand = ReloadTasksCommand.Execute().Subscribe();
    }
    private async Task RemoveTask(int id)
    {
        if (await _dialogService.ShowDialog("Подтверждение", "Вы действительно хотите удалить задачу?",
                DialogButtons.YesNo) == DialogResult.Yes)
        {
            await _taskRepository.Remove(id);
            _notificationService.Show(new Notification("Успех","Запись успешно удалена",NotificationType.Success));
            CanceledAndReloadTasks();
        }
    }

    private async Task ShowDescriptionTask(string description)
    {
        await _dialogService.ShowDialog("Описание задачи", description, DialogButtons.Ok);
    }
    
    
    public TaskPanelViewModel(INotificationService notificationService, ITaskRepository<int> taskRepository,
        IUserService userService, IStatusTaskRepository statusTaskRepository, IViewNavigator viewNavigator,
        IDialogService dialogService, IUserRepository userRepository) : base(notificationService)
    {
        _taskRepository = taskRepository;
        _userService = userService;
        _userRepository = userRepository;
        _statusTaskRepository = statusTaskRepository;
        _viewNavigator = viewNavigator;
        _dialogService = dialogService;
        Categories = new Dictionary<string, ObservableCollection<TaskInfo>>()
        {
            { "Прошедшие", new ObservableCollection<TaskInfo>() },
            { "Сегодня", new ObservableCollection<TaskInfo>() },
            { "Завтра", new ObservableCollection<TaskInfo>() },
            { "Предстоящие", new ObservableCollection<TaskInfo>() },
        };
        _statusTasks = new ObservableCollection<StatusTaskInfo>();
        _tasks = new ObservableCollection<TaskInfo>();
        _users =  new ObservableCollection<UserInfo>();
        TaskFilters = new TaskFilters();
        TaskFilters.WhenAnyValue(p=>p.SearchString)
            .WhereNotNull()
            .Subscribe((_) => CanceledAndReloadTasks());
        TaskFilters.WhenAnyValue(p=>p.Date)
            .WhereNotNull()
            .Subscribe((_) => CanceledAndReloadTasks());
        TaskFilters.WhenAnyValue(p=>p.ExecutorId)
            .WhereNotNull()
            .Subscribe((_) => CanceledAndReloadTasks());
        TaskFilters.WhenAnyValue(p=>p.StatusId)
            .WhereNotNull()
            .Subscribe((_) => CanceledAndReloadTasks());
        _clearFilterVisible = TaskFilters.WhenAnyValue(x => x.Date, x => x.ExecutorId, x => x.StatusId)
            .Select(p => p.Item1 != null || p.Item2 != null || p.Item3 != null)
            .ToProperty(this, x => x.ClearFiltersVisible);
        RemoveTaskCommand = ReactiveCommand.CreateFromTask<int>(RemoveTask);
        ReloadTasksCommand = ReactiveCommand.CreateFromTask(ct => ReloadTasks(ct));
        ReloadTasksCommand.ThrownExceptions.Subscribe(ExceptionHandling);
        RemoveTaskCommand.ThrownExceptions.Subscribe(ExceptionHandling);
        UpdateStatusTaskCommand = ReactiveCommand.CreateFromTask<ReadOnlyCollection<object>>(UpdateStatusTask);
        UpdateStatusTaskCommand.ThrownExceptions.Subscribe(ExceptionHandling);
        ShowDescriptionCommand = ReactiveCommand.CreateFromTask<string>(ShowDescriptionTask);
        ShowDescriptionCommand.ThrownExceptions.Subscribe(ExceptionHandling);   
        LoadData();
    }

    public void ClearFilters()
    {
        TaskFilters.Date = null;
        TaskFilters.ExecutorId = null;
        TaskFilters.StatusId = null;
        CanceledAndReloadTasks();
    }
    public ReactiveCommand<String, Unit> ShowDescriptionCommand { get; }
    public ReactiveCommand<Unit, Unit> ReloadTasksCommand { get; }
    public bool ClearFiltersVisible => _clearFilterVisible.Value;
    public void GoToAdd() => _viewNavigator.GoTo<AddTaskViewModel>(()=> CanceledAndReloadTasks());
    public void GoToEdit(int id) => _viewNavigator.GoTo<EditTaskViewModel>(()=> CanceledAndReloadTasks(), id);

    public ICollection<StatusTaskInfo> StatusTasks => _statusTasks;
    public ICollection<TaskInfo> Tasks => _tasks;
    public ICollection<UserInfo> Users => _users;
    public ReactiveCommand <int, Unit> RemoveTaskCommand { get; }
    public ReactiveCommand <ReadOnlyCollection<object>, Unit> UpdateStatusTaskCommand { get; }
    public override string Title => "Задачи";
    public TaskFilters TaskFilters { get;}

    public int RoleId
    {
        get => _roleId;
        set => this.RaiseAndSetIfChanged(ref _roleId, value);
    }
    public Dictionary<string, ObservableCollection<TaskInfo>> Categories { get; }
}