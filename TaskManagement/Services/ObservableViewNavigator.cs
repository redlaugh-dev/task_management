using System;
using System.Collections.Generic;
using ReactiveUI;
using Splat;
using TaskManagement.ViewModels;

namespace TaskManagement.Services;

public class ObservableViewNavigator : ReactiveObject, IViewNavigator
{
    private readonly Stack<(ViewModelBase, Action)> _viewsStack;
    private ViewModelBase _selectedViewModel;
    private bool _canBack; 
    private object _message;
    
    public ObservableViewNavigator()
    {
        _viewsStack = new Stack<(ViewModelBase, Action)>();
    }

    public void GoTo<VM>(Action closeAction = null, object message = null) where VM : ViewModelBase
    {
        _message = message;
        var viewModel = Locator.Current.GetService<VM>();
        _viewsStack.Push((viewModel, closeAction));
        SelectedViewModel = viewModel;
        CanGoBack = _viewsStack.Count > 1;
    }

    public void CloseAllAndOpen<VM>() where VM: ViewModelBase
    {
        _message = null;
        var viewModel = Locator.Current.GetService<VM>();
        _viewsStack.Clear();
        _viewsStack.Push((viewModel,null));
        SelectedViewModel = viewModel;
        CanGoBack = false;
    }

    public object Message => _message;


    public void GoBack()
    {
        if (_viewsStack.Count > 1)
        {
            _viewsStack.Pop().Item2?.Invoke();
            SelectedViewModel = _viewsStack.Peek().Item1;
        }
        CanGoBack = _viewsStack.Count > 1;
        _message = null;
    }

    public bool CanGoBack
    {
        get => _canBack;
        private set
        {
            this.RaiseAndSetIfChanged(ref  this._canBack, value);
        }
    }

    public ViewModelBase SelectedViewModel
    {
        get => _selectedViewModel;
        private set 
        {
            this.RaiseAndSetIfChanged(ref _selectedViewModel, value);
        }
    }
}