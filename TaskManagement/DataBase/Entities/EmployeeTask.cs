using System;

namespace TaskManagement.DataBase.Entities;

public class EmployeeTask
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateOnly DateCreate { get; set; }
    public int StatusTaskId { get; set; }
    public int AuthorId { get; set; }
    public int ExecutorId { get; set; }
    
    public virtual StatusTask StatusTask { get; set; } = null!;
    public virtual User Author { get; set; } = null!;
    public virtual User Executor { get; set; } = null!;
    
}