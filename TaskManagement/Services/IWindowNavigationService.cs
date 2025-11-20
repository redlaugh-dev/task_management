using TaskManagement.ViewModels;

namespace TaskManagement.Services;

public interface IWindowNavigationService
{
    public void GoTo<VM>() where VM : ViewModelBase;
    public void CloseByViewModel<VM>() where VM : ViewModelBase;
}