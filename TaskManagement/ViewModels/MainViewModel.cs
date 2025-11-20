using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using ReactiveUI;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private UserInfo _userInfo;
    private readonly IUserService _userService;
    private ObservableCollection<MenuItem> _menuItems;
    private readonly IViewNavigator _viewNavigator;
    private MenuItem _selectedMenuItem;
    public MainViewModel(INotificationService notificationService, IUserService userService, IViewNavigator viewNavigator) : base(notificationService)
    {
        _userService = userService;
        _menuItems = new ObservableCollection<MenuItem>();
        _viewNavigator = viewNavigator;
        LoadUserInfo();
    }
    private async void LoadUserInfo()
    {
        try
        {
            UserInfo = await _userService.GetUserInfo();
            switch (UserInfo.TypeUser.Id)
            {
                case 1:
                    _menuItems.Add(new MenuItem("Задачи","CalendarCheck", () => _viewNavigator.CloseAllAndOpen<TaskPanelViewModel>()));
                    break;
                case 2:
                    _menuItems.AddRange(new List<MenuItem>()
                    {
                        new MenuItem("Задачи","CalendarCheck", () => _viewNavigator.CloseAllAndOpen<TaskPanelViewModel>()),
                    });
                    
                    break;
                case 3:
                    _menuItems.AddRange(new List<MenuItem>()
                    {
                        new MenuItem("Задачи","CalendarCheck", () => _viewNavigator.CloseAllAndOpen<TaskPanelViewModel>()),
                        new MenuItem("Панель адмнистратора","Security", () => _viewNavigator.CloseAllAndOpen<AdminPanelViewModel>()),
                    });
                    break;
            }
            SelectedMenuItem = _menuItems[0];
        }
        catch (Exception ex)
        {
            ExceptionHandling(ex);
        }
    }
    public IViewNavigator ViewNavigator => _viewNavigator;
    public ICollection<MenuItem> MenuItems => _menuItems;
    public UserInfo UserInfo
    {
        get => _userInfo;
        set
        {
            this.RaiseAndSetIfChanged(ref _userInfo, value);
        }
    }

    public MenuItem SelectedMenuItem
    {
        get => _selectedMenuItem;
        set
        {
            if (_selectedMenuItem != value)
            {
                _selectedMenuItem = value;
                _selectedMenuItem.Action?.Invoke(); 
            }
        }
    }
    public override string Title => "Главное окно";
}