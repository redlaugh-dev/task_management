using System;
using TaskManagement.ViewModels;

namespace TaskManagement.Services;

public interface IViewNavigator
{
    public void GoTo<VM>(Action closeAction = null, object message = null) where VM: ViewModelBase;
    public void CloseAllAndOpen<VM>() where VM: ViewModelBase;
    public object Message { get; }
    public void GoBack();
    public bool CanGoBack { get; }
    public ViewModelBase SelectedViewModel { get; }
}