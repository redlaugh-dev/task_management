using System;
using ReactiveUI;

namespace TaskManagement.Models;

public class TaskFilters : ReactiveObject
{
    private string? _searchString; 
    private int? _statusId;
    private int? _executorId;
    private DateOnly? _date;

    public string? SearchString
    {
        get => _searchString;
        set => this.RaiseAndSetIfChanged(ref _searchString, value);
    }

    public int? StatusId
    {
        get => _statusId;
        set => this.RaiseAndSetIfChanged(ref _statusId, value);
    }

    public DateOnly? Date
    {
        get => _date;
        set => this.RaiseAndSetIfChanged(ref _date, value);
    }

    public int? ExecutorId
    {
        get => _executorId;
        set => this.RaiseAndSetIfChanged(ref _executorId, value);
    }
    
}